using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    /// <summary>
    /// KTBuildBase针对资源root目录下的所有资源打包到目标目录，所有文件都会被单独打成一个包，不论他们的目录深度如何，最终都会打到同一个目标目录中
    /// 并且不丢失依赖关系，目标目录下只有一个总的manifest文件
    /// </summary>
    public class KTBuildTools : KTBuildBase
    {
        /// <summary>
        /// 打包指定目录，只能选择要打包的资源root目录
        /// 每个文件单独打包方式
        /// </summary>
        [MenuItem("LuaFramework/Build/BuildSelectedFolder")]
        private static void BuildSelectedFolder()
        {
            KTBuildTools.BuildSelectedDir(CreateInstance<KTBuildTools>());
        }

        /// <summary>
        /// 打包指定目录
        /// 每个文件单独打包方式
        /// </summary>
        [MenuItem("LuaFramework/Build/BuildResFolder")]
        private static void BuildSrcFolder()
        {
            KTBuildTools.BuildAll(CreateInstance<KTBuildTools>());
        }

        /// <summary>
        /// 打包BundleConfig文件中的内容
        /// 每次打包，必须是所有资源一起打包，不能单独打某个类型资源的包，否则BundleConfig bundleConfig和
        /// </summary>
        [MenuItem("LuaFramework/Build/BuildAssetBundles")]
        private static void BuildAssetBundles()
        {
            var tools = CreateInstance<KTBuildTools>();
            string bundleConfigPath = Path.Combine(tools.GetResPath(), "BundleConfig.asset");
            string bundleNamesTablePath = Path.Combine(tools.GetResPath(), "BundleNamesTable.asset");
            var bundleConfig = AssetDatabase.LoadAssetAtPath<KTBundleConfig>(bundleConfigPath);
            var bundleNamesTable = AssetDatabase.LoadAssetAtPath<KTBundleNamesTable>(bundleNamesTablePath);

            //打包lua或收集lua打包信息
            if (KTConfigs.kLuaBundleMode)
                BuildLua(bundleConfig);
            else
                CopyLuaFiles();

            if (bundleConfig.assetBundleBuildList == null || bundleConfig.assetBundleBuildList.Count == 0)
                return;

            //导出资源名字映射表，用别名作为key，配置BundleConfig时必须设定别名
            bundleNamesTable.assetNameToBundleNameMap.Clear();
            bundleConfig.assetBundleBuildList.ForEach(buildMap =>
            {
                buildMap.addressableNames.ToList().ForEach(addressableName => bundleNamesTable.assetNameToBundleNameMap.Add(new KTKeyValuePair() { key = addressableName, value = buildMap.assetBundleName }));
            });

            EditorUtility.SetDirty(bundleNamesTable);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("export bundle names table finished");

            if (!Directory.Exists(tools.GetDstPath()))
                Directory.CreateDirectory(tools.GetDstPath());

            BuildPipeline.BuildAssetBundles(tools.GetDstPath(), bundleConfig.ToAssetBundleBuild(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            Debug.Log("build bundle assets finished");

            CopyPbFiles();
        }

        /// <summary>
        /// 打包BundleConfig文件中的内容
        /// 如果是bundle模式，则收集lua数据到bundleConfig中一起打包，否则直接拷贝lua根目录到目标目录
        /// </summary>
        private static void BuildLua(KTBundleConfig bundleConfig)
        {
            //拷贝lua文件到临时目录，把所有lua文件后缀改为bytes，以便可以打进bundle包
            var srcRoot = Path.Combine(Application.dataPath, KTConfigs.kLuaCodeDir);
            var tmpRoot = Path.Combine(Application.dataPath, KTConfigs.kLuaTempDir);

            List<string> luaDirs = new List<string>();
            ListDirs(srcRoot, luaDirs);
            luaDirs.ForEach(dir =>
            {
                var newDir = dir.Replace(srcRoot, tmpRoot);
                if (!Directory.Exists(newDir))
                    Directory.CreateDirectory(newDir);
            });
            luaDirs.ForEach(dir =>
            {
                var files = Directory.GetFiles(dir, "*.lua");
                files.ToList().ForEach(file => File.Copy(file, file.Replace(srcRoot, tmpRoot).Replace(KTConfigs.kLuaExt, KTConfigs.kLuaBytesExt), true));
            });

            Debug.Log("copy lua files to temp folder finished");

            //统计lua文件进KTAssetBundleBuild信息
            luaDirs.Clear();
            ListDirs(tmpRoot, luaDirs);
            List<string> luaFiles = new List<string>();
            luaDirs.ForEach(dir =>
            {
                var files = Directory.GetFiles(dir, "*.bytes").Select(file =>
             {
                    return ("Assets" + file.Replace(Application.dataPath, "")).Replace("\\", "/").Replace(KTConfigs.kLuaExt, KTConfigs.kLuaBytesExt);
                });
                luaFiles.AddRange(files);
            });

            if (luaFiles.Count > 0)
            {
                KTBundleConfig.KTAssetBundleBuild abb = new KTBundleConfig.KTAssetBundleBuild()
                {
                    assetBundleName = "lua.bundle",
                    assetNames = (string[])luaFiles.ToArray().Clone(),
                    addressableNames = luaFiles.Select(name => { return name.Replace("Assets/Temp/", "").Replace(KTConfigs.kLuaBytesExt, ""); }).ToArray()
                };
                bundleConfig.assetBundleBuildList.Add(abb);
                EditorUtility.SetDirty(bundleConfig);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Debug.Log("bundle lua files finished");
        }

        /// <summary>
        /// 每个单位的模型贴图、动画一起打包
        /// 可能还有控制器，网格，材质
        /// </summary>
        private static void BuildUnit(KTBundleConfig bundleConfig)
        {

        }

        /// <summary>
        /// 自定义shader,属于公共依赖包
        /// </summary>
        private static void BuildShaders(KTBundleConfig bundleConfig)
        {

        }

        /// <summary>
        /// 公用图集一起打包（UIAtlas.prefab、UIAtlas.mat、UIAtlas.png）在进入游戏的时候加载并且常驻内存,属于公共依赖包
        /// 单独的UI可能还要单独打包，每个单独的UI，可能包括prefab,mat,png等等
        /// </summary>
        private static void BuildUI(KTBundleConfig bundleConfig)
        {

        }

        /// <summary>
        /// 打包音乐，音效，大型音乐可能要分包打
        /// </summary>
        private static void BuildAudio(KTBundleConfig bundleConfig)
        {

        }

        /// <summary>
        /// 关卡相关，场景，模型，材质，贴图，光照贴图、寻路数据等
        /// </summary>
        private static void BuildLevel(KTBundleConfig bundleConfig)
        {
            
        }

        private static void BuildScene(KTBundleConfig bundleConfig)
        {
            var tools = CreateInstance<KTBuildTools>();
            string[] levels = { "Assets/" + tools.GetDstPath() + ".unity" };
            BuildPipeline.BuildPlayer(levels,Path.Combine(tools.GetDstPath(),"scene.bundle"), BuildTarget.StandaloneWindows64, BuildOptions.BuildAdditionalStreamedScenes);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 特效包，包括动画，模型，材质，贴图
        /// </summary>
        private static void BuildParticle(KTBundleConfig bundleConfig)
        {
            
        }

        /// <summary>
        /// 拷贝lua开发目录到目标目录
        /// </summary>
        private static void CopyLuaFiles()
        {
            var srcRoot = Path.Combine(Application.dataPath, KTConfigs.kLuaCodeDir);
            var dstRoot = Path.Combine(Application.streamingAssetsPath, "lua");
            CopyFiles(srcRoot, dstRoot, "*.lua");
            Debug.Log("copy lua files finished");
        }

        /// <summary>
        /// 拷贝Pb开发目录到目标目录
        /// </summary>
        private static void CopyPbFiles()
        {
            var srcRoot = Path.Combine(Application.dataPath,KTConfigs.kPbDir);
            var dstRoot = Path.Combine(Application.streamingAssetsPath, KTConfigs.kPbDir);
            CopyFiles(srcRoot, dstRoot, "*.pb");
            Debug.Log("copy pb files finished");
        }

        /// <summary>
        /// 拷贝文件到指定目录
        /// </summary>
        /// <param name="srcRoot"></param>
        /// <param name="dstRoot"></param>
        /// <param name="pattern">通配符</param>
        private static void CopyFiles(string srcRoot,string dstRoot,string pattern="*.*")
        {
            List<string> dirs = new List<string>();
            ListDirs(srcRoot, dirs);
            dirs.ForEach(dir =>
            {
                var dstDir = dir.Replace(srcRoot, dstRoot);
                if (!Directory.Exists(dstDir))
                    Directory.CreateDirectory(dstDir);
            });
            dirs.ForEach(dir =>
            {
                var files = Directory.GetFiles(dir, pattern);
                files.ToList().ForEach(file => File.Copy(file, file.Replace(srcRoot, dstRoot), true));
            });
        }

        /// <summary>
        /// 资源路径
        /// </summary>
        /// <returns></returns>
        protected override string GetResPath() { return "Assets/BuildRes"; }

        /// <summary>
        /// 目标路径
        /// </summary>
        /// <returns></returns>
        protected override string GetDstPath() { return "Assets/StreamingAssets/BuildRes"; }

        /// <summary>
        /// 规则过滤,默认情况下是打包prefab
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns>有符合打包条件的资源返回true，其他情况false</returns>
        protected override bool AssetsFilter(FileInfo fileInfo)
        {
            return fileInfo.Exists && !fileInfo.FullName.EndsWith(".meta");
        }

        protected override void RenameDependenciesBundleName(FileInfo fileInfo)
        {
            var dps = AssetDatabase.GetDependencies(fileInfo.ToString());
            dps.Where(f =>
            {
                return Path.GetExtension(f) != ".cs";
            })
            .ToList()
            .ForEach(f2 =>
            {
                AssetImporter ai = AssetImporter.GetAtPath(f2);
                ai.assetBundleName = Path.GetFileNameWithoutExtension(fileInfo.Name) + KTConfigs.kBundleExt;
            });
        }
    }
}