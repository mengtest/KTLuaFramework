using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
using System;

namespace LuaFramework
{
    /// <summary>
    /// 打包要求，被依赖的资源一定与主资源放在同一目录下，主要针对预制体
    /// manifest文件会生成在与资源同目录下
    /// </summary>
    public class KTBuildBase : Editor
    {
        //保存需要打包的资源路径，递归
        private List<string> filesToBuild = new List<string>();
        //需要打包的资源后缀
        private Dictionary<string, string> asExtensionDic = new Dictionary<string, string>();

        /// <summary>
        /// 打包指定目录
        /// </summary>
        protected static void BuildSelectedDir(KTBuildBase build)
        {
            var select = Selection.activeObject;
            var path = AssetDatabase.GetAssetPath(select);
            ListDirs(path, build.filesToBuild);
            build.ListFiles();
            build.ExecuteAssetsBuild();
        }

        /// <summary>
        /// 打包指定目录
        /// </summary>
        protected static void BuildAll(KTBuildBase build)
        {
            ListDirs(build.GetResPath(), build.filesToBuild);
            build.ListFiles();
            build.ExecuteAssetsBuild();
        }

        /// <summary>
        /// 收集文件夹
        /// </summary>
        /// <param name="selectedDir"></param>
        protected static void ListDirs(string selectedDir, List<string> collectedDirs)
        {
            if (!collectedDirs.Contains(selectedDir))
                collectedDirs.Add(selectedDir);
            var dirs = Directory.GetDirectories(selectedDir);
            dirs.ToList().ForEach(dir => ListDirs(dir, collectedDirs));//使用递归方法遍历所有文件夹
        }

        protected static List<string> CollectFiles(string selectedDir,Predicate<string> filter)
        {
            List<string> dirs = new List<string>();
            List<string> files = new List<string>();
            ListDirs(selectedDir, dirs);
            dirs.ForEach(dir =>
            {
                var t_files = Directory.GetFiles(dir);
                files.AddRange(t_files.Where(file => { return filter(file); }));
            });

            return files;
        }

        protected static void UpdateProgress(int progress, int progressMax, string desc)
        {
            string title = "Processing...[" + progress + " - " + progressMax + "]";
            float value = (float)progress / (float)progressMax;
            EditorUtility.DisplayProgressBar(title, desc, value);
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        protected virtual string GetVer() { return "0"; }

        /// <summary>
        /// 资源路径
        /// </summary>
        /// <returns></returns>
        protected virtual string GetResPath() { return "Assets/Resources"; }

        /// <summary>
        /// 目标路径
        /// </summary>
        /// <returns></returns>
        protected virtual string GetDstPath() { return "Assets/StreamingAssets"; }

        /// <summary>
        /// 规则过滤,重命名依赖文件的资源名，默认情况下是打包prefab
        /// </summary>
        /// <param name="file"></param>
        /// <returns>有符合打包条件的资源返回true，其他情况false</returns>
        protected virtual bool AssetsFilter(FileInfo fileInfo) { return false; }

        protected virtual void RenameDependenciesBundleName(FileInfo fileInfo) { }

        private void ListFiles()
        {
            filesToBuild.ForEach((dir) =>
            {
                var files = Directory.GetFiles(dir);
                files.ToList().ForEach((file) =>
                {
                    var info = new FileInfo(file);
                    if (AssetsFilter(info))
                        RenameDependenciesBundleName(info);
                });
            });
        }

        /// <summary>
        /// 执行打包
        /// </summary>
        private void ExecuteAssetsBuild()
        {
            var path = GetDstPath();
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            BuildPipeline.BuildAssetBundles(GetDstPath(), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }

        //清除已经打包的资源 AssetBundleNames
        [MenuItem("LuaFramework/Build/ClearAssetBundlesName")]
        private static void ClearAssetBundlesName()
        {
            AssetDatabase.GetAllAssetBundleNames().ToList().ForEach(bundleName => AssetDatabase.RemoveAssetBundleName(bundleName, true));
        }

        //添加需要打包资源的后缀
        private void SetExtensionDic()
        {
            asExtensionDic.Clear();

            asExtensionDic.Add(".prefab", ".assetbundle");//mesh ui
            asExtensionDic.Add(".controller", ".assetbundle");
            asExtensionDic.Add(".mat", ".assetbundle");//atlas
            asExtensionDic.Add(".jpg", ".assetbundle");
            asExtensionDic.Add(".png", ".assetbundle");
            asExtensionDic.Add(".txt", ".assetbundle");
            asExtensionDic.Add(".xml", ".assetbundle");
            asExtensionDic.Add(".shader", ".assetbundle");
            asExtensionDic.Add(".cs", ".assetbundle");
        }
    }
}
//public class Filter
//{
//    public virtual string ResPath() { return "Assets/Reources"; }
//    public virtual string DstPath() { return "Assets/StreamingAssets"; }

//    /// <summary>
//    /// 规则过滤
//    /// </summary>
//    /// <param name="file"></param>
//    /// <returns>有符合打包条件的资源返回true，其他情况false</returns>
//    public virtual bool AssetsFilter(FileInfo file)
//    {
//        if (file.Exists)
//        {
//            if (file.FullName.EndsWith(".meta"))
//                return false;

//            if (file.FullName.EndsWith(".prefab"))
//            {
//                string[] dps = AssetDatabase.GetDependencies(file.ToString(), false);

//                for (int i = 0; i < dps.Length; i++)
//                {
//                    if (dps[i] == file.ToString())
//                        continue;

//                    //通过资源路径来获取需要打包的资源
//                    AssetImporter ai = AssetImporter.GetAtPath(dps[i]);
//                    ai.assetBundleName = AssetDatabase.AssetPathToGUID(dps[i]);
//                }

//                return true;
//            }
//        }

//        return false;
//    }
//}

////根据切换的平台返回相应的导出路径
//public class Plathform
//{
//    public static string GetPlatformFolder(BuildTarget target)
//    {
//        switch (target)
//        {
//            case BuildTarget.Android:   //Android平台导出到 Android文件夹中
//                return "Android";
//            case BuildTarget.iOS:
//                return "IOS";
//            case BuildTarget.WebPlayer:
//                return "WebPlayer";
//            case BuildTarget.StandaloneWindows:
//            case BuildTarget.StandaloneWindows64:
//                return "Windows";
//            case BuildTarget.StandaloneOSXIntel:
//            case BuildTarget.StandaloneOSXIntel64:
//            case BuildTarget.StandaloneOSXUniversal:
//                return "OSX";
//            default:
//                return null;
//        }
//    }

//}