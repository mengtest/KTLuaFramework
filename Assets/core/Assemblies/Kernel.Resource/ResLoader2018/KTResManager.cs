using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Kernel.core.KTBundleResManager;

namespace Kernel.core
{
	public interface IKTResManager
	{
		void Init();
		void LoadBundle(string bundleName, Action<string> completeCallback = null);
		void LoadScene(string assetName, Action<string> completeCallback = null, LoadSceneMode loadMode = LoadSceneMode.Additive, BundleCacheType cacheType = BundleCacheType.None, float cacheTimeout = 5);
		void LoadAsset(string assetName, Action<string, UnityEngine.Object> completeCallback = null, BundleCacheType cacheType = BundleCacheType.TimeoutCache, float cacheTimeout = 5);
		bool IsLoading { get; }
		float GetAssetProgress(string assetName);
		float GetBundleProgress(string bundleName);
		void RecycleAsset(string assetName);
		IEnumerator UnloadBundle(string bundleName);
		IEnumerator Clear(bool destroy);
	}

	public class KTResManager : Singleton<KTResManager>, IKTResManager, IDisposable
	{
		private IKTResManager m_manager = null;

		public void Init()
		{
#if UNITY_EDITOR
			m_manager = KTEditorResManager.Instance;
#else
            m_manager = KTBundleResManager.Instance;
#endif

			m_manager.Init();
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

		public IEnumerator Clear(bool destroy)
		{
			yield return m_manager.Clear(destroy);
		}
		public IEnumerator UnloadBundle(string bundleName)
		{
			yield return m_manager.UnloadBundle(bundleName);
		}

		public void Dispose()
		{
			if (m_manager != null)
				(m_manager as IDisposable).Dispose();
		}
	}
}