using UnityEngine;

namespace Kernel.core
{
	public static class ExtendBehaviour
	{
		public static bool GetEnabledEx(this Behaviour behaviour)
		{
			if(null != behaviour)
			{
				return behaviour.enabled;
			}

			return false;
		}

		public static void SetEnabledEx(this Behaviour behaviour, bool enabled)
		{
			if(null != behaviour)
			{
				behaviour.enabled = enabled;
			}
		}
	}
}