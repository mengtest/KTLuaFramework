using System.IO;
using UnityEngine;

namespace LuaFramework
{
    public class KTPathHelper
    {
        /// <summary>
        /// 调试游戏运行时的资源加载路径，打包时的目标目录，运行释放时的源目录
        /// 不同平台的streamingAssetsPath目录
        /// </summary>
        public static string AppContentPath()
        {
            string path = string.Empty;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    path = "jar:file://" + Application.dataPath + "!/assets/";
                    break;
                case RuntimePlatform.IPhonePlayer:
                    path = Application.dataPath + "/Raw/";
                    break;
                default:
                    path = Application.streamingAssetsPath+"/";
                    break;
            }
            return path;
        }

        /// <summary>
        /// 取得数据存放目录
        /// 游戏运行时根据情况读取指定目录资源
        /// 除了windows编辑器 特殊，其他都是persistentDataPath，调试模式例外
        /// </summary>
        public static string DataPath
        {
            get
            {
                string game = KTConfigs.kAppName.ToLower();
                if (Application.isMobilePlatform)
                {
                    if(KTConfigs.kDebugMode)
                        return Application.streamingAssetsPath + "/";
                    else
                        return Application.persistentDataPath + "/" + game + "/";
                }
                else if (Application.platform == RuntimePlatform.WindowsEditor)//认为是kDebugMode模式
                {
                    return Application.streamingAssetsPath + "/";
                }
                else if (Application.platform == RuntimePlatform.OSXEditor)//认为是kDebugMode模式
                {
                    int i = Application.dataPath.LastIndexOf('/');
                    return Application.dataPath.Substring(0, i + 1) + game + "/";
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    if (KTConfigs.kDebugMode)
                        return Application.streamingAssetsPath + "/";
                    else
                        return "c:/" + game + "/";
                }

                return "";
            }
        }

        public static string GetRelativePath()
        {
            if (Application.isEditor)
                return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/" + KTConfigs.kAssetDir + "/";
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
                return "file:///" + DataPath;
            else // For standalone player.
                return "file://" + Application.streamingAssetsPath + "/";
        }

        public static string LuaCodePath
        {
            get
            {
#if UNITY_EDITOR
                return Path.Combine(Application.dataPath, KTConfigs.kLuaCodeDir).Replace("\\", "/");
#else
                return KTPathHelper.DataPath + "lua";
#endif
            }
        }
    }
}
