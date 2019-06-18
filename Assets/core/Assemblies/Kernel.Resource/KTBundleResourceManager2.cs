using LuaFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kernel.core
{
	/// <summary>
	/// 请求的每个原始资源（非Instante的）都会增加对应bundle和依赖bundle的引用计数，返回给业务层的原始对象使用完必须由此管理器回收，每回收一次，对应bundle引用计数-1，如果 bundle设置了TimeoutCache缓存模式
	/// 批量手动卸载资源，必须等待所有正在加载的任务完成，再执行卸载。
	/// </summary>
	public class KTBundleResourceManager2:Singleton<KTBundleResourceManager2>,ITick
	{
		/// <summary>
		/// bundle资源缓存类型
		/// </summary>
		public enum BundleCacheType
		{
			None, //不缓存，加载到的bundle不会缓存，在读取玩资源后会立即unload(false),如果有其他正在请求的不同资源正在等待此bundle，会延后unload(false)
			Share, //常驻内存，必须手动卸载已加载的bundle，即使没被用到过，也不会释放，一般用于共享的依赖资源
			TimeoutCache, //缓存，采用相对时间缓存，首先bundle要有引用计数，即使引用计数为0，也不会立即卸载，而是会根据缓存时间，t秒内未被使用过才会真正卸载，
						  //如果有其他正在请求的不同资源正在等待此bundle，会延后unload(true)
		}

		private class KTBundleInfo: IPriorityItem<string>,IRelease
		{
			public string name;
			public AssetBundle bundle;
			public BundleCacheType cacheType; //bundle的cacheType都延续第一次请求时的cacheType,后续操作不能覆盖
			public float cacheTimeout;
			public float startTime;

			public float Value { get { return startTime; } }

			string IPriorityItem<string>.Key { get { return name; } }

			public void Release(bool destroy = false)
			{
				bundle.Unload(destroy);
				bundle = null;
			}
		}

		private class KTAssetLoader :IRelease
		{
			public enum LoaderType
			{
				Asset,
				Scene
			}

			public string requestAssetName;//有可能为空
			public LoaderType loaderType;

			public string requestBundleName;
			public List<string> dependentBundleNames;//包含requestBundleName
			public BundleCacheType cacheType;
			public float cacheTimeout;

			public Action<KTAssetLoader> assetCallbackForManager;//请求的资源加载完回调到管理器

			//加载asset部分
			public AssetBundleRequest assetRequest;
			public List<Action<string, UnityEngine.Object>> assetCompleteCallbackList;//回调给用户，用数组代替+=事件，避免泄露

			//加载场景部分
			public AsyncOperation sceneRequest;
			public LoadSceneMode loadMode;
			public Action<string> sceneCompleteCallback;

			private List<string> dependentBundleNamesLoading;//当前正在加载的依赖包名列表

			public KTBundleLoader CreateBundleLoader(string bundleName, Action<KTBundleLoader> callback)
			{
				var loader = new KTBundleLoader()
				{
					requestBundleName = bundleName,
					cacheType= bundleName== requestBundleName?cacheType: BundleCacheType.TimeoutCache,
					cacheTimeout = bundleName == requestBundleName ? cacheTimeout:0,
					callbackForManager = callback
				};

				loader.Subscribe(this);

				if (dependentBundleNamesLoading == null)
					dependentBundleNamesLoading = new List<string>() {bundleName};
				else
					dependentBundleNamesLoading.Add(bundleName);

				return loader;
			}

			public void BundleLoadedCallback(KTBundleLoader bundleLoader)
			{
				bundleLoader.UnSubscribe(this);
				dependentBundleNamesLoading.Remove(bundleLoader.requestBundleName);

				if (dependentBundleNamesLoading.Count >0)
					return;

				if (loaderType == LoaderType.Asset)
				{
					assetRequest = bundleLoader.request.assetBundle.LoadAssetAsync(requestAssetName);
					assetRequest.completed += AssetLoadedCallback;
				}
				else if (loaderType == LoaderType.Scene)
				{
					sceneRequest = SceneManager.LoadSceneAsync(requestAssetName, loadMode);
					sceneRequest.completed += SceneLoadedCallback;
				}
			}

			private void AssetLoadedCallback(AsyncOperation assetRequest)
			{
				assetRequest.completed -= AssetLoadedCallback;
				for (int i = 0, j = assetCompleteCallbackList.Count; i < j; ++i)
				{
					assetCompleteCallbackList[i](requestAssetName, (assetRequest as AssetBundleRequest).asset);
				}

				assetCompleteCallbackList.Clear();
				assetCallbackForManager?.Invoke(this);
			}

			private void SceneLoadedCallback(AsyncOperation sceneRequest)
			{
				sceneRequest.completed -= SceneLoadedCallback;
				sceneCompleteCallback?.Invoke(requestAssetName);
				sceneCompleteCallback = null;
				assetCallbackForManager?.Invoke(this);
			}

			public void Release(bool destroy = false)
			{
				sceneCompleteCallback = null;
				assetCallbackForManager = null;

				if (dependentBundleNames != null)
					dependentBundleNames.Clear();

				if (assetCompleteCallbackList != null)
					assetCompleteCallbackList.Clear();

				if (dependentBundleNamesLoading != null)
					dependentBundleNamesLoading.Clear();
			}
		}

		private class KTBundleLoader : IRelease
		{
			public string requestBundleName;
			public BundleCacheType cacheType;
			public float cacheTimeout;
			public AssetBundleCreateRequest request;
			public Action<KTBundleLoader> callbackForManager;
			private event Action<KTBundleLoader> completeCallback;

			public void BeginLoad()
			{
				request = AssetBundle.LoadFromFileAsync(requestBundleName);
				request.completed += BundleLoadedCallback;
			}

			public void Subscribe(KTAssetLoader loader)
			{
				completeCallback += loader.BundleLoadedCallback;
			}

			public void Subscribe(Action<KTBundleLoader> evt)
			{
				completeCallback += evt;
			}

			public void UnSubscribe(KTAssetLoader loader)
			{
				completeCallback -= loader.BundleLoadedCallback;
			}

			public void UnSubscribe(Action<KTBundleLoader> evt)
			{
				completeCallback -= evt;
			}

			private void BundleLoadedCallback(AsyncOperation operation)
			{
				callbackForManager?.Invoke(this);
				callbackForManager = null;
				completeCallback?.Invoke(this);
			}

			public void Release(bool destroy = false)
			{
				completeCallback = null;
				callbackForManager = null;
			}
		}

		private AssetBundleManifest m_manifest;
		private Dictionary<string, string> m_assetNameToBundleNameMap;
		private Dictionary<string, KTBundleInfo> m_bundleLoaded = new Dictionary<string, KTBundleInfo>(16); //key:bundleName
		private Dictionary<string, KTBundleLoader> m_bundleLoading = new Dictionary<string, KTBundleLoader>(16);
		private Dictionary<string, KTAssetLoader> m_assetLoading = new Dictionary<string, KTAssetLoader>(16);
		private Dictionary<string, WeakReference> m_assetNameToWeakPtr = new Dictionary<string, WeakReference>(50);
		private Dictionary<string, int> m_bundleRefCountInfo = new Dictionary<string, int>(50);
		private PriorityQueue<string, KTBundleInfo> m_priorityQueue = new PriorityQueue<string, KTBundleInfo>(30,true);

		public void Init()
		{
			var bundle = AssetBundle.LoadFromFile(Path.Combine(KTPathHelper.DataPath, KTConfigs.kManifestBundleName));
			if (bundle == null)
			{
				Debug.LogError("AssetBundleManifest Load Error");
				return;
			}

			var m_manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
			if (m_manifest == null)
			{
				Debug.LogError("AssetBundleManifest Asset Not Found");
				return;
			}

			bundle.Unload(false);
			bundle = null;

			bundle = AssetBundle.LoadFromFile(Path.Combine(KTPathHelper.DataPath, KTConfigs.kBundleNamesTable));
			if (bundle == null)
			{
				Debug.LogError("BundleNamesTable Load Error");
				return;
			}

			var table =
				bundle.LoadAsset<KTBundleNamesTable>("BundleNamesTable") as KTBundleNamesTable; //需要打包时设定别名为BundleNamesTable
			if (table == null)
			{
				Debug.LogError("BundleNamesTable Asset Not Found");
				return;
			}

			m_assetNameToBundleNameMap = table.ToDictionary();
			bundle.Unload(false);
			bundle = null;

			Application.lowMemory += () =>
			{
				Resources.UnloadUnusedAssets();
				System.GC.Collect();
			};
		}

		/// <summary>
		/// 此种方式只能加载需要预加载的依赖包
		/// </summary>
		/// <param name="bundleName"></param>
		/// <param name="completeCallback"></param>
		/// <param name="cacheTimeout"></param>
		public void LoadBundle(string bundleName, Action<string> completeCallback = null)
		{
			if (!CheckInited())
			{
				completeCallback?.Invoke("");
				return;
			}

			IncreaseRef(bundleName);

			var bundleLoader = new KTBundleLoader()
			{
				requestBundleName = bundleName,
				cacheType = BundleCacheType.Share,
				request = AssetBundle.LoadFromFileAsync(bundleName),
				callbackForManager = BundleCompleteCallback
			};

			Action<KTBundleLoader> callback = null;
			callback=(loader) =>
			{
				completeCallback?.Invoke(bundleName);
				bundleLoader.UnSubscribe(callback);
			};

			bundleLoader.Subscribe(callback);
			m_bundleLoading.Add(bundleName, bundleLoader);
			bundleLoader.BeginLoad();
		}


		/// <summary>
		/// 加载场景
		/// </summary>
		/// <param name="assetName"></param>
		/// <param name="completeCallback"></param>
		/// <param name="loadMode"></param>
		/// <param name="cacheType">可以选三种模式</param>
		/// <param name="cacheTimeout"></param>
		public void LoadScene(string assetName, Action<string> completeCallback = null, LoadSceneMode loadMode = LoadSceneMode.Additive, BundleCacheType cacheType = BundleCacheType.None, float cacheTimeout = 5)
		{
			if (!CheckInited())
			{
				completeCallback?.Invoke("");
				return;
			}

			var requestBundleName = m_assetNameToBundleNameMap[assetName];
			if (string.IsNullOrEmpty(requestBundleName))
			{
				completeCallback?.Invoke("");
				Debug.LogErrorFormat("Asset Info {0} Not Found", assetName);
				return;
			}

			if (m_assetLoading.ContainsKey(assetName))//场景不允许请求重复
			{
				completeCallback?.Invoke("");
				Debug.LogErrorFormat("Request Scene {0} Repeated", assetName);
				return;
			}

			IncreaseRef(requestBundleName);

			var assetLoader = new KTAssetLoader()
			{
				loaderType = KTAssetLoader.LoaderType.Scene,
				requestAssetName = assetName,
				requestBundleName = requestBundleName,
				cacheType = cacheType,
				cacheTimeout = cacheTimeout,
				dependentBundleNames = m_manifest.GetAllDependencies(requestBundleName).ToList(),
				sceneRequest = null,
				loadMode= loadMode,
				sceneCompleteCallback = completeCallback,
				assetCallbackForManager= AssetCompleteCallback
			};
			m_assetLoading.Add(assetName, assetLoader);

			for (int i = 0, j = assetLoader.dependentBundleNames.Count; i < j; ++i)
			{
				var bundleName = assetLoader.dependentBundleNames[i];
				if (m_bundleLoaded.ContainsKey(bundleName))
					continue;

				if(m_bundleLoading.ContainsKey(bundleName))
				{
					m_bundleLoading[bundleName].Subscribe(assetLoader);
				}
				else
				{
					var bundleLoader = assetLoader.CreateBundleLoader(bundleName, BundleCompleteCallback);
					m_bundleLoading.Add(bundleName, bundleLoader);
					bundleLoader.BeginLoad();
				}
			}
		}

		/// <summary>
		/// 加载资源
		/// </summary>
		/// <param name="assetName"></param>
		/// <param name="completeCallback"></param>
		/// <param name="cacheType"></param>
		/// <param name="cacheTimeout"></param>
		public void LoadAsset(string assetName, Action<string, UnityEngine.Object> completeCallback = null, BundleCacheType cacheType = BundleCacheType.TimeoutCache, float cacheTimeout = 5)
		{
			if (!CheckInited())
			{
				completeCallback?.Invoke("", null);
				return;
			}

			var requestBundleName = m_assetNameToBundleNameMap[assetName];
			if (string.IsNullOrEmpty(requestBundleName))
			{
				completeCallback?.Invoke("", null);
				Debug.LogErrorFormat("Asset Info {0} Not Found", assetName);
				return;
			}

			IncreaseRef(requestBundleName);
			if (m_assetLoading.ContainsKey(assetName))//说明之前有同一个资源的请求
			{
				m_assetLoading[assetName].assetCompleteCallbackList.Add(completeCallback);
				return;
			}

			WeakReference weakPtr = null;
			if (m_assetNameToWeakPtr.TryGetValue(assetName, out weakPtr) && weakPtr.IsAlive)
			{
				completeCallback?.Invoke(assetName, (UnityEngine.Object)weakPtr.Target);
				return;
			}

			var assetLoader = new KTAssetLoader()
			{
				loaderType = KTAssetLoader.LoaderType.Asset,
				requestAssetName = assetName,
				requestBundleName = requestBundleName,
				cacheType = cacheType,
				cacheTimeout = cacheTimeout,
				dependentBundleNames = m_manifest.GetAllDependencies(requestBundleName).ToList(),
				assetRequest=null,
				assetCompleteCallbackList = new List<Action<string, UnityEngine.Object>>() { completeCallback },
				assetCallbackForManager = AssetCompleteCallback
			};
			m_assetLoading.Add(assetName, assetLoader);

			for (int i = 0, j = assetLoader.dependentBundleNames.Count; i < j; ++i)
			{
				var bundleName = assetLoader.dependentBundleNames[i];
				if (m_bundleLoaded.ContainsKey(bundleName))
					continue;

				if (m_bundleLoading.ContainsKey(bundleName))
				{
					m_bundleLoading[bundleName].Subscribe(assetLoader);
				}
				else
				{
					var bundleLoader = assetLoader.CreateBundleLoader(bundleName, BundleCompleteCallback);
					m_bundleLoading.Add(bundleName, bundleLoader);
					bundleLoader.BeginLoad();
				}
			}
		}

		public bool IsLoading { get { return m_bundleLoading.Count > 0 || m_assetLoading.Count > 0; } }

		/// <summary>
		/// 获取asset加载进度，包括相关依赖bundle的加载
		/// </summary>
		/// <param name="assetName"></param>
		/// <returns></returns>
		public float GetAssetProgress(string assetName)
		{
			if (!m_assetLoading.ContainsKey(assetName))
				return 1;

			var assetLoader = m_assetLoading[assetName];
			var dependentBundleNames = assetLoader.dependentBundleNames;

			float total = dependentBundleNames.Count + 1;//加上asset的读取进度
			float progress = 0;
			dependentBundleNames.ForEach(bundleName =>
			{
				if (m_bundleLoading.ContainsKey(bundleName))
					progress += m_bundleLoading[bundleName].request.progress;

				if (m_bundleLoaded.ContainsKey(bundleName))
					progress += 1;
			});

			if (m_assetLoading[assetName].assetRequest != null)
				progress += m_assetLoading[assetName].assetRequest.progress;

			if (m_assetLoading[assetName].sceneRequest != null)
				progress += m_assetLoading[assetName].sceneRequest.progress;

			return progress / total;
		}

		/// <summary>
		/// 获取bundleName的进度
		/// </summary>
		/// <param name="bundleName"></param>
		/// <returns></returns>
		public float GetBundleProgress(string bundleName)
		{
			if (m_bundleLoaded.ContainsKey(bundleName))
				return 1;

			if (m_bundleLoading.ContainsKey(bundleName))
				return m_bundleLoading[bundleName].request.progress;

			return 0;
		}

		private void BundleCompleteCallback(KTBundleLoader loader)
		{
			var bundleInfo = new KTBundleInfo()
			{
				name = loader.requestBundleName,
				bundle = loader.request.assetBundle,
				cacheType = loader.cacheType,
				cacheTimeout = loader.cacheTimeout,
				startTime = 0
			};
			m_bundleLoaded.Add(loader.requestBundleName, bundleInfo);
			loader.Release();
			m_bundleLoading.Remove(loader.requestBundleName);
		}

		private void AssetCompleteCallback(KTAssetLoader assetLoader)
		{
			if (assetLoader.loaderType == KTAssetLoader.LoaderType.Asset)
				m_assetNameToWeakPtr.Add(assetLoader.requestAssetName, new WeakReference((assetLoader.assetRequest as AssetBundleRequest).asset));

			if (assetLoader.cacheType == BundleCacheType.None)//请求的主包需要销毁,这种包不会被其他包和资源引用，只会引用其他包
			{
				if (m_bundleLoaded.ContainsKey(assetLoader.requestBundleName))
				{
					m_bundleLoaded[assetLoader.requestBundleName].Release(false);
					m_bundleLoaded.Remove(assetLoader.requestBundleName);
					//DecreaseRef(assetLoader.requestBundleName);这种情况不减引用计数，在回收资源时再去减引用计数
				}
			}

			assetLoader.Release();
			m_assetLoading.Remove(assetLoader.requestAssetName);
		}

		private bool CheckInited()
		{
			if (m_manifest == null)
			{
				Debug.LogError("AssetBundleManifest Not Loaded");
				return false;
			}

			if (m_assetNameToBundleNameMap == null)
			{
				Debug.LogError("AssetNameToBundleNameMap Not Loaded");
				return false;
			}

			return true;
		}

		private void IncreaseRef(string bundleName)
		{
			if (m_bundleRefCountInfo.ContainsKey(bundleName))
				m_bundleRefCountInfo[bundleName]++;
			else
				m_bundleRefCountInfo.Add(bundleName, 1);

			DelQueue(bundleName);

			var dependentBundleNames = m_manifest.GetAllDependencies(bundleName);
			for (int i = 0, j = dependentBundleNames.Length; i < j; ++i)
			{
				var depName = dependentBundleNames[i];
				if (m_bundleRefCountInfo.ContainsKey(depName))
					m_bundleRefCountInfo[depName]++;
				else
					m_bundleRefCountInfo.Add(depName, 1);

				DelQueue(depName);
			}
		}

		private void DecreaseRef(string bundleName)
		{
			if (m_bundleRefCountInfo.ContainsKey(bundleName))
			{
				if (--m_bundleRefCountInfo[bundleName] == 0)
				{
					m_bundleRefCountInfo.Remove(bundleName);
					AddQueue(bundleName);
				}
			}

			var dependentBundleNames = m_manifest.GetAllDependencies(bundleName);
			var deleteKeys = new HashSet<string>();
			for (int i = 0, j = dependentBundleNames.Length; i < j; ++i)
			{
				var depName = dependentBundleNames[i];
				if (--m_bundleRefCountInfo[depName] == 0)
					deleteKeys.Add(depName);
			}

			foreach (var item in deleteKeys)
			{
				m_bundleRefCountInfo.Remove(item);
				AddQueue(item);
			}
		}

		private void AddQueue(string bundleName)
		{
			var bundleInfo = m_bundleLoaded[bundleName];
			if(bundleInfo.cacheType== BundleCacheType.TimeoutCache)
			{
				bundleInfo.startTime = Time.realtimeSinceStartup;
				m_priorityQueue.Push(bundleInfo);
			}
		}

		private void DelQueue(string bundleName)
		{
			var bundleInfo = m_bundleLoaded[bundleName];
			if (bundleInfo.cacheType == BundleCacheType.TimeoutCache&& bundleInfo.startTime != 0)
			{
				m_priorityQueue.Remove(bundleName);
				bundleInfo.startTime = 0;
			}
		}

		public void Tick(float deltaTime)
		{
			while(m_priorityQueue.Count>0&& Time.realtimeSinceStartup-m_priorityQueue.Top().startTime>=m_priorityQueue.Top().cacheTimeout)
			{
				var info=m_priorityQueue.Pop();
				info.Release(true);
				m_bundleLoaded.Remove(info.name);
			}
		}
		
		public void RecycleAsset(string assetName)
		{
			var requestBundleName = m_assetNameToBundleNameMap[assetName];
			DecreaseRef(requestBundleName);
		}

		/// <summary>
		/// 强制卸载顶级依赖bundle,必须等待所有正在加载的任务完成
		/// </summary>
		/// <param name="bundleName"></param>
		/// <returns></returns>
		public IEnumerable<bool> UnloadBundle(string bundleName)
		{
			while (m_bundleLoading.Count > 0)
				yield return false;

			while (m_assetLoading.Count > 0)
				yield return false;

			KTBundleInfo info = null;
			if(m_bundleLoaded.TryGetValue(bundleName,out info))
			{
				if(info.cacheType!= BundleCacheType.Share)
				{
					Debug.LogError("CacheType to Unload Bundle is not BundleCacheType.Share");
					yield return false;
				}

				info.Release(true);
				m_bundleLoaded.Remove(bundleName);
			}
			yield return true;
		}

		/// <summary>
		/// 强制卸载所有已加载的资源，必须先停止优先级队列，等待所有正在加载的任务完成，弱引用表也要清理
		/// </summary>
		public IEnumerable<bool> Clear(bool destroy)
		{
			while (m_bundleLoading.Count > 0)
				yield return false;

			while (m_assetLoading.Count > 0)
				yield return false;

			m_bundleRefCountInfo.Clear();
			m_assetNameToWeakPtr.Clear();
			m_priorityQueue.Clear();

			foreach (var item in m_bundleLoaded)
			{
				item.Value.Release(true);
			}
			m_bundleLoaded.Clear();
			yield return true;
		}
	}

}