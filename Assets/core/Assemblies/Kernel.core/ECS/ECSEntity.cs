using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.core
{
	public sealed class ECSEntity
	{
		private Dictionary<BitSet, IComponent> m_componentsTuple=new Dictionary<BitSet, IComponent>();
		private BitSet bitMask=ECSFamily.BitFalse;

		public BitSet BitMask {get { return bitMask; }}

		public HashSet<IComponent> GetComponentsTuple(IList<BitSet> names, HashSet<IComponent> result)
		{
			if (names.Count == 0)
				return null;

			if(result==null)
				result = new HashSet<IComponent>();

			for (int i = 0, j = names.Count; i < j; ++i)
			{
				result.Add(m_componentsTuple[names[i]]);
			}

			return result;
		}

		public IComponent GetComponent(BitSet name)
		{
			IComponent com = null;
			m_componentsTuple.TryGetValue(name, out com);
			return com;
		}

		public void AddComponent<T>() where T : IComponent, new()
		{
			var com = ECSFamily.GetComponent<T>();
			var mask= ECSFamily.GetKey<T>();
			if (m_componentsTuple.ContainsKey(mask))
			{
				Debug.LogWarningFormat("com {0} is repeat", typeof(T).Name);
				return;
			}

			bitMask.OrEquals(mask);
			m_componentsTuple.Add(mask, com);
		}

		public bool DelComponent<T>() where T : IComponent
		{
			var mask = ECSFamily.GetKey<T>();
			bitMask.AndEquals(~mask);
			ECSFamily.DelComponent(m_componentsTuple[mask]);
			return m_componentsTuple.Remove(mask);
		}

		public bool HasComponent<T>() where T : IComponent
		{
			var mask = ECSFamily.GetKey<T>();
			return m_componentsTuple.ContainsKey(mask);
		}

		public void Clear()
		{
			foreach (var item in m_componentsTuple.Values)
			{
				var mask = ECSFamily.GetKey(item);
				ECSFamily.DelComponent(m_componentsTuple[mask]);
			}

			m_componentsTuple.Clear();
		}
	}
}

