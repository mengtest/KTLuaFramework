using System;
using System.Collections.Generic;

namespace Kernel.core.Pool
{
	/// <summary>
	///     基于Pool的SimpleFactory;
	/// </summary>
	/// <typeparam name="TK"></typeparam>
	public class TypedPool<TK> where TK : IPoolItem
	{
		protected Dictionary<Type, Pool<TK>> pools = new Dictionary<Type, Pool<TK>>();

		public void AddCreator<T>() where T : TK, new()
		{
			AddCreator<T>(() => new T());
		}

		public void AddCreator<T>(Func<TK> creator) where T : TK, new()
		{
			if(creator == null)
			{
				creator = () => new T();
			}
			pools.Add(typeof(T), new Pool<TK>(creator));
		}

		public T Create<T>() where T : class, TK
		{
			var ret = pools[typeof(T)].Spawn() as T;
			return ret;
		}

		public Pool<TK> GetPool(Type type)
		{
			return pools[type];
		}
	}
}