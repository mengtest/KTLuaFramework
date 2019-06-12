
namespace Kernel.core
{
	public enum AttributeModifyType
	{
		[Comment("固定值")]
		BASIC = 1,
		[Comment("百分比")]
		RELATIVE,
		[Comment("转化")]
		ABSOLUTE,
		[Comment("额外百分比")]
		PERCENT,
		[Comment("额外固定值")]
		EXTRA,
	}
}
