using System.Collections;
using System.Collections.Generic;
using Kernel.core;
using UnityEngine;

namespace Kernel.game
{
	public struct ComponentData
	{
		
	}

	public class DataComponent : IComponent, IComponentData<ComponentData>
	{
		private ComponentData data;

		public ComponentData Data
		{
			get { return data; }
			set { data = value; }
		}
	}
}
