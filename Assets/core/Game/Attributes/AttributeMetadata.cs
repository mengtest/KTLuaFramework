
using Kernel.core;

namespace Kernel.Game
{
	public class AttributeMetadata
	{
		[Comment("属性")]
		public AttributeType Type;

		[Comment("值类型")]
		public AttributeModifyType ModifyType;

		[Comment("值")]
		public double Value;
	}
}
