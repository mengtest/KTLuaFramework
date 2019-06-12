using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.core
{
	public class ECSFamily
	{
		public const int BitSetMax = 128;
		public static readonly BitSet BitFalse = new BitSet(BitSetMax, false);
		public static readonly BitSet BitTrue = new BitSet(BitSetMax, true);

		private static int seed;//自增长种子
		private static Dictionary<Type, BitSet> typeMap = new Dictionary<Type, BitSet>();
		private static Dictionary<Type, List<IComponent>> componentsPool = new Dictionary<Type, List<IComponent>>(BitSetMax);

		public static T GetComponent<T>() where T: IComponent,new()
		{
			var type = typeof(T);
			var com = new T();
			if (!componentsPool.ContainsKey(type))
				componentsPool.Add(type, new List<IComponent>() {com});
			else
				componentsPool[type].Add(com);

			return com;
		}

		public static void DelComponent(IComponent com)
		{
			var type = com.GetType();
			if (componentsPool.ContainsKey(type))
			{
				componentsPool[type].Remove(com);
				if (componentsPool[type].Count == 0)
					componentsPool.Remove(type);
			}
		}
		public static IList<IComponent> GetComponents<T>() where T : IComponent
		{
			var type = typeof(T);
			if (componentsPool.ContainsKey(type))
			{
				return componentsPool[type];
			}

			return null;
		}

		public static BitSet GetKey(IComponent com)
		{
			var t = com.GetType();
			BitSet result;
			if (!typeMap.TryGetValue(t,out result))
			{
				result = new BitSet(BitSetMax, false);
				result[++seed] = true;
				typeMap.Add(t, result);
			}

			return result;
		}

		public static BitSet GetKey<T>() where T: IComponent
		{
			var t = typeof(T);
			BitSet result;
			if (!typeMap.TryGetValue(t, out result))
			{
				result = new BitSet(BitSetMax, false);
				result[++seed] = true;
				typeMap.Add(t, result);
			}

			return result;
		}
	}
}

