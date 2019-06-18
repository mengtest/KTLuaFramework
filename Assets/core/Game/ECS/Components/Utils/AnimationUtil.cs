using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.Game
{
	public static class AnimationUtil
	{
		public static void SetAnimation(this AnimationComponent com, Animation animation)
		{
			com.animation = animation;
		}

		public static void PlayAnimation(this AnimationComponent com, string clipName, float speed,WrapMode mode)
		{
			if (com.animation != null)
			{
				com.animation[clipName].speed = speed;
				com.animation[clipName].wrapMode = mode;
				com.animation.clip = com.animation[clipName].clip;
				com.animation.Play(clipName);
			}
		}

		public static bool HasPlayingClip(this AnimationComponent com)
		{
			return com.animation.clip!=null&& com.animation.IsPlaying(com.animation.clip.name);
		}
	}
}

