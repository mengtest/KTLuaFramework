using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public class TempHashSet<T> : HashSet<T>, IDisposable
	{
		private static readonly Stack<object> cache = new Stack<object>();

		public static TempHashSet<T> Alloc()
		{
			if (cache.Count > 0)
			{
				return (TempHashSet<T>)cache.Pop();
			}
			else
			{
				return new TempHashSet<T>();
			}
		}

		public void Dispose()
		{
			Clear();
			cache.Push(this);
		}
	}
}
