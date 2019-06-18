using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Logger=Kernel.Log;

namespace Kernel.core
{
    public class PrefabPool : IDisposable
    {
        private static readonly Transform root;
        public readonly bool IsTemporaryPool;
        private readonly string assetName;
        private readonly string resourceKey;
        private bool disposed;
        private Action<PrefabPool> onLoaded;
        private Queue<GameObject> queue;
        private Object loadedAsset;

        static PrefabPool()
        {
            root = new GameObject("PrefabPool").transform;
        }

        public PrefabPool(string assetName, bool isTempPool = false)
        {
            resourceKey = assetName;
            this.assetName = assetName;
            IsTemporaryPool = isTempPool;
            SetLastUsedTime();
        }

        public bool IsLoaded
        {
            get;
            private set;
        }

        public float LastUsedTime
        {
            get;
            private set;
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public override string ToString()
        {
            return string.Format("[PrefabPool: resourceKey= {0}]", resourceKey);
        }

        public bool CanDestory()
        {
            if (IsLoaded && onLoaded == null)
            {
                return true;
            }
            return false;
        }

        public void ClearQueue()
        {
            if (queue != null && queue.Count > 0)
            {
                foreach (var go in queue)
                {
                    go.DestroyEx();
                }
                queue.Clear();
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                onLoaded = null;
                ClearQueue();
				KTBundleResourceManager2.Instance.RecycleAsset(assetName);
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public string GetResourceKey()
        {
            return resourceKey;
        }

        public void Load(Action<PrefabPool> callback)
        {
            if (disposed)
            {
				Logger.Logger.Error("Load failed:{0} Pool has been disposed.", assetName);
            }
            if (IsLoaded)
            {
				Logger.Logger.Error("prefabPool has been loaded with {0}, reload is forbided.", assetName);
            }

			KTBundleResourceManager2.Instance.LoadAsset(assetName, OnMainAssetLoad);

			if (callback != null)
            {
                if (IsLoaded)
                {
                    callback(this);
                }
                else
                {
                    onLoaded += callback;
                }
            }
        }

        /// <summary>
        ///     预加载count数目的对象到内存
        ///     重复调用该方法以最大的count为准（重复进入同一副本时不会重复加载)
        /// </summary>
        public void Preload(int count)
        {
            var tobePreloadCount = count - (queue == null ? 0 : queue.Count);

            for (var i = 0; i < tobePreloadCount; ++i)
            {
                var cloned = CloneOne();
                if (cloned != null)
                {
                    Recycle(cloned);
                }
            }
            SetLastUsedTime();
        }

        public void Recycle(GameObject go)
        {
            if (disposed)
            {
				Logger.Logger.Error(string.Format("Recycle failed:{0} Pool has been disposed.", assetName));
            }
            if (go != null)
            {
				if (queue == null)
				{
					queue = new Queue<GameObject>(8);
				}
				AddToInstanceRoot(go);
                go.SetActive(false);
                queue.Enqueue(go);
            }
        }

        public GameObject Spawn(bool isActivate = true)
        {
            if (disposed || !IsLoaded)
            {
				Logger.Logger.Warn(string.Format("Spawn failed:{0} Pool has been disposed or is loading.", assetName));
                return null;
            }
            GameObject go;
            if (queue != null && queue.Count > 0)
            {
                go = queue.Dequeue();
            }
            else
            {
                go = CloneOne();
            }

            if (go != null)
            {
                go.transform.parent = null;
                if (go.activeSelf != isActivate)
                {
                    go.SetActive(isActivate);
                }
                SetLastUsedTime();
            }
            return go;
        }

        private static void AddToInstanceRoot(GameObject o)
        {
            o.transform.parent = root;
        }

        private GameObject CloneOne()
        {
            if (loadedAsset == null)
            {
				Logger.Logger.Error("loadedAsset资源为空：{0}", assetName);
                return null;
            }

            var cloned = GameObject.Instantiate(loadedAsset) as GameObject;

            if (cloned != null)
            {
                cloned.name = string.Format("{0}[{1:x8}]", cloned.name, cloned.GetHashCode());
            }
            else
            {
				Logger.Logger.Warn("疑似不存在的资源：{0}", assetName);
            }
            return cloned;
        }

        private void OnMainAssetLoad(string assetName,Object asset)
        {
            if (disposed)
            {
                return;
            }
            IsLoaded = true;
            loadedAsset = asset;
            if (onLoaded != null)
            {
                onLoaded(this);
                onLoaded = null;
            }
        }

        private void SetLastUsedTime()
        {
            LastUsedTime = Time.time;
        }

#if UNITY_EDITOR
   //     ~PrefabPool()
   //     {
			//Logger.Logger.Warn("PrefabPool not disposed:{0}", resourceKey);
   //         DisposableRecycler.Add(this);
   //     }
#endif
    }
}