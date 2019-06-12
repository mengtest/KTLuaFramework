using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR 
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// 用于添加打包配置数据
/// </summary>
public class KTBundleConfig : ScriptableObject
{
    [System.Serializable]
    public struct KTAssetBundleBuild
    {
        public string assetBundleName;
        public string[] assetNames;
        public string[] addressableNames;
    }

    public List<KTAssetBundleBuild> assetBundleBuildList=new List<KTAssetBundleBuild>();

#if UNITY_EDITOR 
    public AssetBundleBuild[] ToAssetBundleBuild()
    {
        return assetBundleBuildList.Select(assetBundleBuild =>
        {
            return new AssetBundleBuild()
            {
                assetBundleName = assetBundleBuild.assetBundleName,
                assetNames = assetBundleBuild.assetNames,
                addressableNames =assetBundleBuild.addressableNames
            };
        }).ToArray();
    }
#endif
}