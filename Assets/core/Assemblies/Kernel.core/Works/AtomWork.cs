using System;

namespace Kernel.core
{
	public class AtomWork : CommonWork
	{
		private readonly Action work;
		private readonly string label;
		private readonly float weight;

		public AtomWork(Action work, string label = null, float weight = 1)
		{
			this.work = work;
			this.weight = weight;
			this.label = label;
		}

		public override float GetTotal()
		{
			return weight;
		}

		public override float GetCurrent()
		{
			return IsFinished() ? weight : 0;
		}
	}
}
