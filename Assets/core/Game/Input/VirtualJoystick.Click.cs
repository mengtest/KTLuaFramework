using UnityEngine;

namespace Kernel.Game
{
	public static partial class VirtualJoystick
	{
		private enum ClickEvent
		{
			NONE = 0,
			BEGAN,
			HOLD,
			ENDED
		}

		private class Click
		{
			public ClickEvent Event = ClickEvent.NONE;
			public Vector2 BeginPosition;
			public Vector2 Position;
			public Vector2 LastPosition;
			public int FingerId = -1;
			public bool IsOverUI = false;

			public void Clear()
			{
				Event = ClickEvent.NONE;
				BeginPosition = Vector2.zero;
				Position = Vector2.zero;
				LastPosition = Vector2.zero;
				FingerId = -1;
				IsOverUI = false;
			}
		}

		private static readonly Click touchClick = new Click();

		public static void ResetClick()
		{
			touchClick.Clear();
		}

		private static void TickForClick()
		{
			int touchCount = UnityEngine.Input.touchCount;
			bool trigger = false;
			if(touchCount == 0)
			{
				// 没有触摸，看点击
				Vector3 p = UnityEngine.Input.mousePosition;
				if(UnityEngine.Input.GetMouseButtonDown(0))
				{
					touchClick.Event = ClickEvent.BEGAN;
					touchClick.BeginPosition = p;
					touchClick.Position = p;
					touchClick.LastPosition = p;
					touchClick.FingerId = -1;
					touchClick.IsOverUI = IsTouchOverOtherUI(-1);
					trigger = true;
				}
				else if(UnityEngine.Input.GetMouseButton(0))
				{
					touchClick.Event = ClickEvent.HOLD;
					touchClick.LastPosition = touchClick.Position;
					touchClick.Position = p;
					trigger = true;
				}
				else if(UnityEngine.Input.GetMouseButtonUp(0))
				{
					touchClick.Event = ClickEvent.ENDED;
					touchClick.LastPosition = touchClick.Position;
					touchClick.Position = p;
					trigger = true;
				}
			}
			else
			{
				// 触摸
				for(var i = 0; i < touchCount; i++)
				{
					Touch touch = UnityEngine.Input.GetTouch(i);
					// 第一个不是摇杆的触摸按键
					if(!leftAxisHold || leftAxisFingerId != touch.fingerId)
					{
						// 只有初始化和结束时才触发
						if(touchClick.Event == ClickEvent.NONE || touchClick.FingerId == touch.fingerId)
						{
							if(touch.phase == TouchPhase.Began)
							{
								touchClick.Event = ClickEvent.BEGAN;
								touchClick.BeginPosition = touch.position;
								touchClick.Position = touchClick.BeginPosition;
								touchClick.LastPosition = touchClick.Position;
								touchClick.FingerId = touch.fingerId;
								touchClick.IsOverUI = IsTouchOverOtherUI(i);
							}
							else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
							{
								touchClick.Event = ClickEvent.HOLD;
								touchClick.LastPosition = touchClick.Position;
								touchClick.Position = touch.position;
							}
							else if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
							{
								touchClick.Event = ClickEvent.ENDED;
								touchClick.LastPosition = touchClick.Position;
								touchClick.Position = touch.position;
							}
							trigger = true;
						}
					}
				}
			}

			if(!trigger)
			{
				touchClick.Event = ClickEvent.NONE;
				touchClick.BeginPosition = Vector2.zero;
				touchClick.LastPosition = touchClick.Position;
				touchClick.Position = Vector2.zero;
				touchClick.FingerId = -1;
				touchClick.IsOverUI = false;
			}
		}

		public static bool IsTouchBegan()
		{
			return touchClick.Event == ClickEvent.BEGAN && !touchClick.IsOverUI;
		}

		public static bool IsTouchEnded()
		{
			return touchClick.Event == ClickEvent.ENDED && !touchClick.IsOverUI;
		}

		public static bool IsTouchHold()
		{
			return touchClick.Event == ClickEvent.HOLD && !touchClick.IsOverUI;
		}

		public static Vector2 GetTouchBeginPosition()
		{
			return touchClick.BeginPosition;
		}

		public static Vector2 GetTouchPosition()
		{
			return touchClick.Position;
		}

		public static Vector2 GetTouchLastPosition()
		{
			return touchClick.LastPosition;
		}
	}
}