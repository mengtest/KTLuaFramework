using System.Collections;
using System;

namespace Kernel.core
{
	public static class ExtendDouble
	{
		public const double EPSILON = 1e-15f;

		public static bool EqualsEx(this double a, double b)
		{
			return IsZero(a - b);
		}

		public static bool IsZero(this double a)
		{
			if(a < EPSILON && a > -EPSILON)
				return true;
			return false;
		}

		public static bool GreaterEx(this double a, double b)
		{
			return a - b > EPSILON;
		}

		public static bool GreaterOrEqualsEx(this double a, double b)
		{
			return a - b > -EPSILON;
		}
	}
}