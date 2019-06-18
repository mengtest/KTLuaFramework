using System;
using System.Collections.Generic;

namespace Kernel.core
{
	public interface ISystem
	{

	}

	public abstract class BaseSystem : ISystem
	{
		protected readonly ECSWorld entityManager;

		public BaseSystem(ECSWorld entityManager)
		{
			if (entityManager == null)
			{
				throw new Exception("entityManager can not be null");
			}

			this.entityManager = entityManager;
		}
	}
}

