using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kernel.core
{
	public sealed class KTEditorResManager : Singleton<KTEditorResManager>, IKTResManager
	{
		public bool IsLoading => throw new NotImplementedException();

		public IEnumerator Clear(bool destroy)
		{
			throw new NotImplementedException();
		}

		public float GetAssetProgress(string assetName)
		{
			throw new NotImplementedException();
		}

		public float GetBundleProgress(string bundleName)
		{
			throw new NotImplementedException();
		}

		public void Init()
		{
			throw new NotImplementedException();
		}

		public void LoadAsset(string assetName, Action<string, UnityEngine.Object> completeCallback = null, KTBundleResManager.BundleCacheType cacheType = KTBundleResManager.BundleCacheType.TimeoutCache, float cacheTimeout = 5)
		{
			throw new NotImplementedException();
		}

		public void LoadBundle(string bundleName, Action<string> completeCallback = null)
		{
			throw new NotImplementedException();
		}

		public void LoadScene(string assetName, Action<string> completeCallback = null, LoadSceneMode loadMode = LoadSceneMode.Additive, KTBundleResManager.BundleCacheType cacheType = KTBundleResManager.BundleCacheType.None, float cacheTimeout = 5)
		{
			throw new NotImplementedException();
		}

		public void RecycleAsset(string assetName)
		{
			throw new NotImplementedException();
		}

		public IEnumerator UnloadBundle(string bundleName)
		{
			throw new NotImplementedException();
		}
	}
}