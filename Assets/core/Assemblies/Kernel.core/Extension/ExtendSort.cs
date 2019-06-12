using System;

namespace Kernel.core
{
	public static class ExtendSort
	{
		public static void InsertSortEx<T>(this T[] list, Comparison<T> comparison)
		{
			if(null == list)
			{
				throw new ArgumentNullException("list is null");
			}

			if(null == comparison)
			{
				throw new ArgumentNullException("comparison is null");
			}

			int count = list.Length;
			for(int i = 1; i < count; ++i)
			{
				if(comparison(list[i], list[i - 1]) < 0)
				{
					T temp = list[i];
					int j = i;
					while(j > 0 && comparison(temp, list[j - 1]) < 0)
					{
						list[j] = list[j - 1];
						--j;
					}

					list[j] = temp;
				}
			}
		}
	}
}