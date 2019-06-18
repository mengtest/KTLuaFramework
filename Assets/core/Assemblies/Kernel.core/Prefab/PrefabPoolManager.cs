using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.core
{
	//这个类之所以这样，基于以下考虑：
	//1、预加载（Preload）的东西一定会被使用，所以不希望被随意卸载掉.
	//2、没有预加载的东西，需要在恰当的时机清理一下，否则可能会耗尽内存.
	//由于PrefabPoolMan里的PrefabPool可能随时被清理，所以这里不提供任何能访问到PrefabPool的接口.
	//如果想要使用PrefabPool的功能，请自己创建和销毁.
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	//关于恰当的时机有两点考虑因素：
	//  i、当资源数量超标时，卸载最近没有被使用的资源，但目前看来在不同的使用场合中这个标准是不一样的，如果标准定的比较低，可能引起卡顿。
	//  ii、当资源一段时间没被使用后，卸载它，这个方法的问题是，如果在短时间内使用了大量资源，可能导致内存耗尽。
	public class PrefabPoolManager : Manager<PrefabPoolManager>
	{
		public bool CleanPreloadPool = false;
		public bool CleanWhenCreateTemporyPool = false;
		private readonly Dictionary<string, PrefabPool> pools = new Dictionary<string, PrefabPool>();
		private float lastCleanPoolTime;

		public PrefabPoolManager()
		{
			AutoCleanTime = 10f;
			//Runtime.Game.Instance.OnGameQuit += ForceClearAll;
		}

		public float AutoCleanTime
		{
			get;
			private set;
		}

		public GameObject CloneOnly(string name, GameObject asset, bool isActivate = true)
		{
			var cloned = asset.CloneEx(true);
			if(cloned != null)
			{
				cloned.name = string.Format("{0}[{1:x8}]", cloned.name, cloned.GetHashCode());
				cloned.transform.parent = null;
				cloned.SetActive(isActivate);
			}
			return cloned;
		}

		public void ForceClearAll()
		{
#if UNITY_EDITOR
			InPoolCounter.Clear();
			OutOfPoolBundles.Clear();
#endif
			while (pools.GetEnumerator().MoveNext())
			{
				var item = pools.GetEnumerator().Current;
				DestroyPool(item.Value);
			}
			pools.Clear();
		}

		public int GetPoolCount()
		{
			return pools.Count;
		}

		public void Preload(string assetName, int count)
		{
			if(assetName.IsNullOrEmpty())
			{
				return;
			}
			var pool = CreatePool(assetName);
			pool.Load(p => p.Preload(count));
		}

		public void PreloadLoadedBundle(string bundle, int count)
		{
			if(bundle.IsNullOrEmpty())
			{
				return;
			}
			var pool = CreatePool(bundle);
			if(!pool.IsLoaded)
			{
				pool.Load(p => p.Preload(count));
			}
			else
			{
				pool.Preload(count);
			}
		}

		public void Recycle(string assetName, GameObject go)
		{
			if(!string.IsNullOrEmpty(assetName))
			{
				var pool = pools[assetName];
				if(pool != null)
				{
					pool.Recycle(go);
					return;
				}
			}
			go.DestroyEx();
		}

		public void Preload(string assetName)
		{
			if(assetName.IsNullOrEmpty())
			{
				return;
			}
			if(CleanWhenCreateTemporyPool)
			{
				CheckAndDestoryPool(false);
			}
			var pool = CreatePool(assetName);
			if (!pool.IsLoaded)
				pool.Load(p => OnPreloaded(assetName,p.Spawn()));
		}

		public void Spawn(string assetName, Action<string,GameObject> callback,bool autoActive=true)
		{
			if(assetName.IsNullOrEmpty())
			{
				callback(string.Empty,null);
				return;
			}

			if(CleanWhenCreateTemporyPool)
			{
				CheckAndDestoryPool(false);
			}
			var pool = CreatePool(assetName);
			if(pool.IsLoaded)
			{
				callback(assetName, pool.Spawn(autoActive));
				return;
			}
			pool.Load(p => callback(assetName, p.Spawn()));
		}

        public bool InPool(string assetName)
        {
            return pools[assetName] != null;
        }

        public void TimeToCleanPool()
		{
			CheckAndDestoryPool(true);
		}

		public void Unload(string assetName)
		{
			if(pools.ContainsKey(assetName))
			{
				pools[assetName].Dispose();
				//pools[bundle].Dispose();
				pools.Remove(assetName);
			}
		}

		private static void DestroyPool(PrefabPool pool)
		{
			pool.Dispose();
		}

		private void OnPreloaded(string assetName,GameObject go)
		{
			Recycle(assetName, go);
		}

		private void CheckAndDestoryPool(bool cleanPreloadPool)
		{
			var curTime = Time.time;
			if(curTime - lastCleanPoolTime > 1)
			{
				lastCleanPoolTime = curTime;
			}
			else
			{
				return;
			}

			cleanPreloadPool &= CleanPreloadPool;
			using(var removeList = TempList<string>.Alloc())
			{
				while (pools.GetEnumerator().MoveNext())
				{
					var item = pools.GetEnumerator().Current;
					if (item.Value.IsTemporaryPool || cleanPreloadPool)
					{
						if (item.Value.CanDestory() && curTime - item.Value.LastUsedTime > AutoCleanTime)
						{
							removeList.Add(item.Key);
						}
					}
				}

				for(var i = 0; i < removeList.Count; ++i)
				{
#if UNITY_EDITOR
					InPoolCounter.Remove(removeList[i]);
					OutOfPoolBundles.Remove(removeList[i]);
#endif
					var pool = pools[removeList[i]];
					DestroyPool(pool);
					pools.Remove(removeList[i]);
				}
			}
		}

		private PrefabPool CreatePool(string assetName)
		{
			var pool = pools[assetName];
			if (pool == null)
			{
#if UNITY_EDITOR
				if(InPoolCounter.ContainsKey(assetName))
				{
					InPoolCounter[assetName]++;
				}
				else
				{
					InPoolCounter.Add(assetName, 1);
				}
#endif
				pool = new PrefabPool(assetName);
				pools.Add(assetName, pool);
			}
			return pool;
		}
#if UNITY_EDITOR
		//监视用的字段
		public static Dictionary<string, int> InPoolCounter = new Dictionary<string, int>();
		public static Dictionary<string, int> OutOfPoolBundles = new Dictionary<string, int>();
#endif
	}
}