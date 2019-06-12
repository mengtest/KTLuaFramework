using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public interface ISystem
	{
		IList<BitSet> SubscribedComponentMasks { get; }
		BitSet SubscribedComponentsMask { get; }
	}

	public abstract class BaseSystem : ISystem
	{
		protected ECSWorld entityManager;

		public BaseSystem(ECSWorld entityManager)
		{
			if (entityManager == null)
			{
				throw new Exception("entityManager can not be null");
			}

			this.entityManager = entityManager;
		}

		public abstract IList<BitSet> SubscribedComponentMasks { get; }
		public abstract BitSet SubscribedComponentsMask { get; }
	}
}

