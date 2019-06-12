using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace  Kernel.core
{
	public class SimpleWork : CommonWork
	{
		private float total;
		private float current;
		private Action action;

		public SimpleWork(Action action)
		{
			this.action = action;
		}

		public override float GetCurrent()
		{
			if (IsFinished())
				return total;

			return current;
		}

		public override float GetTotal()
		{
			return total;
		}
	}
}
