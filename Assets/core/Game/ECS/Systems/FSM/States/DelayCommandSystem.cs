
using Kernel.core;
using System.Collections.Generic;

namespace Kernel.game
{
	public class DelayCommandSystem : BaseSystem, ITick
	{
		private IList<BitSet> subscribedComponentMasks;
		private BitSet subscribedComponentsMask;

		public DelayCommandSystem(ECSWorld entityManager):base(entityManager)
		{
			InitMaskInfo();
		}

		protected void InitMaskInfo()
		{
			subscribedComponentMasks = new List<BitSet>
			{
				ECSFamily.GetKey<SinglationComponent>(),
				ECSFamily.GetKey<DataComponent>()
			};

			subscribedComponentsMask = subscribedComponentMasks[0] | subscribedComponentMasks[1];
		}

		public void Tick(float deltaTime)
		{
			//获取多个组件
			var result = new List<HashSet<IComponent>>();
			entityManager.FectchAll(subscribedComponentsMask, subscribedComponentMasks, result);

			//获取一个组件

			var result2= new List<IComponent>();
			entityManager.FectchAll(subscribedComponentsMask, result2);
		}
	}
}

