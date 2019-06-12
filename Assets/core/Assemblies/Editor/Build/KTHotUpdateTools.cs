using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Octodiff;

namespace LuaFramework
{
    public class KTHotUpdateTools
    {
        private static List<string> paths = new List<string>();
        private static List<string> files = new List<string>();

        /// <summary>
        /// 拷贝新资源到指定目录，以便将来做文件差异对比
        /// 把NewVersion的内容拷贝到OldVersion中
        /// 把streamingAssetsPath的内容拷贝到NewVersion中
        /// todo:未来如果分包压缩，可能不只是压缩Application.streamingAssetsPath，而是它的多个子目录
        /// 这样files.txt里就会包含多个zip数据，热更新管理器需要修改
        /// </summary>
        [MenuItem("LuaFramework/HotUpdateTools/MakeNewVersionSrc", false, 103)]
        public static void MakeNewVersionSrc()
        {
            string srcName = Path.Combine(Application.dataPath, "NewVersion", KTConfigs.kSrcName);
            string fileName = Path.Combine(Application.dataPath,"NewVersion/files.txt");
            string cpySrcName = Path.Combine(Application.dataPath,"OldVersion",KTConfigs.kSrcName);
            string cpyFileName = Path.Combine(Application.dataPath,"OldVersion/files.txt");
            if (File.Exists(srcName) && File.Exists(fileName))
            {
                File.Copy(srcName, cpySrcName, true);
                File.Delete(srcName);
                File.Copy(fileName, cpyFileName, true);
                File.Delete(fileName);
            }

            ZipTool.ZipFile(Application.streamingAssetsPath, srcName);//创建资源到NewVersion中
            BuildZipFileIndex();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建增量delta文件到NewVersion以便将来放到服务器给游戏加载
        /// </summary>
        [MenuItem("LuaFramework/HotUpdateTools/CreateDeltaFile", false, 104)]
        public static void CreateDeltaFile()
        {
            string oldSrcName = Path.Combine("OldVersion" , KTConfigs.kSrcName);
            string octosigName = Path.Combine("NewVersion",KTConfigs.kSigName);
            string newSrcName = Path.Combine("NewVersion", KTConfigs.kSrcName);
            string deltaName = Path.Combine("NewVersion" ,KTConfigs.kDeltaName);
            if (!File.Exists(oldSrcName) || !File.Exists(newSrcName))
            {
                UnityEngine.Debug.LogError(oldSrcName + " or" + newSrcName + " not exist");
                return;
            }

            Directory.SetCurrentDirectory(Application.dataPath);
            OctodiffUtil.ExecuteSignature(oldSrcName, octosigName);
            OctodiffUtil.ExecuteDelta(octosigName, newSrcName, deltaName);
            Directory.SetCurrentDirectory(Application.dataPath.Replace("/Assets", ""));
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 组合增量文件测试到选择目录中
        /// </summary>
        [MenuItem("LuaFramework/HotUpdateTools/PitchDeltaFile", false, 105)]
        public static void PitchDeltaFile()
        {
            Object obj = Selection.activeObject;
            if (obj == null)
                return;

            string oldSrcName = Path.Combine("OldVersion" ,KTConfigs.kSrcName);
            string deltaName = Path.Combine("NewVersion",KTConfigs.kDeltaName);
            string newSrcName = Path.Combine(AssetDatabase.GetAssetPath(obj),KTConfigs.kSrcName);

            Directory.SetCurrentDirectory(Application.dataPath);
            OctodiffUtil.ExecutePitch(oldSrcName, deltaName, newSrcName);
            Directory.SetCurrentDirectory(Application.dataPath.Replace("/Assets", ""));
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 创建初始资源文件或增量更新文件的index file文件到NewVersion目录中
        /// </summary>
        private static void BuildZipFileIndex()
        {
            string newFilePath = Path.Combine(Application.dataPath ,"NewVersion/files.txt");
            string srcName = Path.Combine(Application.dataPath, "NewVersion", KTConfigs.kSrcName);
            FileStream fs = new FileStream(newFilePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            string md5 = Util.md5file(srcName);
            sw.WriteLine(KTConfigs.kSrcName + "|" + md5);
            sw.Close();
            fs.Close();
        }
    }
}