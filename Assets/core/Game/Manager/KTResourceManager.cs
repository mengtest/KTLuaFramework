using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LuaFramework.KTBundleInfo;

namespace LuaFramework
{
    public interface IKTResourceManager
    {
        void Init();
        void LoadBundle(string bundleName, Action<string> completeCallback = null);
        void LoadScene(string assetName, Action<string> completeCallback = null, LoadSceneMode loadMode = LoadSceneMode.Additive, BundleCacheType cacheType = BundleCacheType.None, float cacheTimeout = 5);
        void LoadAsset(string assetName, Action<string, UnityEngine.Object> completeCallback = null, BundleCacheType cacheType = BundleCacheType.None, float cacheTimeout = 5);
        void RecycleAsset(string assetName);
        bool IsLoading { get; }
        float GetAssetProgress(string assetName);
        float GetBundleProgress(string bundleName);
        IEnumerator RecycleScene(string sceneName);
        IEnumerator RemoveBundle(string bundleName);
        IEnumerator Clear();
        IEnumerator Stop();
    }

    public class KTResourceManager : MonoBehaviour,IKTResourceManager,IDisposable
    {
        private IKTResourceManager m_manager = null;

        public void Init()
        {
#if UNITY_EDITOR
            m_manager = gameObject.AddComponent<KTEditorResourceManager>();
#else
            m_manager = gameObject.AddComponent<KTBundleResourceManager>();
#endif
        }

        public void LoadBundle(string bundleName, Action<string> completeCallback = null)
        {
            m_manager.LoadBundle(bundleName, completeCallback);
        }

        public void LoadScene(string assetName, Action<string> completeCallback = null, LoadSceneMode loadMode = LoadSceneMode.Additive, BundleCacheType cacheType = BundleCacheType.None, float cacheTimeout = 5)
        {
            m_manager.LoadScene(assetName, completeCallback, loadMode, cacheType, cacheTimeout);
        }
        public void LoadAsset(string assetName, Action<string, UnityEngine.Object> completeCallback = null, BundleCacheType cacheType = BundleCacheType.None, float cacheTimeout = 5)
        {
            m_manager.LoadAsset(assetName, completeCallback, cacheType, cacheTimeout);
        }

        public void RecycleAsset(string assetName)
        {
            m_manager.RecycleAsset(assetName);
        }

        public bool IsLoading { get { return m_manager.IsLoading; } }

        public float GetAssetProgress(string assetName)
        {
            return m_manager.GetAssetProgress(assetName);
        }

        public float GetBundleProgress(string bundleName)
        {
            return m_manager.GetBundleProgress(bundleName);
        }

        public IEnumerator RecycleScene(string sceneName)
        {
            yield return m_manager.RecycleScene(sceneName);
        }

        public IEnumerator RemoveBundle(string bundleName)
        {
            yield return m_manager.RemoveBundle(bundleName);
        }

        public IEnumerator Clear()
        {
            yield return m_manager.Clear();
        }

        public IEnumerator Stop()
        {
            yield return m_manager.Stop();
        }

        public void RecycleSceneAsync(string sceneName)
        {
            StartCoroutine(m_manager.RecycleScene(sceneName));
        }

        public void RemoveBundleAsync(string bundleName)
        {
            StartCoroutine(m_manager.RemoveBundle(bundleName));
        }

        public void ClearAsync()
        {
            StartCoroutine(m_manager.Clear());
        }

        public void StopAsync()
        {
            StartCoroutine(m_manager.Stop());
        }

        public void Dispose()
        {
            if(m_manager!=null)
                (m_manager as IDisposable).Dispose();
        }
    }
}