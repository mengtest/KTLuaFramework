using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.core
{
	public class GenericPool<T>
	{
		protected Func<T> creator;
		protected Action<T> disposer;
		protected Action<T> initializer;
		protected Action<T> recycler;
		private readonly Stack<T> pool = new Stack<T>(8);

		// 使用继承的方式来设置各种函数;
		protected GenericPool()
		{
		}

		// 直接使用;
		public GenericPool(Func<T> creator, Action<T> initializer, Action<T> recycler, Action<T> disposer)
		{
			this.creator = creator;
			this.initializer = initializer;
			this.recycler = recycler;
			this.disposer = disposer;
		}

		public void Clear()
		{
			foreach(var item in pool)
			{
				if(null != item)
				{
					disposer(item);
				}
			}
			pool.Clear();
		}

		public void Recycle(T item)
		{
			if(item != null)
			{
				recycler(item);
				pool.Push(item);
			}
		}

		public T Spawn()
		{
			if(pool.Count > 0)
			{
				var product = pool.Pop();
				initializer(product);
				return product;
			}

			if(creator != null)
			{
				var product = creator();
				initializer(product);
				return product;
			}

			Debug.LogError("creator is null");
			return default(T);
		}
	}
}