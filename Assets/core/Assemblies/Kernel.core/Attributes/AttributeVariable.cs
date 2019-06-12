using System;

namespace Kernel.core
{
	public struct AttributeVariable
	{
		private readonly int key;
        public AttributeValue value;

		public AttributeVariable(int key, AttributeValue value)
		{
			this.key = key;
			this.value = value;
		}

		public int Key => key;

		public AttributeValue Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
			}
		}

		public void Add(AttributeModifyType attributeModifyType, double v)
		{
			value.Add(attributeModifyType, v);
		}

		public void Set(AttributeModifyType attributeModifyType, double v)
		{
			value.Set(attributeModifyType, v);
		}

		public void Add(AttributeVariable v)
		{
			if(v.key == key)
			{
				value.Add(v.value);
			}
		}

		public void Sub(AttributeVariable v)
		{
			if(v.key == key)
			{
				value.Sub(v.value);
			}
		}

		public void Set(AttributeVariable v)
		{
			if(v.key == key)
			{
				value.Set(v.value);
			}
		}

		public AttributeVariable Multiple(int count)
		{
			value.Multiple(count);
			return this;
		}
	}
}
