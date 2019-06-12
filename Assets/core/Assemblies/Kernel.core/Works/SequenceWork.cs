using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Kernel.core
{
	public class SequenceWork : CommonWork
	{
		protected readonly IList<IWork> works;
		private float total;
		private float completed;
		private int currIndex;

		public SequenceWork(params IWork[] works)
		{
			this.works = new IWork[works.Length];
			for (int i = 0, j = works.Length; i < j; i++)
			{
				total += works[i].GetTotal();
				this.works[i] = works[i];
			}
		}

		public SequenceWork(IList<IWork> works)
		{
			this.works = new IWork[works.Count];
			for (int i = 0, j = works.Count; i < j; i++)
			{
				total += works[i].GetTotal();
				this.works[i] = works[i];
			}
		}

		public SequenceWork()
		{
			works = new List<IWork>();
		}

		public virtual void AddWork(IWork work)
		{
			if (work != null)
			{
				works.Add(work);
				total += work.GetTotal();
			}
		}

		public override float GetCurrent()
		{
			if (IsFinished())
				return total;

			if (currIndex < works.Count)
				return completed + (works[currIndex].IsStarted() ? works[currIndex].GetCurrent() : 0);

			return total;
		}

		public override float GetTotal()
		{
			return total;
		}

		protected override void OnStart()
		{
			StartCurrentWorks(0);
		}

		protected override void OnTick(float deltaTime)
		{
			if (currIndex < works.Count)
			{
				if (!works[currIndex].IsStarted())
				{
					StartCurrentWorks(deltaTime);
				}
				else if(works[currIndex].IsPlaying())
				{
					(works[currIndex] as ITick).Tick(deltaTime);
					if (works[currIndex].IsFinished())
					{
						completed += works[currIndex].GetTotal();
						++currIndex;
						StartCurrentWorks(deltaTime);
					}
				}
			}
		}

		private void StartCurrentWorks(float deltaTime)
		{
			if (total == 0)
				return;

			DateTime begin = DateTime.Now;
			while (currIndex < works.Count)
			{
				if ((DateTime.Now - begin).TotalSeconds > 0.1f)
					return;

				Assert.IsTrue(!works[currIndex].IsStarted(), "work is already started");
				if (!works[currIndex].IsStarted())
				{
					works[currIndex].Start();
					(works[currIndex] as ITick).Tick(deltaTime);
					if (works[currIndex].IsFinished())
					{
						completed += works[currIndex].GetTotal();
						++currIndex;
					}
					else
					{
						break;
					}
				}
			}
		}

		public override void Dispose()
		{
			for (int i = 0;i<works.Count;++i)
			{
				(works[i] as IDisposable).Dispose();
			}

			works.Clear();
		}
	}
}
