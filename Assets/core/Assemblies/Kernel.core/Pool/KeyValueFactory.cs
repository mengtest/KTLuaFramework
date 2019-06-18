using System;
using System.Collections.Generic;

namespace Kernel.core
{
	/// <summary>
	///    Key Value Factory
	/// </summary>
	/// <typeparam name="TKey">需要支持GetHashCode</typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class KeyValueFactory<TKey, TValue>
	{
		protected Dictionary<TKey, Func<TValue>> factoryMethods = new Dictionary<TKey, Func<TValue>>();

		public void AddCreator<T>(TKey type) where T : TValue, new()
		{
			AddCreator<T>(type, () => new T());
		}

		public void AddCreator<T>(TKey type, Func<TValue> creator) where T : TValue, new()
		{
			if(creator == null)
			{
				creator = () => new T();
			}
			factoryMethods.Add(type, creator);
		}

		public TValue Create(TKey type)
		{
			var ret = factoryMethods[type]();
			return ret;
		}
	}
}