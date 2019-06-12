using UnityEngine;
using System.IO;
using UnityEngine.Assertions;

public class KTConfigs
{
    public const bool kDebugMode = false;                       //调试模式-表示在编辑器下，或windows运行时，或手机端，读取的目录都从streamingAssets内读取                                                    
    public const bool kLuaByteMode = true;                      //Lua字节码模式-默认关闭 
    public const bool kLuaBundleMode = false;                   //Lua代码AssetBundle模式
    public const bool kUpdateMode = false;                      //是否开启热更新-默认关闭 
    public const bool kIncrementalmMode = true;                 //是否使用增量更新模式

    public const int kTimerInterval = 1;
    public const int kGameFrameRate = 30;                        //游戏帧频

    public const string kAppName = "KTFramework";                //应用程序名称
    public const string kManifestBundleName ="BuildRes/BuildRes";//主AssetBundleManifest包名
    public const string kBundleNamesTable = "BuildRes/bundlenamestable.bundle";//BundleNamesTable映射表包名
    public const string kLuaTempDir = "Temp";                    //临时目录
    public const string kAppPrefix = kAppName + "_";             //应用程序前缀
    public const string kExtName = ".unity3d";                   //素材扩展名
    public const string kAssetDir = "StreamingAssets";           //素材目录 
    public const string kWebUrl = "http://118.198.132.14/";      //测试更新地址
    public const string kSrcName = "src.zip";                    //增量资源包名
    public const string kDeltaName = "src.zip.delta";            //增量文件名
    public const string kSigName = "src.zip.sig";                //sig文件名
    public const string kSceneExt = ".unity";
    public const string kLuaExt = ".lua";
    public const string kDllExt = ".dll";
    public const string kSoExt = ".so";
    public const string kLuaBytesExt = ".bytes";
    public const string kAssetExt = ".asset";
    public const string kBundleExt = ".bundle";
    public const string kLuaCodeDir = "core/LuaScripts/luacode"; //编辑器模式下运行时的lua读取目录
    public const string kPbDir = "BuildRes/PbProtocol";          //pb协议文件目录

    public static string kUserId = string.Empty;                 //用户ID
    public static int kSocketPort = 0;                           //Socket服务器端口
    public static string kSocketAddress = string.Empty;          //Socket服务器地址

    public static string FrameworkRoot
    {
        get
        {
            return Application.dataPath + "/" + kAppName;
        }
    }
}