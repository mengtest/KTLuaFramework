using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LuaFramework
{
    public class KTEditorResourceManager:MonoBehaviour,IKTResourceManager,IDisposable
    {
        public class Loader:IDisposable
        {
            public enum LoaderType
            {
                Asset,
                Scene,
                Bundle
            }

            public LoaderType loaderType = LoaderType.Asset;
            public float progress = 0;
            public AsyncOperation opt = null;

            public void Dispose()
            {
                opt = null;
            }
        }

        private Dictionary<string, Loader> m_assetLoading = new Dictionary<string, Loader>();

        public void Init()
        {
            //读取资源配置表
        }

        public void LoadAsset(string assetName, Action<string, UnityEngine.Object> completeCallback = null, KTBundleInfo.BundleCacheType cacheType = KTBundleInfo.BundleCacheType.None, float cacheTimeout = 5)
        {
            if (!m_assetLoading.ContainsKey(assetName))
                m_assetLoading.Add(assetName, new Loader() { loaderType = Loader.LoaderType.Asset });

            var obj=AssetDatabase.LoadMainAssetAtPath(assetName);
            if (completeCallback != null)
            {
                m_assetLoading[assetName].progress = 1;
                completeCallback(obj != null ? assetName : string.Empty, obj);
                m_assetLoading.Remove(assetName);
            }
        }

        public void LoadBundle(string bundleName, Action<string> completeCallback = null)
        {
            if (!m_assetLoading.ContainsKey(bundleName))
                m_assetLoading.Add(bundleName, new Loader() { loaderType = Loader.LoaderType.Bundle });

            var obj = AssetDatabase.LoadMainAssetAtPath(bundleName);
            if (completeCallback != null)
            {
                m_assetLoading[bundleName].progress = 1;
                completeCallback(obj != null ? bundleName : string.Empty);
                m_assetLoading.Remove(bundleName);
            }
        }

        public void LoadScene(string assetName, Action<string> completeCallback = null, LoadSceneMode loadMode = LoadSceneMode.Additive, KTBundleInfo.BundleCacheType cacheType = KTBundleInfo.BundleCacheType.None, float cacheTimeout = 5)
        {
            StartCoroutine(LoadScene(assetName, completeCallback, loadMode));
        }

        public void RecycleAsset(string assetName)
        {

        }

        public bool IsLoading { get { return m_assetLoading.Count > 0; } }

        public float GetAssetProgress(string assetName)
        {
            return m_assetLoading.ContainsKey(assetName) ? 0 : m_assetLoading[assetName].progress;
        }

        public float GetBundleProgress(string bundleName)
        {
            return m_assetLoading.ContainsKey(bundleName) ? 0 : m_assetLoading[bundleName].progress;
        }

        public IEnumerator RecycleScene(string sceneName)
        {
#if UNITY_EDITOR
            yield return EditorSceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
#else
            yield return break;
#endif
        }

        public IEnumerator RemoveBundle(string bundleName)
        {
            yield break;
        }

        public IEnumerator Clear()
        {
            while (m_assetLoading.Count > 0)
            {
                yield return null;
            }

            m_assetLoading.ToList().ForEach(item => item.Value.Dispose());
            m_assetLoading.Clear();
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        public IEnumerator Stop()
        {
            while (m_assetLoading.Count > 0)
            {
                yield return null;
            }

            m_assetLoading.ToList().ForEach(item => item.Value.Dispose());
            m_assetLoading.Clear();
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
        }

        private IEnumerator LoadScene(string assetName, Action<string> completeCallback = null, LoadSceneMode loadMode = LoadSceneMode.Additive)
        {
#if UNITY_EDITOR
            if (!m_assetLoading.ContainsKey(assetName))
                m_assetLoading.Add(assetName, new Loader() { loaderType = Loader.LoaderType.Scene, opt = EditorSceneManager.LoadSceneAsyncInPlayMode(assetName, new LoadSceneParameters(loadMode)) });

            m_assetLoading[assetName].progress = m_assetLoading[assetName].opt.progress;
            yield return m_assetLoading[assetName].opt;

            if (completeCallback != null)
            {
                m_assetLoading[assetName].progress = 1;
                completeCallback(assetName);
                m_assetLoading.Remove(assetName);
            }
#else
            yield return break;
#endif
        }

        public void Dispose()
        {
            StartCoroutine(Clear());
        }
	}
}
