using UnityEngine;
using Logger = Kernel.Log.Logger;

namespace Kernel.core
{
	public static class ExtendGameObject
	{
		public static GameObject FindGameObjectInChildren(this GameObject owner, string name, bool onlyChildren = false)
		{
			if(!onlyChildren && owner.name == name)
				return owner;

			var t = owner.transform;

			for(var i = 0; i < t.childCount; ++i)
			{
				var child = t.GetChild(i);
				if(child.name == name)
				{
					return child.gameObject;
				}
				if(child.childCount > 0)
				{
					var go = FindGameObjectInChildren(child.gameObject, name);
					if(go != null)
						return go;
				}
			}
			return null;
		}

		public static void SetLayer(this GameObject go, int layer)
		{
			if(go != null)
			{
				go.layer = layer;

				var t = go.transform;

				for(int i = 0, imax = t.childCount; i < imax; ++i)
				{
					var child = t.GetChild(i);
					SetLayer(child.gameObject, layer);
				}
			}
		}

		public static GameObject CloneEx(this GameObject go, bool reserveName = false)
		{
			if(null != go)
			{
				var cloned = Object.Instantiate(go);
				if(reserveName)
				{
					cloned.name = go.name;
				}

				return cloned;
			}

			return null;
		}

		public static bool GetActiveInHierarchyEx(this GameObject go)
		{
			if(null != go)
			{
				return go.activeInHierarchy;
			}
			Logger.Fatal("[GameObject:GetActiveInHierarchyEx()] go is null");
			return false;
		}

		public static bool GetActiveSelfEx(this GameObject go)
		{
			if(null != go)
			{
				return go.activeSelf;
			}
			Logger.Fatal("[GameObject:GetActiveSelfEx()] go is null");
			return false;
		}

		public static T SetDefaultComponentEx<T>(this GameObject go) where T : Component
		{
			if(null != go)
			{
				var component = go.GetComponent<T>();

				if(null == component)
				{
					component = go.AddComponent<T>();
				}

				return component;
			}

			return null;
		}

		public static void SetParentExAndResetLocal(this GameObject target, Transform parent)
		{
			if(target == null)
			{
				return;
			}
			target.transform.parent = parent;
			target.ResetLocal();
		}

		public static void SetParentExAndResetLocal(this GameObject target, GameObject parent)
		{
			if(target == null || parent == null)
			{
				return;
			}
			target.transform.parent = parent.transform;
			target.ResetLocal();
		}

		public static void SetParent(this GameObject go, Transform parent, bool preserveLocalTransform = false)
		{
			if(go == null)
			{
				return;
			}

			var trans = go.transform;
			if(preserveLocalTransform)
			{
				var localScale = trans.localScale;
				var localPosition = trans.localPosition;
				var localRotation = trans.localRotation;

				trans.parent = parent;
				trans.localScale = localScale;
				trans.localRotation = localRotation;
				trans.localPosition = localPosition;
			}
			else
			{
				trans.parent = parent;
				trans.localScale = Vector3.one;
				trans.localPosition = Vector3.zero;
				trans.localRotation = Quaternion.identity;
			}
		}

		public static void SetLocalPosition(this GameObject obj, Vector3 pos)
		{
			if(obj != null)
			{
				obj.transform.localPosition = pos;
			}
		}

		public static void SetLocalScale(this GameObject obj, Vector3 scale)
		{
			if(obj != null)
			{
				obj.transform.localScale = scale;
			}
		}

        public static bool GetIsPrefab(this GameObject obj)
        {
            return obj?.scene.rootCount == 0;
        }
	}
}