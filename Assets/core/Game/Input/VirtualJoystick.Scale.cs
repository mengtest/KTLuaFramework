using UnityEngine;
using UnityEngine.EventSystems;

namespace Kernel.Game
{
	public static partial class VirtualJoystick
	{
		private static float oldDistance = 0;

		private static bool IsOverUI()
		{
			if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
			{
#if IPHONE || ANDROID
				if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
#else
				if (EventSystem.current.IsPointerOverGameObject())
#endif
					return true;
			}

			return false;
		}

		public static float GetScaleValue()
		{
			if (IsOverUI())
				return 0;

			if (UnityEngine.Input.touchCount == 2)
			{
				Touch touch1 = UnityEngine.Input.GetTouch(0);
				Touch touch2 = UnityEngine.Input.GetTouch(1);
				if(!IsTouchOverOtherUI(0) && !IsTouchOverOtherUI(1))
				{
					if(!leftAxisHold || (touch1.fingerId != leftAxisFingerId && touch2.fingerId != leftAxisFingerId))
					{
						if(oldDistance <= 0)
						{
							oldDistance = (UnityEngine.Input.GetTouch(0).position - UnityEngine.Input.GetTouch(1).position).magnitude;
						}
						if(UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved || UnityEngine.Input.GetTouch(1).phase == TouchPhase.Moved)
						{
							var newDis = (UnityEngine.Input.GetTouch(0).position - UnityEngine.Input.GetTouch(1).position).magnitude;
							var ret = newDis - oldDistance;
							oldDistance = newDis;
							return ret * 0.01f;
						}
					}
				}
			}
			else
			{
				return oldDistance = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
			}
			return 0;
		}

		public static void ResetScale()
		{
			oldDistance = 0;
		}
	}
}