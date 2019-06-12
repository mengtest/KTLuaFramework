using System.Collections.Generic;
using UnityEngine;

namespace LuaFramework
{
    public class TestScriptObj : ScriptableObject
    {
        [System.Serializable]
        public class BundleConfig
        {
            public string bundleName;
            public string srcPath;
        }
        public List<BundleConfig> content;
    }
}