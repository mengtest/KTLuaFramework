using UnityEngine;

namespace Kernel.Game
{
	public static partial class VirtualJoystick
	{
		//public static bool IsShowOutLine { get; set; }

		private static readonly KeyCode[] joystickButtons =
		{
			KeyCode.JoystickButton0,
			KeyCode.JoystickButton1,
			KeyCode.JoystickButton2,
			KeyCode.JoystickButton3,
			KeyCode.JoystickButton4,
			KeyCode.JoystickButton5
		};

		/// <summary>
		/// 必须在Unity的Input设置中把这两个都改成Joystick Axis
		/// </summary>
		/// <param name="deltaTime"></param>
		private static void _TickLeftAxisForJoystick(float deltaTime)
		{
			leftAxisDirection = Vector2.zero;
			var x = UnityEngine.Input.GetAxis("Horizontal");
			var y = UnityEngine.Input.GetAxis("Vertical");
			var motion = new Vector2(x, y);
			leftAxisDirection = motion;
			if(leftAxisDirection.sqrMagnitude < float.Epsilon)
			{
				leftAxisHold = false;
				leftAxisDirection = Vector2.zero;
			}
			else
			{
				leftAxisHold = true;
			}
		}

		private static void _TickKeysForJoystick(float deltaTime)
		{
			for(var i = 0; i < joystickButtons.Length; ++i)
			{
				if(!keyPressed[i])
				{
					keyHold[i] = UnityEngine.Input.GetKey(joystickButtons[i]);
					keyDown[i] = UnityEngine.Input.GetKeyDown(joystickButtons[i]);
					keyUp[i] = UnityEngine.Input.GetKeyUp(joystickButtons[i]);
					if(keyHold[i] || keyDown[i] || keyUp[i])
					{
						keyPressed[i] = true;
					}
				}
			}
		}
	}
}