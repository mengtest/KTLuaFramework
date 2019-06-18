using UnityEngine;
using Logger = Kernel.Log.Logger;

namespace Kernel.Game
{
	public static class AnimatorUtil
	{
		public static void SetParam(this AnimatorComponent com, ActionParam param)
		{
			if (param == null)
			{
				return;
			}

			switch (param.ParamType)
			{
				case AnimatorParamType.INTEGER:
					com.SetParam(param.ParamNameHash, param.IntegerValue);
					break;
				case AnimatorParamType.FLOAT:
					com.SetParam(param.ParamNameHash, param.FloatValue);
					break;
				case AnimatorParamType.LERP_FLOAT:
					com.SetParam(param.ParamNameHash, param.FloatValue, param.lerpTime, Time.deltaTime);
					break;
				case AnimatorParamType.BOOL:
					com.SetParam(param.ParamNameHash,param.BooleanValue);
					break;
				case AnimatorParamType.TRIGGER:
					com.SetParam(param.ParamNameHash);
					break;
				case AnimatorParamType.STATE:
					com.PlayState(param.ParamNameHash,param.layer,param.normalizedTime);
					break;
				case AnimatorParamType.SPEED_CONTROL:
					com.SetSpeed(param.FloatValue);
					break;
				default:
					Logger.Error("Animator start 不接受该类型的参数 name {0} type {1}", param.ParamNameHash, param.ParamType);
					break;
			}
		}

		public static void SetParam(this AnimatorComponent com, int id, bool value)
		{
			if (com.animator != null)
			{
				com.animator.SetBool(id, value);
			}
		}

		public static void SetParam(this AnimatorComponent com, int id, float value)
		{
			if (com.animator != null)
			{
				com.animator.SetFloat(id, value);
			}
		}

		public static void SetParam(this AnimatorComponent com, int id, int value)
		{
			if (com.animator != null)
			{
				com.animator.SetInteger(id, value);
			}
		}

		public static void SetParam(this AnimatorComponent com, int id, float value, float dampTime, float deltaTime)
		{
			if (com.animator != null)
			{
				com.animator.SetFloat(id, value, dampTime, deltaTime);
			}
		}

		public static void SetParam(this AnimatorComponent com, int id)
		{
			if (com.animator != null)
			{
				com.animator.ResetTrigger(id);
			}
		}

		public static void PlayState(this AnimatorComponent com, int stateNameHash, int layer,float normalizedTime)
		{
			if (com.animator != null)
			{
				com.animator.Play(stateNameHash,layer, normalizedTime);
			}
		}

		public static void CrossFade(this AnimatorComponent com,int stateHashName, float normalizedTransitionDuration, int layer, float normalizedTimeOffset, float normalizedTransitionTime,bool isFixedTime=false)
		{
			if (com.animator != null)
			{
				if(isFixedTime)
					com.animator.CrossFadeInFixedTime(stateHashName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
				else
					com.animator.CrossFade(stateHashName,normalizedTransitionDuration,layer, normalizedTimeOffset, normalizedTransitionTime);
			}
		}

		public static AnimationClip GetAnimationClipWithRuntimeController(this AnimatorComponent com, string clipName)
		{
			if (com.animator != null && com.animator.runtimeAnimatorController != null)
			{
				var controller = com.animator.runtimeAnimatorController as AnimatorOverrideController;
				if (controller != null)
				{
					return controller[clipName];
				}
			}
			return null;
		}

		public static RuntimeAnimatorController GetRuntimeAnimatorController(this AnimatorComponent com)
		{
			if (com.animator != null)
			{
				return com.animator.runtimeAnimatorController;
			}
			return null;
		}

		public static void SetRuntimeAnimatorController(this AnimatorComponent com, AnimatorOverrideController controller)
		{
			if (com.animator != null)
			{
				com.animator.runtimeAnimatorController= controller;
			}
		}
		public static void SetAnimator(this AnimatorComponent com, Animator animator)
		{
			com.animator = animator;
		}

		public static void SetActive(this AnimatorComponent com, bool active)
		{
			if (com.animator != null)
			{
				com.animator.enabled = active;
			}
		}

		public static void SetApplyRootMotion(this AnimatorComponent com, bool active)
		{
			if (com.animator != null)
			{
				com.animator.applyRootMotion = active;
			}
		}

		public static void SetApplyCullMode(this AnimatorComponent com, AnimatorCullingMode mode)
		{
			if (com.animator != null)
			{
				com.animator.cullingMode = mode;
			}
		}

		public static void SetSpeed(this AnimatorComponent com, float speed)
		{
			if (com.animator != null)
			{
				com.animator.speed = speed;
			}
		}
	}
}


