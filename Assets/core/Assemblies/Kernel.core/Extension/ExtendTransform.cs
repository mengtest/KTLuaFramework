using System.Collections.Generic;
using UnityEngine;

namespace Kernel.core
{
	public static class ExtendTransform
	{
		public static Transform DeepFindEx(this Transform startNode, string name)
		{
			if(null != startNode && null != name)
			{
				var cachedNames = new Dictionary<int, string>();

				var rootName = _GetNodeName(startNode, cachedNames);
				if(rootName == name)
				{
					return startNode;
				}

				var found = _DFS_DeepFindEx(startNode, name, startNode.childCount, cachedNames);
				if(null != found)
				{
					return found;
				}
			}

			return null;
		}

		public static IEnumerable<Transform> ListDirectChildrenEx(this Transform tr)
		{
			var l = tr.childCount;
			for(var i = 0; i < l; i++)
			{
				yield return tr.GetChild(i);
			}
		}

		public static void ResetLocal(this GameObject go)
		{
			if(go != null)
			{
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;
			}
		}

		public static void ResetLocal(this Transform trans)
		{
			if(trans != null)
			{
				trans.localPosition = Vector3.zero;
				trans.localRotation = Quaternion.identity;
				trans.localScale = Vector3.one;
			}
		}

		public static void SetParent(this Transform child, Transform parent, bool peserveLocal = false)
		{
			if(child)
			{
				if(peserveLocal)
				{
					var originalScale = child.localScale;
					var originalRotation = child.localRotation;
					var originalPosition = child.localPosition;
					child.parent = parent;
					child.localScale = originalScale;
					child.localRotation = originalRotation;
					child.localPosition = originalPosition;
				}
				else
				{
					child.parent = parent;
					child.localPosition = Vector3.zero;
					child.localRotation = Quaternion.identity;
					child.localScale = Vector3.one;
				}
			}
		}

		public static void SetParentExAndResetLocal(this Transform target, Transform parent)
		{
			if(target == null || parent == null)
			{
				return;
			}
			target.parent = parent;
			target.ResetLocal();
		}

		public static void SetParentExAndResetLocal(this Transform target, GameObject parent)
		{
			if(target == null || parent == null)
			{
				return;
			}
			target.parent = parent.transform;
			target.ResetLocal();
		}

		public static void SetLocalPositionX(this Transform trans, float x)
		{
			if(trans)
			{
				var pos = trans.localPosition;
				pos.x = x;
				trans.localPosition = pos;
			}
		}

		public static void SetLocalPositionY(this Transform trans, float y)
		{
			if(trans)
			{
				var pos = trans.localPosition;
				pos.y = y;
				trans.localPosition = pos;
			}
		}

		public static void SetLocalPositionZ(this Transform trans, float z)
		{
			if(trans)
			{
				var pos = trans.localPosition;
				pos.z = z;
				trans.localPosition = pos;
			}
		}

		private static Transform _DFS_DeepFindEx(Transform root, string name, int childCount,
			Dictionary<int, string> cachedNames)
		{
			for(var i = 0; i < childCount; ++i)
			{
				var child = root.GetChild(i);
				var childName = _GetNodeName(child, cachedNames);

				if(childName == name)
				{
					return child;
				}

				var grandsonCount = child.childCount;
				if(grandsonCount > 0)
				{
					var found = _DFS_DeepFindEx(child, name, grandsonCount, cachedNames);
					if(null != found)
					{
						return found;
					}
				}
			}

			return null;
		}

		private static string _GetNodeName(Transform node, Dictionary<int, string> cachedNames)
		{
			var idNode = node.GetInstanceID();
			var nodeName = cachedNames.GetEx(idNode);

			if(null == nodeName)
			{
				nodeName = node.name;
				cachedNames.Add(idNode, nodeName);
			}

			return nodeName;
		}


		public static void AttachChild(this Transform parent, Transform child)
		{
			AttachChild(parent, child, Vector3.zero);
		}

		public static void AttachChild(this Transform parent, Transform child, Vector3 position)
		{
			AttachChild(parent, child, position, Quaternion.identity);
		}

		public static void AttachChild(this Transform parent, Transform child, Vector3 position, Quaternion rotation)
		{
			if(parent != null)
			{
				if(child != null)
				{
					child.parent = parent;
					child.localPosition = position;
					child.localRotation = rotation;
					child.localScale = Vector3.one;
				}
			}
		}

		public static void DetachChild(this Transform parent, Transform child)
		{
			if(child != null)
			{
				child.parent = null;
			}
		}

        public static string GetTransformFullPath(this Transform trans, Transform parent = null)
        {
            string path = trans.name;
            while (trans.parent != null && trans.parent != parent)
            {
                trans = trans.parent;
                path = trans.name + "/" + path;
            }
            return path;
        }
    }
}