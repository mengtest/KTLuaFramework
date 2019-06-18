using Kernel.core;
using Kernel.Engine;
using UnityEngine;

namespace Kernel.Game
{
	//TODO 详细的注释，精简的代码
	public enum VirtualJoystickKey
	{
		X = 0,
		A,
		B,
		C,
		D,
		E,
		F,
		U1,
		U2,
		U3,
		S,
		COUNT
	}

	public struct VirtualJoystickSetting
	{
		public Vector2 Center; //可呼出区域中心点，屏幕比例0-1
		public Vector2 Size;//可呼出区域范围，屏幕比例0-1
		public Vector2 FollowBottomLeft; //底盘可跟随区域左下点，屏幕比例0-1
		public Vector2 FollowTopRight; //底盘可跟随区域右上点，屏幕比例0-1
		public float centerAxisRadius;//摇杆的中间球的半径
		public float outAxisRadius;//相对外边界的半径
		public float availableAxisRadius; //摇杆中间球中心可移动半径
		public Vector2 defaultAxisPoint;//默认摇杆球中心点，屏幕比例0-1
	}

	public static partial class VirtualJoystick
	{
		private static readonly bool[] keyPressed;
		private static readonly bool[] keyHold;
		private static readonly bool[] keyDown;
		private static readonly bool[] keyUp;
		private static readonly Counter[] keyTimer;

		private static bool axisEnabled;
		private static Vector2 leftAxisDirection;
		private static Vector2 leftAxisFingerDownPosition;
		private static int leftAxisFingerId = -1;
		private static bool leftAxisHold;
		private static readonly Counter leftAxisTimer = new Counter();
		private static AxisAlignRectangle2D region;//摇杆区域设置
		private static Vector2 axisPoint;
		private static float centerAxisRadius = 10;//摇杆的中间球的半径
		private static float outAxisRadius;//相对外边界的半径
		private static float availableAxisRadius = 53; //摇杆中间球中心可移动半径

		private static float startTime = 0;
        private static float endTime = 0;
        private static float holdTime = 0;

        //private static ConfFightConst confFightConst;
        static VirtualJoystick()
		{
			keyPressed = new bool[(int)VirtualJoystickKey.COUNT];
			keyHold = new bool[(int)VirtualJoystickKey.COUNT];
			keyDown = new bool[(int)VirtualJoystickKey.COUNT];
			keyUp = new bool[(int)VirtualJoystickKey.COUNT];
			keyTimer = new Counter[(int)VirtualJoystickKey.COUNT];
			for (var i = 0; i < (int)VirtualJoystickKey.COUNT; ++i)
			{
				keyTimer[i] = new Counter();
			}

			Init(new VirtualJoystickSetting());
		}

		public static void Init(VirtualJoystickSetting setting)
		{
			SetAxisRegion(setting.Center, setting.Size, setting.FollowBottomLeft, setting.FollowTopRight);
			SetAxisPoint(setting.defaultAxisPoint);
			Reset();
		}

		public static float PPI
		{
			get
			{
				return Screen.dpi.IsZero() ? 96f : Screen.dpi;
			}
		}

		/// <summary>
		/// 检查pos是否在摇杆中心点size的矩形范围内
		/// </summary>
		/// <param name="pos">是计算后的屏幕像素坐标</param>
		/// <returns></returns>
		private static bool CheckAxisRegion(Vector2 pos)
		{
			if (!axisEnabled)
			{
				return false;
			}
			return region.Inside(pos);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pos">是计算后的屏幕像素坐标</param>
		/// <returns></returns>
		private static Vector2 FixFollowPosition(Vector2 pos)
		{
			if (!axisEnabled)
			{
				return Vector2.zero;
			}
			return region.FixFollowPosition(pos, outAxisRadius);
		}

		public static void EnableAxisRegion(bool enable)
		{
			axisEnabled = enable;
			if (!axisEnabled)
			{
				ResetLeftAxis();
			}
		}

		/// <summary>
		///		摇杆区域设置
		/// </summary>
		/// <param name="center">可呼出区域中心点</param>
		/// <param name="size">可呼出区域范围</param>
		/// <param name="followBottomLeft">底盘可跟随区域坐下点</param>
		/// <param name="followTopRight">地盘可跟随区域右上点</param>
		public static void SetAxisRegion(Vector2 center, Vector2 size, Vector2 followBottomLeft, Vector2 followTopRight)
		{
			region.Center = new Vector2(Screen.width * center.x, Screen.height * center.y);
			region.HalfSize = new Vector2(Screen.width * size.x, Screen.height * size.y) / 2;
			region.FollowBottomLeft = new Vector2(Screen.width * followBottomLeft.x, Screen.height * followBottomLeft.y);
			region.FollowTopRight = new Vector2(Screen.width * followTopRight.x, Screen.height * followTopRight.y);
#if UNITY_EDITOR
			//Logger.Info("屏幕大小 width:{0}, height:{1}", Screen.width, Screen.height);
			//Logger.Info("摇杆可呼出区域 center:{0}, halfsize:{1}", region.Center, region.HalfSize);
			//Logger.Info("地盘可跟随区域 center:{0}, halfsize:{1}", region.FollowBottomLeft, region.FollowTopRight);
#endif
			//confFightConst = ConfigManager.Instance.GetConfig<ConfFightConst>();

        }

		/// <summary>
		/// 设置摇杆球中心点位置
		/// </summary>
		/// <param name="point">xy表示屏幕比例0到1</param>
		public static void SetAxisPoint(Vector2 point)
		{
			axisPoint = new Vector2(Screen.width * point.x, Screen.height * point.y);
		}

		public static Vector2 GetAxisPoint()
		{
			return axisPoint;
		}

		/// <summary>
		/// 在UI处设置，摇杆外框大小和中间的大小
		/// </summary>
		public static void SetAxisRadius(float availableRadius, float centerRadius, float outRadius)
		{
			availableAxisRadius = availableRadius;
			outAxisRadius = outRadius;
			centerAxisRadius = Mathf.Min(availableRadius, centerRadius);
		}

		/// <summary>
		/// 摇杆的中间球的半径
		/// </summary>
		/// <returns></returns>
		public static float GetAxisJoystickRadius()
		{
			return centerAxisRadius;
		}
		/// <summary>
		/// 摇杆中间球可移动半径
		/// </summary>
		/// <returns></returns>
		public static float GetAvailableAxisRadius()
		{
			return availableAxisRadius;
		}

		/// <summary>
		/// 获取按键获压住的时间
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static float GetKeyHoldTime(VirtualJoystickKey key)
		{
			return keyTimer[(int)key].Current;
		}

		/// <summary>
		/// 左侧摇杆方向
		/// </summary>
		/// <returns></returns>
		public static Vector2 GetLeftAxisDirection()
		{
			return leftAxisDirection;
		}

		/// <summary>
		/// 获取左侧摇杆改变后保持的事件
		/// </summary>
		/// <returns></returns>
		public static float GetLeftAxisHoldTime()
		{
			return leftAxisTimer.Current;
		}

		public static Vector2 GetLeftAxisPosition()
		{
			return leftAxisFingerDownPosition;
		}

		public static bool IsAnyKeyHold()
		{
			for (var i = 0; i < keyHold.Length; i++)
			{
				if (keyHold[i])
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsKeyDown(VirtualJoystickKey key)
		{
			return keyDown[(int)key];
		}

		public static bool IsKeyHold(VirtualJoystickKey key)
		{
			return keyHold[(int)key];
		}

		public static bool IsKeyUp(VirtualJoystickKey key)
		{
			return keyUp[(int)key];
		}

		public static bool IsLeftAxisHold()
		{
			return leftAxisHold;
		}

		public static void Reset()
		{
			ResetLeftAxis();
			ResetKeys();
			ResetClick();
			ResetScale();
		}

		private static void ResetLeftAxis()
		{
            leftAxisFingerId = -1;
			leftAxisFingerDownPosition = Vector2.zero;
			leftAxisHold = false;
			leftAxisDirection = Vector2.zero;
			leftAxisTimer.Reset();

            startTime = 0;
            endTime = 0;
        }

		private static void ResetKeys()
		{
			ResetKeyboard();
			for (var i = 0; i < (int)VirtualJoystickKey.COUNT; ++i)
			{
				keyPressed[i] = false;
				keyHold[i] = false;
				keyDown[i] = false;
				keyUp[i] = false;
				keyTimer[i].Reset();
			}
		}

		public static void ClearKey(VirtualJoystickKey key)
		{
			var i = (int)key;
			keyHold[i] = false;
			keyDown[i] = false;
			keyUp[i] = false;
			keyPressed[i] = true;
		}

		public static void PressKey(VirtualJoystickKey key, bool pressed)
		{
			var i = (int)key;
			if (keyPressed[i])
			{
				// 同一帧按下，后面的覆盖前面的
				keyDown[i] = pressed;
			}
			else
			{
				if (keyDown[i])
				{
					// 这一帧第一次按下，如果之前有keyDown，那么不再触发。
					keyDown[i] = false;
				}
				else
				{
					// 这一帧第一次按下，之前没有keyDown，设置为pressed
					keyDown[i] = pressed;
				}
			}
			keyHold[i] = pressed;
			keyUp[i] = !pressed;
			keyPressed[i] = true;
			//Logger.Debug("Key {0} pressed {1} {2} {3}", key, keyHold[i], keyDown[i], keyUp[i]);
		}
		public static void Tick(float deltaTime)
		{
			// 判断左边摇杆
			TickLeftAxis(deltaTime);
			// 判断按钮
			TickKeys(deltaTime);
			// 判断点击
			TickForClick();
        }

		private static void TickLeftAxis(float deltaTime)
		{
			if (axisEnabled)
			{
				leftAxisTimer.Increase(deltaTime);

				_TickLeftAxisForTouch(deltaTime);
				if (!leftAxisHold) _TickLeftAxisForKeyboard(deltaTime);
				if (!leftAxisHold) _TickLeftAxisForJoystick(deltaTime);

				if (!leftAxisHold)
				{
					leftAxisTimer.Reset();
				}
			}
		}

		private static void TickKeys(float deltaTime)
		{
			for (var i = 0; i < (int)VirtualJoystickKey.COUNT; ++i)
			{
				if (!keyPressed[i])
				{
					keyHold[i] = false;
					keyDown[i] = false;
					keyUp[i] = false;
				}
			}

			_TickKeysForKeyboard(deltaTime);
			_TickKeysForJoystick(deltaTime);

			for (var i = 0; i < (int)VirtualJoystickKey.COUNT; ++i)
			{
				keyPressed[i] = false;
				keyTimer[i].Increase(deltaTime);
				if (!keyHold[i] && !keyUp[i])
				{
					keyTimer[i].Reset();
				}
			}
		}

		private static void _TickLeftAxisForTouch(float deltaTime)
		{
			if (leftAxisHold)
			{
				var needReset = true;
                Vector2 lastPostion = Vector2.zero;
				for (var ii = 0; ii < UnityEngine.Input.touchCount; ++ii)
				{
					var touch = UnityEngine.Input.GetTouch(ii);
					if (touch.fingerId == leftAxisFingerId)
					{
						if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
						{
							leftAxisDirection = touch.position - leftAxisFingerDownPosition;
							if (leftAxisDirection.magnitude > availableAxisRadius)
							{
								leftAxisDirection = leftAxisDirection.normalized * availableAxisRadius;
							}
							needReset = false;
						}

                        lastPostion = touch.position;

                        break;
					}
				}

				if (needReset)
				{
                    //if (startTime != 0 && confFightConst != null && confFightConst.JoystickMinDistance > 0 && confFightConst.JoystickMaxTime > 0)
                    {
                        endTime = Time.time;
                       // Logger.Trace("holdTime:" + (endTime - startTime) + " distance:" + (lastPostion- leftAxisFingerDownPosition).magnitude + " lastPostion:" + lastPostion + " leftAxisFingerDownPosition:" + leftAxisFingerDownPosition);
                        //if ((lastPostion - leftAxisFingerDownPosition).magnitude >= confFightConst.JoystickMinDistance && endTime - startTime <= confFightConst.JoystickMaxTime && endTime - startTime > 0)
                        {
                            //设置角色方向
                            leftAxisDirection = lastPostion - leftAxisFingerDownPosition;
                            if (leftAxisDirection.magnitude > availableAxisRadius)
                            {
                                leftAxisDirection = leftAxisDirection.normalized * availableAxisRadius;
                            }
                           // HostPlayer.Instance.GetLeader().SetRotation(FixedQuaternion.LookRotation(new FixedVector3(leftAxisDirection.x, 0, leftAxisDirection.y)));
                            PressKey(VirtualJoystickKey.C, false);
                        }
                    }
                    ResetLeftAxis();
                }
            }
			else
			{
				for (var ii = 0; ii < UnityEngine.Input.touchCount; ++ii)
				{
					var touch = UnityEngine.Input.GetTouch(ii);
					if (touch.phase == TouchPhase.Began)
					{
						if (CheckAxisRegion(touch.position))
						{
							if (IsTouchOverOtherUI(ii))
							{
								return;
							}
							leftAxisHold = true;
							leftAxisFingerId = touch.fingerId;

                            leftAxisFingerDownPosition = FixFollowPosition(touch.position);
							leftAxisDirection = touch.position - leftAxisFingerDownPosition;
							if(leftAxisDirection.magnitude > availableAxisRadius)
							{
								leftAxisDirection = leftAxisDirection.normalized * availableAxisRadius;
							}
                            startTime = Time.time;
                            break;
						}
					}
				}
			}
		}

		private static bool IsTouchOverOtherUI(int ii)
		{
			//var touch = UICamera.GetTouch(ii, false);
			//if (touch != null && touch.isOverUI)
			//{
			//	return !GUIJoyStick.ContainsGameObject(touch.current);
			//}
			return false;
		}

	}

	internal struct AxisAlignRectangle2D
	{
		public Vector2 Center;
		public Vector2 HalfSize;
		public Vector2 FollowBottomLeft;
		public Vector2 FollowTopRight;

		public Vector2 GetCenter()
		{
			return Center;
		}

		public bool Inside(Vector2 point)
		{
			var v = point - Center;
			if (v.x < -HalfSize.x || v.x > HalfSize.x || v.y < -HalfSize.y || v.y > HalfSize.y)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// 把point限制在相对摇杆外圈范围以里radius的矩形范围内，方向不变
		/// </summary>
		/// <param name="point"></param>
		/// <param name="radius"></param>
		/// <returns></returns>
		public Vector2 FixFollowPosition(Vector2 point, float radius)
		{
			Vector2 fixPosition = point;
			if (FollowTopRight.x - FollowBottomLeft.x > radius)
			{
				if (point.x - radius < FollowBottomLeft.x)
				{
					fixPosition.x = FollowBottomLeft.x + radius;
				}
				if (point.x + radius > FollowTopRight.x)
				{
					fixPosition.x = FollowTopRight.x - radius;
				}
			}
			if (FollowTopRight.y - FollowBottomLeft.y > radius)
			{
				if (point.y - radius < FollowBottomLeft.y)
				{
					fixPosition.y = FollowBottomLeft.y + radius;
				}
				if (point.y + radius > FollowTopRight.y)
				{
					fixPosition.y = FollowTopRight.y - radius;
				}
			}
			return fixPosition;
		}
	}
}