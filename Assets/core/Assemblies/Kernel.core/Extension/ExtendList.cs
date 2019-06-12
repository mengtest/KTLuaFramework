using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public static class ExtendList
	{
		public static T BackEx<T>(this List<T> list)
		{
			if(null != list)
			{
				var count = list.Count;
				if(count > 0)
				{
					var idxLast = count - 1;
					var back = list[idxLast];

					return back;
				}
			}

			return default(T);
		}

		public static int ClampIndexEx<T>(this List<T> list, int index)
		{
			if(list == null || list.Count == 0 || index < 0)
			{
				return 0;
			}
			if(index >= list.Count)
			{
				return list.Count - 1;
			}
			return index;
		}

		public static bool EmptyEx<T>(this List<T> list)
		{
			return null != list && list.Count == 0;
		}

		public static T GetItemEx<T>(this List<T> list, int index)
		{
			if(list != null && index >= 0 && index < list.Count)
			{
				return list[index];
			}
			return default(T);
		}

		public static T LastOrDefaultEx<T>(this List<T> list)
		{
			if(list == null || list.Count == 0)
			{
				return default(T);
			}
			return list[list.Count - 1];
		}

		public static T PopBackEx<T>(this List<T> list)
		{
			if(null != list)
			{
				var count = list.Count;
				if(count > 0)
				{
					var idxLast = count - 1;
					var back = list[idxLast];
					list.RemoveAt(idxLast);

					return back;
				}
			}

			return default(T);
		}

		public static void ReserveEx<T>(this List<T> list, int minCapacity)
		{
			if(null != list)
			{
				var capacity = list.Capacity;

				if(minCapacity > capacity)
				{
					list.Capacity = Math.Max(Math.Max(capacity * 2, 4), minCapacity);
				}
			}
		}

		public static void Resize<T>(this List<T> list, int size, bool createDefaultElem = false)
		{
			if(list == null)
			{
				return;
			}

			var oldLen = list.Count;
			ResizeImpl(list, size, default(T));

			if(createDefaultElem)
			{
				for(var i = oldLen; i < list.Count; i++)
				{
					list[i] = (T) Activator.CreateInstance(typeof(T));
				}
			}
		}

		private static void ResizeImpl<T>(List<T> list, int size, T defaultValue)
		{
			var count = list.Count;
			if(size > count)
			{
				if(list.Capacity < size)
				{
					list.Capacity = size;
				}

				for(var i = count; i < size; ++i)
				{
					list.Add(defaultValue);
				}
			}

			if(size < count)
			{
				list.RemoveRange(size, count - size);
			}
		}
	}
}