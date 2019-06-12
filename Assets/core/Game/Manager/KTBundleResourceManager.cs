using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LuaFramework.KTBundleInfo;

namespace LuaFramework
{
    public class KTBundleInfo
    {
        /// <summary>
        /// bundle资源缓存类型
        /// </summary>
        public enum BundleCacheType
        {
            None,//不缓存，加载到的bundle不会缓存，在读取玩资源后会立即unload(false),如果有其他正在请求的不同资源正在等待此bundle，会延后unload(false)
            Share,//常驻内存，必须手动卸载已加载的bundle，即使没被用到过，也不会释放，一般用于共享的依赖资源
            TimeoutCache,//缓存，采用相对时间缓存，首先bundle要有引用计数，即使引用计数为0，也不会立即卸载，而是会根据缓存时间，t秒内未被使用过才会真正卸载，
                         //如果有其他正在请求的不同资源正在等待此bundle，会延后unload(true)
        }

        public string name;
        public AssetBundle bundle = null;
        public BundleCacheType cacheType = BundleCacheType.None; //bundle的cacheType都延续第一次请求时的cacheType,后续操作不能覆盖
        public float cacheTimeout = 5;
        public float startTime = 0;
        private int m_refCount = 0;

        public int refCount
        {
            get { return m_refCount; }
            set
            {
                if (cacheType == BundleCacheType.TimeoutCache)
                {
                    m_refCount = value;
                    startTime = m_refCount > 0 ? 0 : Time.realtimeSinceStartup;
                }
            }
        }

        public void Release(bool destroy)
        {
            bundle.Unload(destroy);
            bundle = null;
        }
    }

    public class KTAssetLoader : IDisposable
    {
        public enum LoaderType
        {
            Asset,
            Scene
        }

        public string requestBundleName;
        public string requestAssetName;//有可能为空
        public BundleCacheType cacheType = BundleCacheType.None;
        public float cacheTimeout = 5;
        public List<string> dependentBundleNames;//包含requestBundleName
        public AssetBundleRequest request;
        public List<Action<string, UnityEngine.Object>> completeCallbackList;
        public bool isAllBundleLoaded = false;//标记依赖的bundle是否都加载完
        public bool isAllBundleLoading = false;//标记开始加载bundle包括依赖的

        public AsyncOperation sceneRequest;
        public LoadSceneMode loadMode = LoadSceneMode.Additive;
        public Action<string> sceneCompleteCallback;
        public LoaderType loaderType = LoaderType.Asset;

        public void Dispose()
        {
            request = null;
            sceneRequest = null;
            sceneCompleteCallback = null;

            if (dependentBundleNames != null)
            {
                dependentBundleNames.Clear();
                dependentBundleNames = null;
            }

            if (completeCallbackList != null)
            {
                completeCallbackList.Clear();
                completeCallbackList = null;
            }
        }
    }

    public class KTBundleLoader : IDisposable
    {
        public string requestBundleName;
        public BundleCacheType cacheType = BundleCacheType.None;
        public float cacheTimeout = 5;
        public AssetBundleCreateRequest request;
        public Action<string> completeCallback;

        public void Dispose()
        {
            request = null;
            completeCallback = null;
        }
    }

    /// <summary>
    /// 资源管理器
    /// 所有的资源请求都会在下一帧才会返回，不会直接返回
    /// 对加载的bundle做了缓存设定，具体请看BundleCacheType说明
    /// bundleInfo的生存期与m_assetNameToWeakPtr中对象的生存期不相干
    /// 存在一个情况，就是assetLoader需要等待所有相关依赖的bundle全部加载完才会去读取bundle中的asset，
    /// 这个时候如果bundle设置了不缓存，可能需要调用Unload(false)来做清理
    /// 但这个时候其他assetLoader可能同时依赖这个bundle而且需要等待所有相关bundle加载完才会读取这个bundle
    /// 如果卸载了这个bundle，等待其他assetLoader想读取这个bundle的时候就会访问不到，而且也不再会中途开启新加载
    /// 不然会出现不断卸载，加载存在，所以需要m_assetsWaitingBundle记录assetLoader和bundleInfo的关系
    /// </summary>
    public class KTBundleResourceManager:MonoBehaviour, IKTResourceManager,IDisposable
    {
        private AssetBundleManifest m_manifest;
        private Dictionary<string, string> m_assetNameToBundleNameMap;
        private Dictionary<string, KTBundleInfo> m_bundleLoaded = new Dictionary<string, KTBundleInfo>();//key:bundleName
        private Dictionary<string, KTBundleLoader> m_bundleLoading = new Dictionary<string, KTBundleLoader>();//key:bundleName
        private Dictionary<string, KTAssetLoader> m_assetLoading = new Dictionary<string, KTAssetLoader>();//key:assetName loadiing scene中和loading asset中的信息都在这里保存
        private Dictionary<string, HashSet<string>> m_assetsWaitingBundle = new Dictionary<string, HashSet<string>>();
        private Dictionary<string, WeakReference> m_assetNameToWeakPtr = new Dictionary<string,WeakReference>();


	    private Dictionary<string, int> m_bundleRefCountInfo = new Dictionary<string, int>();

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

            var table = bundle.LoadAsset<KTBundleNamesTable>("BundleNamesTable") as KTBundleNamesTable;//需要打包时设定别名为BundleNamesTable
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
                if (completeCallback != null)
                    completeCallback("");

                return;
            }

            var bundleLoader = new KTBundleLoader()
            {
                requestBundleName = bundleName,
                cacheType = BundleCacheType.Share,
                request = AssetBundle.LoadFromFileAsync(bundleName),
                completeCallback = completeCallback
            };

            m_bundleLoading.Add(bundleName, bundleLoader);
	        IncreaseRef(bundleName);
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
                if (completeCallback != null)
                    completeCallback("");

                return;
            }

            var requestBundleName = m_assetNameToBundleNameMap[assetName];
	        IncreaseRef(requestBundleName);

			if (string.IsNullOrEmpty(requestBundleName))
            {
                if (completeCallback != null)
                    completeCallback("");

                Debug.LogError("Asset Info " + assetName + " Not Found");
                return;
            }

            if (m_assetLoading.ContainsKey(assetName))//场景不允许请求重复
            {
                if (completeCallback != null)
                    completeCallback("");

                Debug.LogError("Request Scene " + assetName + " Repeated");
                return;
            }

            WeakReference weakPtr = null;
            if (m_assetNameToWeakPtr.TryGetValue(assetName, out weakPtr) && weakPtr.IsAlive)
            {
                if (completeCallback != null)
                    completeCallback(assetName);

                return;
            }

            //由update决定下一帧是读取bundle还是读取asset
            var assetLoader = new KTAssetLoader()
            {
                loaderType = KTAssetLoader.LoaderType.Scene,
                requestAssetName = assetName,
                requestBundleName = requestBundleName,
                cacheType = cacheType,
                cacheTimeout = cacheTimeout,
                dependentBundleNames = m_manifest.GetAllDependencies(requestBundleName).ToList(),
                sceneRequest = null,
                sceneCompleteCallback = completeCallback
            };
            m_assetLoading.Add(assetName, assetLoader);
        }

		/// <summary>
		/// 加载资源
		/// </summary>
		/// <param name="assetName"></param>
		/// <param name="completeCallback"></param>
		/// <param name="cacheType"></param>
		/// <param name="cacheTimeout"></param>
		public void LoadAsset(string assetName, Action<string, UnityEngine.Object> completeCallback = null, BundleCacheType cacheType = BundleCacheType.None, float cacheTimeout = 5)
        {
            if (!CheckInited())
            {
                if (completeCallback != null)
                    completeCallback("", null);

                return;
            }

            var requestBundleName = m_assetNameToBundleNameMap[assetName];
	        IncreaseRef(requestBundleName);

			if (string.IsNullOrEmpty(requestBundleName))
            {
                if (completeCallback != null)
                    completeCallback("", null);

                Debug.LogError("Asset Info " + assetName + " Not Found");
                return;
            }

            if (m_assetLoading.ContainsKey(assetName))//说明之前有同一个资源的请求
            {
                m_assetLoading[assetName].completeCallbackList.Add(completeCallback);
                return;
            }

            WeakReference weakPtr = null;
            if (m_assetNameToWeakPtr.TryGetValue(assetName, out weakPtr) && weakPtr.IsAlive)
            {
                if (completeCallback != null)
                    completeCallback(assetName,(UnityEngine.Object) weakPtr.Target);

                return;
            }

            //由update决定下一帧是读取bundle还是读取asset
            var assetLoader = new KTAssetLoader()
            {
                loaderType = KTAssetLoader.LoaderType.Asset,
                requestAssetName = assetName,
                requestBundleName = requestBundleName,
                cacheType = cacheType,
                cacheTimeout = cacheTimeout,
                dependentBundleNames = m_manifest.GetAllDependencies(requestBundleName).ToList(),
                request = null,
                completeCallbackList = new List<Action<string, UnityEngine.Object>>() { completeCallback }
            };
            m_assetLoading.Add(assetName, assetLoader);
        }

        /// <summary>
        /// 回收asset
        /// 回收资源会减去相关的引用计数（必须是BundleCacheType.TimeoutCache模式的资源）
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="obj">clone的资源</param>
        /// <returns></returns>
        public void RecycleAsset(string assetName)
        {
            if (!CheckInited())
                return;

            var requestBundleName = m_assetNameToBundleNameMap[assetName];
            if (string.IsNullOrEmpty(requestBundleName))
            {
                Debug.LogError("Asset Info " + assetName + " Not Found");
                return;
            }

			DecreaseRef(requestBundleName);
		}

		/// <summary>
		/// 运行时使用
		/// </summary>
		/// <param name="sceneName"></param>
		/// <returns></returns>
        public IEnumerator RecycleScene(string sceneName)
        {
            if (!CheckInited())
                yield break;

            var requestBundleName = m_assetNameToBundleNameMap[sceneName];
            if (string.IsNullOrEmpty(requestBundleName))
            {
                Debug.LogError("Asset Info " + sceneName + " Not Found");
                yield break;
            }

            yield return SceneManager.UnloadSceneAsync(sceneName);
	        DecreaseRef(requestBundleName);
		}

        /// <summary>
        /// 强制卸载bundle，必须等待所有下载操作完成，并且没有等待中的assetLoader依赖它，才可以这样操作
        /// 一般是在统一销毁资源时使用，运行时只会RecycleAsset
        /// 即使有等待的请求
        /// </summary>
        /// <param name="bundleName"></param>
        public IEnumerator RemoveBundle(string bundleName)
        {
	        if (!CheckInited())
		        yield break;

			while (!IsBundleAvailable(bundleName))
            {
                yield return null;
            }

	        while (HasAssetWaitingBundle(bundleName))
	        {
		        yield return null;
	        }

	        DecreaseRef(bundleName);
			if (m_bundleLoaded.ContainsKey(bundleName))
            {
                m_bundleLoaded[bundleName].Release(true);
                m_bundleLoaded.Remove(bundleName);
            }
        }

        /// <summary>
        /// 卸载所有bundle，清空所有下载
        /// </summary>
        /// <returns></returns>
        public IEnumerator Clear()
        {
            yield return WaitAllAssetLoaded();
            m_bundleLoaded.Values.ToList().ForEach(bundleInfo => bundleInfo.Release(true));
            m_bundleLoaded.Clear();
	        m_bundleRefCountInfo.Clear();
			m_bundleLoading.Clear();
            m_assetLoading.Clear();
            m_assetsWaitingBundle.Clear();
            m_assetNameToWeakPtr.Clear();
            yield return Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        /// <summary>
        /// 停止所有加载，如果有尚未完成的
        /// </summary>
        public IEnumerator Stop()
        {
            yield return WaitAllAssetLoaded();
            m_bundleLoading.Clear();
            m_assetLoading.Clear();
            m_assetsWaitingBundle.Clear();
            yield return Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public bool IsLoading { get { return m_bundleLoading.Count > 0 || m_assetLoading.Count > 0 || m_assetsWaitingBundle.Count > 0; } }

        /// <summary>
        /// 获取asset加载进度，包括相关依赖bundle的加载
        /// </summary>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public float GetAssetProgress(string assetName)
        {
            var requestBundleName = m_assetNameToBundleNameMap[assetName];
            var dependentBundleNames = m_manifest.GetAllDependencies(requestBundleName).ToList();
            dependentBundleNames.Add(requestBundleName);

            float total = dependentBundleNames.Count + 1;//加上asset的读取进度
            float progress = 0;
            dependentBundleNames.ForEach(bundleName =>
            {
                if (m_bundleLoading.ContainsKey(bundleName))
                    progress += m_bundleLoading[bundleName].request.progress;

                if (m_bundleLoaded.ContainsKey(bundleName))
                    progress += 1;
            });

            if (m_assetLoading.ContainsKey(assetName))
            {
                if (m_assetLoading[assetName].request != null)
                    progress += m_assetLoading[assetName].request.progress;

                if (m_assetLoading[assetName].sceneRequest != null)
                    progress += m_assetLoading[assetName].sceneRequest.progress;
            }

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

        private void Update()
        {
	        UpdateAssetLoader();
			UpdateBundleLoader();
            UpdateBundleInfo();
            UpdateWeakPtr();
        }

        /// <summary>
        /// 处理m_bundleLoading中加载完成的bundle加入到m_bundleLoaded里
        /// </summary>
        private void UpdateBundleLoader()
        {
            var namesToDeleteBundle = new List<string>();
            foreach (var item in m_bundleLoading)
            {
                if (item.Value.request.isDone)
                {
                    KTBundleInfo bundleInfo = new KTBundleInfo()
                    {
                        name = item.Key,
                        cacheType = item.Value.cacheType,//凡是新请求的bundle cacheType都延续第一次请求时的cacheType，此操作并不会覆盖已存在的依赖bundle的类型
                        cacheTimeout = item.Value.cacheTimeout,
                        bundle = item.Value.request.assetBundle
                    };

                    m_bundleLoaded.Add(item.Key, bundleInfo);

                    if (item.Value.completeCallback != null)
                        item.Value.completeCallback(item.Key);

                    namesToDeleteBundle.Add(item.Key);
                }
            }

            namesToDeleteBundle.ForEach(name => m_bundleLoading.Remove(name));
        }

        /// <summary>
        /// 处理m_assetLoading中加载完成的asset回调资源或尚未加载的bundle加入到m_bundleLoading中
        /// </summary>
        private void UpdateAssetLoader()
        {
            var namesToDeleteAsset = new List<string>();
            foreach (var item in m_assetLoading)
            {
                if (item.Value.loaderType == KTAssetLoader.LoaderType.Asset)
                {
                    if (item.Value.request == null)//尚未请求资源
                    {
                        var allBundleLoaded = IsBundleAvailable(item.Value.requestBundleName);
                        if (allBundleLoaded)
                        {
                            var bundleInfo = m_bundleLoaded[item.Value.requestBundleName];
                            item.Value.request = bundleInfo.bundle.LoadAssetAsync(item.Value.requestAssetName);
                            item.Value.isAllBundleLoaded = allBundleLoaded;
                        }
                        else
                        {
                            if (item.Value.isAllBundleLoading)
                                continue;

                            //把没在loading中的KTBundleLoader加入到loading中
                            var toLoadBundles = item.Value.dependentBundleNames.Where(name =>
                            {
                                return !m_bundleLoaded.ContainsKey(name) && !m_bundleLoading.ContainsKey(name);//保证不会被正在加载的bundleLoader覆盖cacheType
                            });

                            toLoadBundles.ToList().ForEach(name =>
                            {
                                var bundleLoader = new KTBundleLoader()
                                {
                                    requestBundleName = item.Value.requestBundleName,
                                    cacheType = item.Value.cacheType,
                                    cacheTimeout = item.Value.cacheTimeout,
                                    request = AssetBundle.LoadFromFileAsync(item.Value.requestBundleName)
                                };

                                m_bundleLoading.Add(item.Value.requestBundleName, bundleLoader);
	                            AddAssetWaitingBundle(item.Key, item.Value.requestBundleName);
                            });

                            item.Value.isAllBundleLoading = true;
                            continue;
                        }
                    }

                    if (item.Value.request.isDone)
                    {
                        //相关依赖包增加引用计数
                        item.Value.dependentBundleNames.ForEach(bundleName =>
                        {
                            m_bundleLoaded[bundleName].refCount++;
                        });

                        m_assetNameToWeakPtr.Add(item.Key, new WeakReference(item.Value.request.asset));

                        //回调所有请求
                        item.Value.completeCallbackList.ForEach(callback =>
                        {
                            callback(item.Key, item.Value.request.asset);
                            namesToDeleteAsset.Add(item.Key);
                        });

                        //根据情况销毁已加载的请求的bundle,不过如果存在不同的资源都使用了同一个bundle的情况，要在所有asset请求返回后再删除bundle,所以，需要延后处理
                    }
                }
                else if (item.Value.loaderType == KTAssetLoader.LoaderType.Scene)
                {
                    if (item.Value.sceneRequest == null)//尚未请求资源
                    {
						var allBundleLoaded = IsBundleAvailable(item.Value.requestBundleName);
						if (allBundleLoaded)
                        {
                            var bundleInfo = m_bundleLoaded[item.Value.requestBundleName];
                            item.Value.sceneRequest = SceneManager.LoadSceneAsync(item.Value.requestAssetName, item.Value.loadMode);
                            item.Value.isAllBundleLoaded = allBundleLoaded;
                        }
                        else
                        {
                            if (item.Value.isAllBundleLoading)
                                continue;

                            //把没在loading中的KTBundleLoader加入到loading中
                            var toLoadBundles = item.Value.dependentBundleNames.Where(name =>
                            {
                                return !m_bundleLoaded.ContainsKey(name) && !m_bundleLoading.ContainsKey(name);
                            });

                            toLoadBundles.ToList().ForEach(name =>
                            {
                                var bundleLoader = new KTBundleLoader()
                                {
                                    requestBundleName = item.Value.requestBundleName,
                                    cacheType = item.Value.cacheType,
                                    cacheTimeout = item.Value.cacheTimeout,
                                    request = AssetBundle.LoadFromFileAsync(item.Value.requestBundleName)
                                };

                                m_bundleLoading.Add(item.Value.requestBundleName, bundleLoader);
	                            AddAssetWaitingBundle(item.Key, item.Value.requestBundleName);//不同场景可能会同时请求一个bundle，所以此处与加载其他asset一样
                            });

                            item.Value.isAllBundleLoading = true;
                            continue;
                        }
                    }

                    if (item.Value.sceneRequest.isDone)
                    {
                        //相关依赖包增加引用计数
                        item.Value.dependentBundleNames.ForEach(bundleName =>
                        {
                            m_bundleLoaded[bundleName].refCount++;
                        });

                        m_assetNameToWeakPtr.Add(item.Key, new WeakReference(item.Value.request.asset));

                        //回调所有请求
                        item.Value.sceneCompleteCallback(item.Key);
                        namesToDeleteAsset.Add(item.Key);
                        //根据情况销毁已加载的请求的bundle,不过如果存在不同的资源都使用了同一个bundle的情况，要在所有asset请求返回后再删除bundle,所以，需要延后处理
                    }
                }
            }

            //删除所有请求
            namesToDeleteAsset.ForEach(name =>
            {
                if (m_assetLoading.ContainsKey(name))
                {
                    //清除掉所有等待bundleLoader的assetLoader关系信息
                    var assetLoader = m_assetLoading[name];
                    assetLoader.dependentBundleNames.ForEach(bundleName => DelAssetWaitingBundle(bundleName));
                    m_assetLoading[name].Dispose();
                    m_assetLoading.Remove(name);
                }
            });
        }

        /// <summary>
        /// 管理KTBundleInfo生存期
        /// </summary>
        private void UpdateBundleInfo()
        {
            var namesToDeleteBundleInfo = new List<string>();
            foreach (var item in m_bundleLoaded)
            {
                if (item.Value.cacheType == BundleCacheType.TimeoutCache)
                {
                    if (item.Value.startTime > 0 &&Time.realtimeSinceStartup - item.Value.startTime >= item.Value.cacheTimeout)
                    {
						//使用item.Value.startTime > 0潜规则表示引用计数为0不直观
						if (!HasAssetWaitingBundle(item.Key))
                        {
                            item.Value.Release(true);//一般是缓存资源，没被任何人使用和等待
                            namesToDeleteBundleInfo.Add(item.Key);
                        }
                    }
                }
                else if (item.Value.cacheType == BundleCacheType.None)
                {
                    if (!HasAssetWaitingBundle(item.Key))
                    {
                        item.Value.Release(false); //这种一般是加载完就不需要缓存bundle的资源,但在上一帧已经暂时缓存在m_bundleLoaded内
                        namesToDeleteBundleInfo.Add(item.Key);
                    }
                }
            }

            namesToDeleteBundleInfo.ForEach(name => m_bundleLoaded.Remove(name));
        }

        private void UpdateWeakPtr()
        {
            m_assetNameToWeakPtr.ToList()
            .Where(item => !item.Value.IsAlive)
            .ToList()
            .ForEach(item => m_assetNameToWeakPtr.Remove(item.Key));
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

	    private void AddAssetWaitingBundle(string assetName,string bundleName)
	    {
			if (!m_assetsWaitingBundle.ContainsKey(bundleName))
				m_assetsWaitingBundle.Add(bundleName, new HashSet<string>() { assetName });
			else
				m_assetsWaitingBundle[bundleName].Add(assetName);
		}

	    private void DelAssetWaitingBundle(string bundleName)
	    {
		    HashSet<string> set = null;
		    if (m_assetsWaitingBundle.TryGetValue(bundleName, out set))
		    {
			    if (set.Contains(name))
				    set.Remove(name);

			    if (set.Count == 0)
			    {
				    m_assetsWaitingBundle[bundleName] = null;
				    m_assetsWaitingBundle.Remove(bundleName);
			    }
		    }
	    }

	    /// <summary>
	    /// 检查bundle是否被某个assetLoader等待
	    /// </summary>
	    /// <param name="bundleName"></param>
	    /// <returns></returns>
	    private bool HasAssetWaitingBundle(string bundleName)
	    {
		    return m_assetsWaitingBundle.ContainsKey(bundleName) && m_assetsWaitingBundle[bundleName].Count > 0;
	    }

		private IEnumerator WaitAllAssetLoaded()
        {
            while (m_bundleLoading.Count > 0 || m_assetLoading.Count > 0 || m_assetsWaitingBundle.Count > 0)
            {
                yield return null;
            }
        }

		private bool IsBundleAvailable(string bundleName)
	    {
		    var dependentBundleNames = m_manifest.GetAllDependencies(bundleName);
		    for (int i = 0, j = dependentBundleNames.Length; i < j; ++i)
		    {
			    var depName = dependentBundleNames[i];
			    if (!m_bundleLoaded.ContainsKey(depName))
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

		    var dependentBundleNames = m_manifest.GetAllDependencies(bundleName);
		    for (int i = 0, j = dependentBundleNames.Length; i < j; ++i)
		    {
			    var depName = dependentBundleNames[i];
			    if (m_bundleRefCountInfo.ContainsKey(depName))
				    m_bundleRefCountInfo[depName]++;
			    else
				    m_bundleRefCountInfo.Add(depName, 1);
		    }
	    }

	    private void DecreaseRef(string bundleName)
	    {
		    if (m_bundleRefCountInfo.ContainsKey(bundleName))
		    {
			    if (--m_bundleRefCountInfo[bundleName] == 0)
				    m_bundleRefCountInfo.Remove(bundleName);
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
		    }
	    }


		public void Dispose()
        {
            StartCoroutine(Clear());
        }
	}
}

