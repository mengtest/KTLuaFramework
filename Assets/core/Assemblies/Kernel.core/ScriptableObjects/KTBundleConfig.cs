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
	public enum BundleType
	{
		TopLevel,//包中的资源都是会被直接请求的资源，他们只会依赖其他包，此包不可能被其他包依赖
		Auxiliary,//辅助包，这种包只会被其他直接请求的唯一资源依赖，不会被多分资源依赖
		Share,//共享包，会被多个直接请求的资源直接依赖
	}


    [System.Serializable]
    public struct KTAssetBundleBuild
    {
        public string assetBundleName;
		public BundleType type;
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