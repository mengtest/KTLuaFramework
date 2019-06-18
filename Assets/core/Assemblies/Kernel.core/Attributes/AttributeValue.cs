using Kernel.Engine;
using UnityEngine;

namespace Kernel.core
{
	/// <summary>
	/// 总公式是 (basic * (1 + relative) + absolute) * (1 + percent) + extra
	/// </summary>
	public class AttributeValue
	{
		private double basic;
		private double relative;
		private double absolute;
		private double percent;
		private double extra;

		private bool dirty;
		private double value;
		private bool dirty1;

		private double value1;
		private bool dirty2;
		private double value2;

		public AttributeValue(double basic) : this(basic, 0, 0, 0, 0)
		{
			
		}

		public AttributeValue(double basic, double relative = 0, double absolute = 0, double percent = 0, double extra = 0)
		{
			this.basic = basic;
			this.relative = relative;
			this.absolute = absolute;
			this.percent = percent;
			this.extra = extra;
			dirty = true;
			dirty1 = true;
			dirty2 = true;
			value = value1 = value2 = 0;
		}

		public AttributeValue(AttributeModifyType attributeModifyType, double v)
		{
			basic = relative = absolute = percent = extra = 0;
			dirty = dirty1 = dirty2 = false;
			value = value1 = value2 = 0;
			Set(attributeModifyType, v);
		}

		public void Redefine(double b, double r = 0, double a = 0, double p = 0, double e = 0)
		{
			basic = b;
			relative = r;
			absolute = a;
			percent = p;
			extra = e;
		}

		public void Clear()
		{
			basic = relative = absolute = percent = extra = 0;
			dirty = dirty1 = dirty2 = false;
			value = value1 = value2 = 0;
		}

		public void Add(AttributeModifyType attributeModifyType, double v)
		{
			switch(attributeModifyType)
			{
				case AttributeModifyType.BASIC:
					Basic += v;
					break;
				case AttributeModifyType.RELATIVE:
					Relative += v;
					break;
				case AttributeModifyType.ABSOLUTE:
					Absolute += v;
					break;
				case AttributeModifyType.PERCENT:
					Percent += v;
					break;
				case AttributeModifyType.EXTRA:
					Extra += v;
					break;
			}
		}

		public void Set(AttributeModifyType attributeModifyType, double v)
		{
			switch(attributeModifyType)
			{
				case AttributeModifyType.BASIC:
					Basic = v;
					break;
				case AttributeModifyType.RELATIVE:
					Relative = v;
					break;
				case AttributeModifyType.ABSOLUTE:
					Absolute = v;
					break;
				case AttributeModifyType.PERCENT:
					Percent = v;
					break;
				case AttributeModifyType.EXTRA:
					Extra = v;
					break;
			}
		}

		public void Add(AttributeValue v)
		{
			Basic += v.Basic;
			Relative += v.Relative;
			Absolute += v.Absolute;
			Percent += v.Percent;
			Extra += v.Extra;
		}

		public void Sub(AttributeValue v)
		{
			Basic = Basic - v.Basic ;
			Relative = Relative - v.Relative ;
			Absolute = Absolute - v.Absolute ;
			Percent = Percent - v.Percent ;
			Extra = Extra - v.Extra ;
		}

		public void Set(AttributeValue v)
		{
			Basic = v.Basic;
			Relative = v.Relative;
			Absolute = v.Absolute;
			Percent = v.Percent;
			Extra = v.Extra;
		}

		public void Set(double basic,double relative=0,double absolute=0,double percent=0,double extra=0)
		{
			Basic = basic;
			Relative = relative;
			Absolute = absolute;
			Percent = percent;
			Extra = extra;
		}

		public double Basic
		{
			get
			{
				return basic;
			}
			set
			{
				basic = value;
				dirty = true;
				dirty1 = true;
			}
		}

		public double Relative
		{
			get
			{
				return relative;
			}
			set
			{
				relative = value;
				dirty = true;
				dirty1 = true;
			}
		}

		public double Absolute
		{
			get
			{
				return absolute;
			}
			set
			{
				absolute = value;
				dirty = true;
				dirty2 = true;
			}
		}

		public double Percent
		{
			get
			{
				return percent;
			}
			set
			{
				percent = value;
				dirty = true;
				dirty1 = true;
				dirty2 = true;
			}
		}

		public double Extra
		{
			get
			{
				return extra;
			}
			set
			{
				extra = value;
				dirty = true;
				dirty1 = true;
			}
		}

		/// <summary>
		/// = (basic * (1 + relative) + absolute) * (1 + percent) + extra
		/// </summary>
		public double Value
		{
			get
			{
				if(dirty)
				{
					value = (basic * (1 + relative) + absolute) * (1 + percent) + extra;
					if(value > int.MaxValue)
					{
						value = int.MaxValue;
						Debug.LogError("属性值超出上限");
					}
					dirty = false;
				}
				return value;
			}
		}

		/// <summary>
		/// = basic * (1 + relative) * (1 + percent) + extra 用来做转化属性前的计算值
		/// </summary>
		public double Value1
		{
			get
			{
				if(dirty1)
				{
					value1 = basic * (1 + relative) * (1 + percent) + extra;
					if (value1 > int.MaxValue)
					{
						value1 = int.MaxValue;
						Debug.LogError("属性值超出上限");
					}
					dirty1 = false;
				}
				return value1;
			}
		}

		/// <summary>
		/// = absolute * (1 + percent) 这是转化属性
		/// </summary>
		public double Value2
		{
			get
			{
				if(dirty2)
				{
					value2 = absolute * (1 + percent);
					if (value2 > int.MaxValue)
					{
						value2 = int.MaxValue;
						Debug.LogError("属性值超出上限");
					}
					dirty2 = false;
				}
				return value2;
			}
		}

		bool Equals(AttributeValue other)
		{
			return basic==other.basic && relative== other.relative && absolute== other.absolute && percent== other.percent && extra== other.extra;
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != GetType()) return false;
			return Equals((AttributeValue)obj);
		}

		// ReSharper disable NonReadonlyMemberInGetHashCode
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = basic.GetHashCode();
				hashCode = (hashCode * 397) ^ relative.GetHashCode();
				hashCode = (hashCode * 397) ^ absolute.GetHashCode();
				hashCode = (hashCode * 397) ^ percent.GetHashCode();
				hashCode = (hashCode * 397) ^ extra.GetHashCode();
				return hashCode;
			}
		}

		public AttributeValue Clone()
		{
			return new AttributeValue(basic, relative, absolute, percent, extra);
		}

		public void Multiple(int count)
		{
			Basic *= count;
			Relative *= count;
			Absolute *= count;
			Percent *= count;
			Extra *= count;
		}
	}
}