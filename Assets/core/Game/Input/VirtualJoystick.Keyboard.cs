using UnityEngine;

namespace Kernel.Game
{
	public static partial class VirtualJoystick
	{
		private static readonly bool[] keyboardDown = new bool[(int)VirtualJoystickKey.COUNT];
		private static readonly bool[] keyboardHold = new bool[(int)VirtualJoystickKey.COUNT];
		private static readonly bool[] keyboardUp = new bool[(int)VirtualJoystickKey.COUNT];
		private static readonly KeyCode[][] keyboardKeys =
		{
			new[]
			{
				KeyCode.J
			},
			new[]
			{
				KeyCode.K
			},
			new[]
			{
				KeyCode.L
			},
			new[]
			{
				KeyCode.Space
			},
			new[]
			{
				KeyCode.Alpha4, KeyCode.Keypad4
			},
			new[]
			{
				KeyCode.Alpha5, KeyCode.Keypad5
			},
			null,
			new[]
			{
				KeyCode.Alpha1, KeyCode.Keypad1
			},
			new[]
			{
				KeyCode.Alpha2, KeyCode.Keypad2
			},
			new[]
			{
				KeyCode.Alpha3, KeyCode.Keypad3
			},
			new[]
			{
				KeyCode.U
			}
		};

		private static void _TickLeftAxisForKeyboard(float deltaTime)
		{
			if (leftAxisDirection.sqrMagnitude < float.Epsilon)
			{
				ResetLeftAxis();
			}
			if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow))
			{
                leftAxisDirection += Vector2.up * availableAxisRadius;
			}
			if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow))
			{
                leftAxisDirection += Vector2.down * availableAxisRadius;
			}
			if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow))
			{
                leftAxisDirection += Vector2.left * availableAxisRadius;
			}
			if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow))
			{
                leftAxisDirection += Vector2.right * availableAxisRadius;
			}
			if (leftAxisDirection.sqrMagnitude < float.Epsilon)
			{
				ResetLeftAxis();
			}
			else
			{
                leftAxisHold = true;
				leftAxisFingerId = 0;
				leftAxisFingerDownPosition = axisPoint;
			}
		}

		private static void _TickKeysForKeyboard(float deltaTime)
		{
			for (var i = 0; i < (int)VirtualJoystickKey.COUNT; ++i)
			{
				if (!keyPressed[i])
				{
					if (keyboardKeys[i] == null)
					{
						continue;
					}
					for (var key = 0; key < keyboardKeys[i].Length; ++key)
					{
						if (UnityEngine.Input.GetKey(keyboardKeys[i][key]))
						{
							keyHold[i] = true;
						}
						if (UnityEngine.Input.GetKeyDown(keyboardKeys[i][key]))
						{
							keyDown[i] = true;
						}
						if (UnityEngine.Input.GetKeyUp(keyboardKeys[i][key]))
						{
							keyUp[i] = true;
						}
					}
					if (keyHold[i] || keyDown[i] || keyUp[i])
					{
						keyPressed[i] = true;
					}
				}
			}
		}

		private static void ResetKeyboard()
		{
			for (var i = 0; i < (int)VirtualJoystickKey.COUNT; ++i)
			{
				keyboardHold[i] = false;
				keyboardDown[i] = false;
				keyboardUp[i] = false;
			}
		}
	}
}