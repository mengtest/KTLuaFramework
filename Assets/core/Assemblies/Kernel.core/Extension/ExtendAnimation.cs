using System.Collections.Generic;
using UnityEngine;

namespace Kernel.core
{
	public static class ExtendAnimation
	{
		public static void CollectClipNamesEx(this Animation animation, List<string> names)
		{
			var e = animation.GetEnumerator();

			while(e.MoveNext())
			{
				var state = e.Current as AnimationState;
				var clip = state.clip;
				names.Add(clip.name);
			}
		}
	}
}