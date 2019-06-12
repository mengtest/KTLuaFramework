using System;
using System.Collections.Generic;

namespace Kernel.core
{
    public static class ExtendForEach
    {
        public static void ForEachEx<T>(this T[] array, Action<T> action)
        {
            if(null != array && null != action)
            {
                for(int i= 0; i< array.Length; ++i)
                {
                    action(array[i]);
                }
            }
        }

        public static void ForEachNotNullEx<T>(this T[] array, Action<T> action)
        {
            if(null != array && null != action)
            {
                for(int i= 0; i< array.Length; ++i)
                {
                    var item = array[i];
                    if(null != item)
                    {
                        action(array[i]);
                    }
                }
            }
        }

		public static void ForEachEx<T>(this HashSet<T> hashSet, Action<T> action)
		{
			if (null != hashSet && null != action)
			{
				var e = hashSet.GetEnumerator();
				using (e)
				{
					while (e.MoveNext())
					{
						action(e.Current);
					}
				}
			}
		}

        public static void ForEachEx<T>(this LinkedList<T> linkedList, Action<T> action)
        {
            if (null != linkedList && null != action)
            {
                var e = linkedList.GetEnumerator();
                using(e)
                {
                    while(e.MoveNext())
                    {
                        action(e.Current);
                    }
                }
            }
        }

        public static void ForEachNotNullEx<T>(this LinkedList<T> linkedList, Action<T> action)
        {
            if (null != linkedList && null != action)
            {
                var e = linkedList.GetEnumerator();
                using(e)
                {
                    while(e.MoveNext())
                    {
                        var item = e.Current;
                        if(null != item)
                        {
                            action(item);
                        }
                    }
                }
            }
        }

        public static void ForEachEx<K, V>(this Dictionary<K, V> dict, Action<KeyValuePair<K, V>> action)
        {
            if (null != dict && null != action)
            {
                var e = dict.GetEnumerator();
                using(e)
                {
                    while (e.MoveNext())
                    {
                        action(e.Current);
                    }
                }
            }
        }

        public static void ForEachEx<T>(this Queue<T> queue, Action<T> action)
        {
            if (null != queue && null != action)
            {
                var e = queue.GetEnumerator();
                using(e)
                {
                    while(e.MoveNext())
                    {
                        action(e.Current);
                    }
                }
            }
        }

        public static void ForEachNotNullEx<T>(this Queue<T> queue, Action<T> action)
        {
            if (null != queue && null != action)
            {
                var e = queue.GetEnumerator();
                using(e)
                {
                    while(e.MoveNext())
                    {
                        var item = e.Current;
                        if(null != item)
                        {
                            action(item);
                        }
                    }
                }
            }
        }
    }
}