using System;
using UnityEngine;

namespace Kernel.core
{
	public class ResourceGameObject:ITick
	{
		protected GameObject resourceGameObject;
		private string name;
		private string originalName;

		private int originLayer;
		private bool resourceGameObjectActive = true;
		private GameObject rootGameObject;
		protected Transform transform;
		private bool usePool;
		private Action<ResourceGameObject> mLoadDone = null;
		public ResourceGameObject()
		{
			resourceGameObject = null;
			AssetPath = null;
			IsLoaded = false;
			IsLoading = false;
			rootGameObject = new GameObject(GetType().FullName);
			transform = rootGameObject.transform;
		}

		public bool IsDestroyed
		{
			get
			{
				return rootGameObject == null;
			}
		}

		public string Name
		{
			get
			{
				return !IsDestroyed ? rootGameObject.name : null;
			}
		}

		public string AssetPath
		{
			get;
			set;
		}

		public bool IsLoaded
		{
			get;
			private set;
		}

		public bool IsLoading
		{
			get;
			private set;
		}

		public static implicit operator GameObject(ResourceGameObject r)
		{
			if (r != null)
			{
				return r.rootGameObject;
			}
			return null;
		}

		public virtual void Destroy()
		{
			Unload();
			if (rootGameObject != null)
			{
				rootGameObject.DestroyEx();
				rootGameObject = null;
				transform = null;
			}
		}

		public virtual void Load(string path, bool usePool = true, Action<ResourceGameObject> LoadDone = null)
		{
			if (path.IsNullOrEmpty())
			{
				return;
			}
			if (!IsDestroyed && !IsLoading && !IsLoaded)
			{
				mLoadDone += LoadDone;
				this.usePool = usePool;
				IsLoading = true;
				AssetPath = path;
				if (usePool)
				{
					PrefabPoolManager.Instance.Spawn(path, OnLoadFinish);
				}
				else
				{
					KTBundleResourceManager2.Instance.LoadAsset(path, OnLoadFinish);
				}
			}
		}

		public virtual void Load(UnityEngine.Object go)
		{
			if (go == null)
			{
				return;
			}
			if (!IsDestroyed && !IsLoading && !IsLoaded)
			{
				usePool = false;
				IsLoading = true;
				AssetPath = null;
				OnLoadFinish(AssetPath, go);
			}
		}

		public virtual void SetParent(Transform parent, bool worldPositionStays = false)
		{
			if (!IsDestroyed)
			{
				transform.SetParent(parent, worldPositionStays);
				transform.localPosition = Vector3.zero;
				//transform.localRotation = Quaternion.identity;
				//transform.localScale = Vector3.one;
			}
		}

		public virtual void SetPosition(Vector3 position)
		{
			SetPosition(position, Space.World);
		}

		public virtual void Tick(float deltaTime)
		{
		}

		public virtual void Unload()
		{
			if (!IsDestroyed && IsLoaded)
			{
				if (resourceGameObject != null)
				{
					resourceGameObject.name = originalName;
					if (usePool)
					{
						PrefabPoolManager.Instance.Recycle(AssetPath, resourceGameObject);
					}
					else
					{
						//ResourceManager.Instance.Unload(Bundle);
						resourceGameObject.DestroyEx();
					}
					resourceGameObject = null;
					usePool = false;
					rootGameObject = new GameObject(GetType().FullName);
					transform = rootGameObject.transform;
					originLayer = 0;
					OnUnloaded();
				}
				resourceGameObjectActive = false;
				AssetPath = null;
				IsLoading = false;
				IsLoaded = false;
				originalName = string.Empty;
			}
		}

		public T GetComponent<T>() where T : Component
		{
			if (!IsDestroyed)
			{
				return rootGameObject.GetComponent<T>();
			}
			return null;
		}

		public T[] GetComponents<T>() where T : Component
		{
			if (!IsDestroyed)
			{
				return rootGameObject.GetComponents<T>();
			}
			return new T[0];
		}

		public T AddComponent<T>() where T : Component
		{
			if (!IsDestroyed)
			{
				return rootGameObject.AddComponent<T>();
			}
			return null;
		}

		public void AttachChild(ResourceGameObject child)
		{
			if (child != null && !child.IsDestroyed)
			{
				AttachChild(child.transform, Vector3.zero);
			}
		}

		public void AttachChild(ResourceGameObject child, Vector3 position)
		{
			if (child != null && !child.IsDestroyed)
			{
				AttachChild(child.transform, position);
			}
		}

		public void AttachChild(Transform child)
		{
			AttachChild(child, Vector3.zero);
		}

		public void AttachChild(Transform child, Vector3 position)
		{
			AttachChild(child, position, Quaternion.identity);
		}

		public void AttachChild(Transform child, Vector3 position, Quaternion rotation)
		{
			if (!IsDestroyed)
			{
				if (child != null)
				{
					child.parent = transform;
					child.localPosition = position;
					child.localRotation = rotation;
					child.localScale = Vector3.one;
				}
			}
		}

		public void DetachChild(ResourceGameObject child)
		{
			if (child != null && !child.IsDestroyed)
			{
				DetachChild(child.transform);
			}
		}

		public void DetachChild(Transform child)
		{
			if (child != null)
			{
				child.parent = null;
			}
		}

		public Transform GetChildTransform(string name)
		{
			return !IsDestroyed ? rootGameObject.transform.Find(name) : null;
		}

		public T GetComponentInChildren<T>(bool includeInactive = false) where T : Component
		{
			if (!IsDestroyed)
			{
				return rootGameObject.GetComponentInChildren<T>();
			}
			return null;
		}

		public Vector3 GetForward()
		{
			if (!IsDestroyed)
			{
				return transform.forward;
			}
			return Vector3.forward;
		}

		public int GetLayer()
		{
			return IsDestroyed ? 0 : rootGameObject.layer;
		}

		public Matrix4x4 GetLocalToWorldMatrix()
		{
			if (!IsDestroyed)
			{
				return transform.localToWorldMatrix;
			}
			return Matrix4x4.identity;
		}

		public Vector3 GetPosition()
		{
			return GetPosition(Space.World);
		}

		public Vector3 GetPosition(Space relativeTo)
		{
			if (!IsDestroyed)
			{
				return relativeTo == Space.World ? transform.position : transform.localPosition;
			}
			return Vector3.zero;
		}

		public Quaternion GetQuaternion(Space relateTo = Space.World)
		{
			return GetRotation(relateTo);
		}

		public Vector3 GetRight()
		{
			if (!IsDestroyed)
			{
				return transform.right;
			}
			return Vector3.right;
		}

		public Quaternion GetRotation()
		{
			return GetRotation(Space.World);
		}

		public Quaternion GetRotation(Space relativeTo)
		{
			if (!IsDestroyed)
			{
				return relativeTo == Space.World ? transform.rotation : transform.localRotation;
			}
			return Quaternion.identity;
		}

		public Vector3 GetScale(Space relativeTo = Space.Self)
		{
			if (!IsDestroyed)
			{
				return relativeTo == Space.World ? transform.lossyScale : transform.localScale;
			}
			return Vector3.one;
		}

		public Transform GetTransform()
		{
			return transform;
		}

		public Vector3 GetUp()
		{
			if (!IsDestroyed)
			{
				return transform.up;
			}
			return Vector3.up;
		}

		public bool HasChildren(GameObject go)
		{
			while (go != null)
			{
				if (go == rootGameObject)
				{
					return true;
				}
				go = go.transform.parent != null ? go.transform.parent.gameObject : null;
			}
			return false;
		}

		public bool IsActive()
		{
			if (!IsDestroyed)
			{
				return rootGameObject.activeSelf;
			}
			return false;
		}

		public bool IsResourceGameObjectActive()
		{
			return resourceGameObjectActive;
		}

		public void ResetLayer()
		{
			if (!IsDestroyed && rootGameObject.layer != originLayer)
			{
				rootGameObject.SetLayer(originLayer);
			}
		}

		public void SetActive(bool active)
		{
			//UnityEngine.Debug.LogError(Bundle+ " SetActive " + active);
			if (!IsDestroyed)
			{
				rootGameObject.SetActive(active);
			}
		}

		public void SetEulerAngles(Vector3 eulerAngles, Space relativeTo = Space.World)
		{
			if (!IsDestroyed)
			{
				if (relativeTo == Space.World)
				{
					transform.eulerAngles = eulerAngles;
				}
				else
				{
					transform.localEulerAngles = eulerAngles;
				}
			}
		}

		public void SetLayer(int layer)
		{
			if (!IsDestroyed && rootGameObject.layer != layer)
			{
				rootGameObject.SetLayer(layer);
			}
		}

		public void SetTag(string tag)
		{
			if (!IsDestroyed && !rootGameObject.CompareTag(tag))
			{
				rootGameObject.tag = tag;
			}
		}

		public void SetPosition(Vector3 position, Space relativeTo)
		{
			if (!IsDestroyed)
			{
				if (relativeTo == Space.World)
				{
					transform.position = position;
				}
				else
				{
					transform.localPosition = position;
				}
			}
		}

		public void SetResourceGameObjectActive(bool active)
		{
			resourceGameObjectActive = active;
			if (resourceGameObject != null)
			{
				resourceGameObject.SetActive(resourceGameObjectActive);
			}
		}

		// ReSharper disable once ParameterHidesMember
		public void SetRootName(string name)
		{
			this.name = name;
			if (!IsDestroyed && this.name != null)
			{
				rootGameObject.name = this.name;
			}
		}

		public void SetRotation(Vector3 dir)
		{
			if (dir.sqrMagnitude > 0.0f)
			{
				SetRotation(Quaternion.LookRotation(dir, Vector3.up));
			}
		}

		public void SetRotation(Vector3 dir, Vector3 up)
		{
			if (dir.sqrMagnitude > 0.0f && up.sqrMagnitude > 0.0f)
			{
				SetRotation(Quaternion.LookRotation(dir, up));
			}
		}

		public void SetRotation(Quaternion rotation, Space relativeTo = Space.World)
		{
			if (!IsDestroyed)
			{
				if (relativeTo == Space.World)
				{
					transform.rotation = rotation;
				}
				else
				{
					transform.localRotation = rotation;
				}
			}
		}

		public void SetRotationH(Vector3 dir)
		{
			dir.y = 0.0f;
			if (dir.sqrMagnitude > 0.0f)
			{
				dir.Normalize();
				SetRotation(Quaternion.LookRotation(dir, Vector3.up));
			}
		}

		public virtual void SetScale(float scale)
		{
			if (scale > 0.0f)
			{
				SetScale(Vector3.one * scale);
			}
		}

		public virtual void SetScale(Vector3 scale)
		{
			if (IsDestroyed)
			{
				return;
			}
			if (!(scale.x > 0.0f) || !(scale.y > 0.0f) || !(scale.z > 0.0f))
			{
				return;
			}
			transform.localScale = scale;
		}

		protected virtual void OnLoaded(GameObject go)
		{
		}

		protected virtual void OnUnloaded()
		{
		}

		private void OnLoadFinish(string bundle, UnityEngine.Object obj)
		{
			var go = (GameObject)obj;
			if (go == null)
			{
				OnLoaded(null);
				return;
			}

			if (!IsDestroyed && IsLoading && !IsLoaded)
			{
				IsLoading = false;
				go.transform.parent = transform.parent;
				go.transform.position = transform.position;
				go.transform.rotation = transform.rotation;
				go.transform.localScale = transform.localScale;
				originalName = go.name;
				if (rootGameObject != null)
				{
					rootGameObject.DestroyEx();
				}
				if (!go.activeSelf)
				{
					go.SetActive(true);
				}
				SetResourceGameObjectActive(IsResourceGameObjectActive());
				IsLoaded = true;
				resourceGameObject = go;
				originLayer = resourceGameObject.layer;
				transform = resourceGameObject.transform;
				rootGameObject = resourceGameObject;
				if (name != null)
				{
					rootGameObject.name = name;
				}
				OnLoaded(go);

				if (mLoadDone != null)
				{
					mLoadDone(this);
					mLoadDone = null;
				}
			}
			else
			{
				go.DestroyEx();
				OnLoaded(null);
			}
		}
	}
}