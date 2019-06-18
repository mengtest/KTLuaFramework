using UnityEngine;

namespace Kernel.core
{
	public static class ExtendObject
	{
		public static void DestroyEx(this Object obj)
		{
			if(Application.isEditor)
			{
				Object.DestroyImmediate(obj);
			}
			else
			{
				Object.Destroy(obj);
			}
		}

		public static bool IsNullOrDestroied(this Object obj)
		{
			var state = obj == null;
			return state;
		}
	}
}