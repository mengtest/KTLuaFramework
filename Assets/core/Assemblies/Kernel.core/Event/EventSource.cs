using System;
using System.Collections.Generic;
using Kernel.Log;

namespace Kernel.core
{
	internal class EventSource<T> where T : Event
	{
		private readonly Dictionary<int, List<Action<T>>> events = new Dictionary<int, List<Action<T>>>();

		internal void FireEvent(T value)
		{
			var key = (int)value.Type;
			if(events.ContainsKey(key))
			{
				List<Action<T>> list = events[key];
				if(list != null)
				{
					for(int i = 0; i < list.Count; ++i)
					{
						Action<T> action = list[i];
						if(action != null)
						{
							action.Invoke(value);
						}
					}
				}
			}

			IPoolItem poolItem = value as IPoolItem;
			if(poolItem != null)
			{
				poolItem.Dispose();
			}
		}

		internal void SubscribeEvent(int type, Action<T> handler)
		{
			if(events.ContainsKey(type))
			{
				List<Action<T>> list = events[type];
				if(list == null)
				{
					events[type]=new List<Action<T>>();
				}

#if UNITY_EDITOR
				if(list.Contains(handler))
				{
					Logger.Warn("重复注册事件 {0} {1}", type, handler);
					return;
				}
#endif
				list.Add(handler);
			}
			else
			{
				events.Add(type,new List<Action<T>>() {handler });
			}
		}

		internal void UnsubscribeEvent(int type, Action<T> handler)
		{
			if(events.ContainsKey(type))
			{
				List<Action<T>> list = events[type];
				if(list != null)
				{
					list.Remove(handler);
				}
			}
		}

		internal void Destroy()
		{
			events.Clear();
		}
	}
}