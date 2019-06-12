using Kernel.Engine;

namespace Kernel.core
{
	public class FixedCounter : ICounter<Fixed>
	{
		public FixedCounter()
		{
			Redefine(Fixed.MaxValue);
		}

		public FixedCounter(Fixed total)
		{
			Redefine(total);
		}

		public Fixed Remain
		{
			get
			{
				if(IsPositiveFinity())
				{
					return FixedMathf.Max(0, Total - Current);
				}
				return Fixed.MaxValue;
			}
		}

		public Fixed RemainNormalized
		{
			get
			{
				if(IsPositiveFinity())
				{
					return Remain / Total;
				}
				return 1;
			}
		}

		public Fixed Current
		{
			get;
			private set;
		}

		public Fixed CurrentNormalized
		{
			get
			{
				if(IsPositiveFinity())
				{
					return FixedMathf.Min(Total, Current) / Total;
				}
				return 0;
			}
		}

		public Fixed Total
		{
			get;
			private set;
		}

		public void Delay(Fixed extra)
		{
			if(extra > 0)
			{
				Total += extra;
			}
		}

		public void Exceed()
		{
			Current = Total;
		}

		//当增加到目标值后，返回true，否则返回false
		public bool Increase(Fixed delta)
		{
			if(Current + delta < Total)
			{
				Current += delta;
			}
			else
			{
				Current = Total;
			}
			return IsExceed();
		}

		public void Infinity()
		{
			Total = Fixed.MaxValue;
			Reset();
		}

		public bool IsExceed()
		{
			return Current >= Total;
		}

		public bool IsFinity()
		{
			return Total < Fixed.MaxValue;
		}

		public bool IsInfinity()
		{
			return Total == Fixed.MaxValue;
		}

		public bool IsNotExceed()
		{
			return Current < Total;
		}

		public bool IsPositiveFinity()
		{
			return 0 < Total && Total < Fixed.MaxValue;
		}

		public bool IsZero()
		{
			return Total == 0;
		}

		public void Redefine(Fixed total)
		{
			if(total < 0)
			{
				Total = Fixed.MaxValue;
			}
			else
			{
				Total = total;
			}
			Reset();
		}

		public void Reset()
		{
			Current = 0;
		}

		public void Zero()
		{
			Total = 0;
			Reset();
		}
	}
}
