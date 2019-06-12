using System;
using System.Collections.Generic;
using System.Linq;

namespace Kernel.core
{
	public static class ExtendArray
	{
		public static T[] AppendEx<T>(this T[] array, T a)
		{
			var oldSize = null == array ? 0 : array.Length;
			Array.Resize(ref array, oldSize + 1);
			array[oldSize] = a;

			return array;
		}

		public static T[] AppendEx<T>(this T[] array, IEnumerable<T> list)
		{
			if(null != list)
			{
				var appendLength = list.Count();
				var orginalLength = null == array ? 0 : array.Length;
				var next = new T[orginalLength + appendLength];

				if(null != array && orginalLength > 0)
				{
					Array.Copy(array, next, orginalLength);
				}

				var e = list.GetEnumerator();
				using(e)
				{
					var index = orginalLength;
					while(e.MoveNext())
					{
						next[index++] = e.Current;
					}
				}

				return next;
			}

			return array;
		}

		public static T[] BlockCloneEx<T>(this T[] array) where T : struct
		{
			if(null != array)
			{
				var length = array.Length;
				var cloned = new T[length];
				Buffer.BlockCopy(array, 0, cloned, 0, Buffer.ByteLength(array));
				return cloned;
			}

			return null;
		}

		public static void ClearEx<T>(this T[] array)
		{
			if(null != array)
			{
				Array.Clear(array, 0, array.Length);
			}
		}

		public static T GetItemEx<T>(this T[] list, int index)
		{
			if(list != null && index >= 0 && index < list.Length)
			{
				return list[index];
			}
			return default(T);
		}

		public static T LastOrDefaultEx<T>(this T[] list)
		{
			if(list == null || list.Length == 0)
			{
				return default(T);
			}
			return list[list.Length - 1];
		}

		// 如果找不到，返回-1
		public static int IndexOfEx<T>(this T[] list, T target)
		{
			if(list != null)
			{
				var imax = list.Length;
				for(var i = 0; i < imax; ++i)
				{
					if(list[i] == null)
					{
						return target == null ? i : -1;
					}
					if(list[i].Equals(target))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static int IndexOfEx<T>(this T[] list, Predicate<T> pred)
		{
			if(list != null)
			{
				var imax = list.Length;
				for(var i = 0; i < imax; ++i)
				{
					if(pred(list[i]))
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static int IndexOfEx(this int[] list, int target)
		{
			if(list != null)
			{
				var imax = list.Length;
				for(var i = 0; i < imax; ++i)
				{
					if(list[i] == target)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static long IndexOfEx(this long[] list, long target)
		{
			if(list != null)
			{
				var imax = list.Length;
				for(var i = 0; i < imax; ++i)
				{
					if(list[i] == target)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public static Array Resize(this Array list, int size, bool createDefaultElem = false)
		{
			if(list != null)
			{
				var elementType = list.GetType().GetElementType();
				if(elementType != null)
				{
					var result = Array.CreateInstance(elementType, size);

					for(var i = 0; i < list.Length && i < result.Length; i++)
					{
						result.SetValue(list.GetValue(i), i);
					}

					if(createDefaultElem)
					{
						for(var i = list.Length; i < result.Length; i++)
						{
							result.SetValue(Activator.CreateInstance(elementType), i);
						}
					}
					return result;
				}
			}
			return null;
		}
	}
}