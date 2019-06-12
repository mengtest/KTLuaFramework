using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.core.Pool
{
	public class Pool<T> : IPool where T : IPoolItem
	{
		protected Func<T> creator;
		private readonly Stack<T> pool = new Stack<T>(8);

		public Pool(Func<T> creator)
		{
			this.creator = creator;
		}

		public IPoolItem SpawnItem()
		{
			return Spawn();
		}

		public void RecycleItem(IPoolItem item)
		{
			Recycle((T)item);
		}

		public void Clear()
		{
			foreach(var item in pool)
			{
				item.Dispose();
			}
			pool.Clear();
		}

		public void Recycle(T item)
		{
			if(item != null)
			{
				item.OnPreRecycle();
				pool.Push(item);
			}
		}

		public T Spawn()
		{
			if(pool.Count > 0)
			{
				var product = pool.Pop();
				return product;
			}

			if(creator != null)
			{
				var product = creator();
				return product;
			}

			Debug.LogError("creator is null");
			var result = default(T);
			return result;
		}

		public bool TryGetCache(ref T ret)
		{
			if(pool.Count > 0)
			{
				var product = pool.Pop();
				ret = product;
				return true;
			}

			return false;
		}
	}
}