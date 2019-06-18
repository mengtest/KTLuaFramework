using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public enum Usage
	{
		IMMEDIATE,
		AFTER_TICK
	}

	/// <summary>
	///     游戏事件管理器。最好采用组合的方式使用。不要更改这个类任何内容。
	/// </summary>
	/// <typeparam name="T">T继承自Event，是游戏中产生的事件</typeparam>
	public class EventManager<T> where T : Event
	{
		private readonly HashSet<int> firingTypes = new HashSet<int>();
		private readonly EventSource<T> source = new EventSource<T>();
		private readonly List<T> tickEvents = new List<T>();

		public virtual void FireEvent(T value, Usage usage = Usage.IMMEDIATE)
		{
			switch(usage)
			{
				case Usage.IMMEDIATE:
					FireEventImmediate(value);
					break;
				case Usage.AFTER_TICK:
					tickEvents.Add(value);
					break;
			}
		}

		public virtual void Tick(float deltaTime)
		{
			if(tickEvents.Count > 0)
			{
				foreach(var t in tickEvents)
				{
					FireEventImmediate(t);
				}
				tickEvents.Clear();
			}
		}

		public virtual void SubscribeEvent(int type, Action<T> handler)
		{
			source.SubscribeEvent(type, handler);
		}

		public virtual void UnsubscribeEvent(int type, Action<T> handler)
		{
			source.UnsubscribeEvent(type, handler);
		}

		public void Destroy()
		{
			tickEvents.Clear();
			firingTypes.Clear();
			source.Destroy();
		}

		private void FireEventImmediate(T value)
		{
			firingTypes.Add(value.Type);
			source.FireEvent(value);
			firingTypes.Remove(value.Type);
		}
	}
}