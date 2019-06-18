using System.Collections.Generic;

namespace Kernel.core
{
	public class ObjectPool<T> where T : class, new()
	{
		private static readonly Stack<object> cache = new Stack<object>();

		static ObjectPool()
		{
//			if (PlatformInfo.IsEditor)
//				GenericTypeRoot.Register(typeof(ObjectPool<T>));
		}

		public static T Alloc()
		{
			if (cache.Count > 0)
			{
				return (T)cache.Pop();
			}
			else
			{
				return new T();
			}
		}

		public static void Dealloc(T t)
		{
			if (t != null)
			{
				cache.Push(t);
			}
		}
	}
}
