using UnityEngine;

namespace Kernel.Game
{
	public class TransformUtil
	{
		public static void SetLookRotation(TransformComponent com,Vector3 direction)
		{
			if (direction.sqrMagnitude > 0.0f)
			{
				com.Rotation = Quaternion.LookRotation(direction);
			}
		}
		public static void SetRotationH(TransformComponent com, Vector3 direction)
		{
			direction.y = 0;
			if (direction.sqrMagnitude > 0)
			{
				direction.Normalize();
				com.Rotation = Quaternion.LookRotation(direction);
			}
		}
	}
}


