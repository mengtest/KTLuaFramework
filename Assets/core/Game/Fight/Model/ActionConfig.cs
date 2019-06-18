
using Kernel.core;

namespace Kernel.Game
{
	public enum AnimatorParamType
	{
		[Comment("需要lerp的float参数")]
		LERP_FLOAT,
		[Comment("int参数")]
		INTEGER,
		[Comment("非lerp的float参数")]
		FLOAT,
		[Comment("bool参数")]
		BOOL,
		[Comment("trigger参数")]
		TRIGGER,
		[Comment("用PlatState播放")]
		STATE,
		[Comment("指的是控制速度的参数")]
		SPEED_CONTROL,
		[Comment("用于替换的clip")]
		FREE_CLIP,
	}

	public class ActionParam
	{
		public int ParamNameHash;
		public AnimatorParamType ParamType;
		public int IntegerValue;
		public float FloatValue;
		public float lerpTime;
		public bool BooleanValue;
		public int layer;
		public float normalizedTime;
	}

}
