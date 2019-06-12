using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Kernel.core
{
	public class ParallelWork : CommonWork
	{
		protected readonly IList<IWork> works;
		private float total;
		private float current;

		public ParallelWork(params IWork[] works)
		{
			this.works = new IWork[works.Length];
			for (int i = 0,j=works.Length; i <j; i++)
			{
				total += works[i].GetTotal();
				this.works[i] = works[i];
			}
		}

		public ParallelWork(IList<IWork> works)
		{
			this.works = new IWork[works.Count];
			for (int i = 0, j = works.Count; i < j; i++)
			{
				total += works[i].GetTotal();
				this.works[i] = works[i];
			}
		}

		public ParallelWork()
		{
			works = new List<IWork>();
		}

		public virtual void AddWork(IWork work)
		{
			if(work != null)
			{
				works.Add(work);
				total += work.GetTotal();
			}
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

		protected override void OnStart()
		{
			for(int i = 0,j=works.Count; i < j; ++i)
			{
				works[i].Start();
				(works[i] as ITick).Tick(0);//可能会卡很长时间
			}
		}

		protected override void OnTick(float deltaTime)
		{
			float temp = 0;
			for (int i = 0,j=works.Count; i < j; ++i)
			{
				if (!works[i].IsStarted())
				{
					works[i].Start();
				}

				if (works[i].IsPlaying())
				{
					(works[i] as ITick).Tick(deltaTime);
				}

				temp += works[i].GetCurrent();
			}

			current = temp;
		}

		public override void Dispose()
		{
			for (int i = 0; i < works.Count; ++i)
			{
				(works[i] as IDisposable).Dispose();
			}

			works.Clear();
		}
	}
}