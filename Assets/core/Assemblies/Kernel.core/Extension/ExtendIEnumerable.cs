using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public static class ExtendIEnumerable
	{
		public static T FindEqualOrNearrest<T>(this IEnumerable<T> elemets, Func<T, int> fun, int value)
		{
			T result = default(T);
			var e = elemets.GetEnumerator();
			int minDifference = int.MaxValue;
			while(e.MoveNext())
			{
				var dif = Math.Abs(fun(e.Current) - value);
				if(dif < minDifference)
				{
					result = e.Current;
					minDifference = dif;
					if(dif == 0)
						break;
				}
			}
			return result;
		}
	}
}