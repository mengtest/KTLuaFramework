using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// assetName与assetBundleName名映射表
/// </summary>
public class KTBundleNamesTable : ScriptableObject
{
    public List<KTKeyValuePair> assetNameToBundleNameMap = new List<KTKeyValuePair>();
    
    public Dictionary<string,string> ToDictionary()
    {
        return assetNameToBundleNameMap.ToDictionary(Ky => Ky.key, ky => ky.value);
    }
}
