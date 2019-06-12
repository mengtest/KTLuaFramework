using System;
using System.Collections.Generic;
using Kernel.game;

namespace Kernel.core
{
	public class ECSWorld : ITick, IFixedTick, ILateTick
	{
		private List<ISystem> m_systems = new List<ISystem>();
		private List<ITick> m_tickSystems = new List<ITick>();
		private List<IFixedTick> m_fixedTickSystems = new List<IFixedTick>();
		private List<ILateTick> m_lateTickSystems = new List<ILateTick>();
		private HashSet<ECSEntity> m_entities = new HashSet<ECSEntity>();


		//////////////////////////////////entity////////////////////////////////////////////

		public void AddEntity(ECSEntity entity)
		{
			if (m_entities.Contains(entity))
				return;

			m_entities.Add(entity);
		}

		public bool DelEntity(ECSEntity entity)
		{
			return m_entities.Remove(entity);
		}

		public HashSet<ECSEntity> GetEntities()
		{
			return m_entities;
		}

		public void ClearEntities()
		{
			foreach (var item in m_entities)
			{
				item.Clear();
			}

			m_entities.Clear();
		}

		//////////////////////////////////system////////////////////////////////////////////

		public void AddSystem(ISystem system)
		{
			if (m_systems.Contains(system))
				return;

			m_systems.Add(system);

			if (system is ITick)
				m_tickSystems.Add(system as ITick);
			else if(system is IFixedTick)
				m_fixedTickSystems.Add(system as IFixedTick);
			else if(system is ILateTick)
				m_lateTickSystems.Add(system as ILateTick);
		}

		public bool DelSystem(ISystem system)
		{
			if (system is ITick)
				m_tickSystems.Remove(system as ITick);
			else if (system is IFixedTick)
				m_fixedTickSystems.Remove(system as IFixedTick);
			else if (system is ILateTick)
				m_lateTickSystems.Remove(system as ILateTick);

			return m_systems.Remove(system);
		}

		public void ClearSystems()
		{
			m_systems.Clear();
			m_tickSystems.Clear();
			m_fixedTickSystems.Clear();
			m_lateTickSystems.Clear();
		}

		//////////////////////////////////component////////////////////////////////////////////

		/// <summary>
		/// 
		/// </summary>
		/// <param name="system"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public IList<HashSet<IComponent>> FectchAll(ISystem system, IList<HashSet<IComponent>> result)
		{
			if (result == null)
				result = new List<HashSet<IComponent>>();

			foreach (var item in m_entities)
			{
				if ((system.SubscribedComponentsMask & item.BitMask).IsZero)
					result.Add(item.GetComponentsTuple(system.SubscribedComponentMasks, new HashSet<IComponent>()));
			}

			return result;
		}

		/// <summary>
		/// </summary>
		/// <param name="system"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public IList<IComponent> FectchAll(ISystem system, IList<IComponent> result)
		{
			if (result == null)
				result = new List<IComponent>();

			foreach (var item in m_entities)
			{
				if ((system.SubscribedComponentsMask & item.BitMask).IsZero)
					result.Add(item.GetComponent(item.BitMask));
			}

			return result;
		}

		public void Copy()
		{
			
		}

		public void RollBack()
		{
			
		}

		public void Tick(float deltaTime)
		{
			for (int i = 0, j = m_tickSystems.Count; i < j; ++i)
			{
				m_tickSystems[i].Tick(deltaTime);
			}
		}

		public void FixedTick(float fixedDeltaTime)
		{
			for (int i = 0, j = m_fixedTickSystems.Count; i < j; ++i)
			{
				m_fixedTickSystems[i].FixedTick(fixedDeltaTime);
			}
		}

		public void LateTick(float deltaTime)
		{
			for (int i = 0, j = m_lateTickSystems.Count; i < j; ++i)
			{
				m_lateTickSystems[i].LateTick(deltaTime);
			}
		}

		public void Clear()
		{
			ClearEntities();
			ClearSystems();
		}
	}
}

