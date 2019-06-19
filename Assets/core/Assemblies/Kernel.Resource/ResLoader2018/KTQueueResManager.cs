using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static Kernel.core.KTBundleResManager;

namespace Kernel.core
{
	/// <summary>
	/// 队列加载器，主要用于场景切换时的预加载，只有结束回调和提供进度接口，没有单个文件加载回调
	/// 游戏过程中如果想加载多个资源来做一件事，则由游戏内部来处理加载列表，典型的例子是 人物的各个组件，可以直接使用KTResourceManager
	/// 加载一旦开始，中途不允许加入新请求
	/// </summary>
	public class KTQueueResManager : IDisposable
	{
		public class Loader
		{
			public enum LoaderType
			{
				Asset,
				Scene,
				Bundle
			}

			public string name;
			public LoaderType loaderType;
			public BundleCacheType cacheType;
			public float cacheTimeout;
			public LoadSceneMode loadMode;
		}

		private KTResManager m_resManager = new KTResManager();
		private Queue<Loader> m_loaderQueue = new Queue<Loader>();
		private float m_total = 0;
		private Action m_completeCallback;

		public void Init()
		{
			m_resManager = new KTResManager();
			m_resManager.Init();
			m_loaderQueue = new Queue<Loader>();
		}

		public void AddBundle(string name)
		{
			var loader = new Loader()
			{
				name = name,
				loaderType = Loader.LoaderType.Bundle
			};
			m_loaderQueue.Enqueue(loader);
		}


		public void AddScene(string name, LoadSceneMode loadMode, BundleCacheType cacheType, float cacheTimeout = 5)
		{
			var loader = new Loader()
			{
				name = name,
				loaderType = Loader.LoaderType.Scene,
				loadMode = loadMode,
				cacheType = cacheType,
				cacheTimeout = cacheTimeout
			};
			m_loaderQueue.Enqueue(loader);
		}

		public void AddAsset(string name, BundleCacheType cacheType, float cacheTimeout = 5)
		{
			var loader = new Loader()
			{
				name = name,
				loaderType = Loader.LoaderType.Asset,
				cacheType = cacheType,
				cacheTimeout = cacheTimeout
			};
			m_loaderQueue.Enqueue(loader);
		}

		public void Start(Action completeCallback)
		{
			m_total = m_loaderQueue.Count;
			m_completeCallback = completeCallback;
			ExecuteLoad();
		}

		public void Stop()
		{
			m_loaderQueue.Clear();
			m_completeCallback = null;
		}

		public bool IsLoading { get { return m_resManager.IsLoading; } }

		public float Progress { get { return m_total - m_loaderQueue.Count; } }

		private void ExecuteLoad()
		{
			if (m_loaderQueue.Count == 0)
			{
				if (m_completeCallback != null)
				{
					m_completeCallback();
					m_completeCallback = null;
				}
				return;
			}

			var loader = m_loaderQueue.Dequeue();
			switch (loader.loaderType)
			{
				case Loader.LoaderType.Bundle:
					m_resManager.LoadBundle(loader.name, name => ExecuteLoad());
					break;
				case Loader.LoaderType.Scene:
					m_resManager.LoadScene(loader.name, name => ExecuteLoad(), loader.loadMode, loader.cacheType, loader.cacheTimeout);
					break;
				case Loader.LoaderType.Asset:
					m_resManager.LoadAsset(loader.name, (name, obj) => ExecuteLoad(), loader.cacheType, loader.cacheTimeout);
					break;
			}
		}

		public void Dispose()
		{
			Stop();
		}
	}
}
