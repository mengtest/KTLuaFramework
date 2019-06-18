using Kernel.core;

namespace Kernel.Game
{
	/// <summary>
	/// 都使用此类的GetAttributeVariables方法
	/// </summary>
	public class ConfAttributeSet
	{
		[Comment("编号")]
		public int Id;

		[Comment("属性包类型")]
		public AttributeSetType Type;

		[Comment("生效范围")]
		public AttributeEffectiveType EffectiveType;

		[Comment("图标")]
		public string Icon;

		[Comment("描述")]
		public string Description;

		[Comment("描述前缀")]
		public string DescriptionPrefix;

		[Comment("描述后缀")]
		public string DescriptionPostfix;

		[Comment("描述数值额外乘以")]
		public double DescriptionValueManification;

		[Comment("要转化的属性类型")]
		public AttributeType ChangeType;

		///每ChangeValue点属性，转化为n点目标属性
		[Comment("转化基准值")]
		public double ChangeValue;

		[Comment("属性列表")]
		public AttributeMetadata[] Attributes;

		public TempList<AttributeVariable> GetAttributeVariables(IAttributeProvider attributeProvider)
		{
			var list = TempList<AttributeVariable>.Alloc();
			if(Attributes != null && Attributes.Length > 0)
			{
				switch(Type)
				{
					case AttributeSetType.NORMAL:
						foreach(AttributeMetadata attribute in Attributes)
						{
							AttributeVariable v = new AttributeVariable((int)attribute.Type, new AttributeValue(attribute.ModifyType, attribute.Value));
							list.Add(v);
						}
						break;
					case AttributeSetType.CHANGE:
						AttributeVariable variable = attributeProvider.GetAttributeVariable((int)ChangeType);
						if(!variable.Value.Value1.IsZero())
						{
							double d = variable.Value.Value1 / ChangeValue;
							foreach(AttributeMetadata attribute in Attributes)
							{
								AttributeVariable v = new AttributeVariable((int)attribute.Type, new AttributeValue(AttributeModifyType.ABSOLUTE, attribute.Value * d));
								list.Add(v);
							}
						}
						break;
				}
			}
			return list;
		}
	}
}