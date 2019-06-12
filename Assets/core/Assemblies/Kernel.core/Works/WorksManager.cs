using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kernel.core
{
	public class WorksManager : Manager<WorksManager>, IPause
	{
		private readonly List<IWork> runningWorks = new List<IWork>();
		private readonly HashSet<IWork> allWorks = new HashSet<IWork>();
		private int runningCount;
		private bool isPause = false;

		public void Pause()
		{
			isPause = true;
		}

		public void Resume()
		{
			isPause = false;
		}

		public bool IsPaused { get{return isPause;} }

		public IEnumerable<object> Iterator(IEnumerable<object> enumerable)
		{
			foreach(var current in enumerable)
			{
				if(current is IEnumerable<object>)
				{
					foreach(var e in Iterator(current as IEnumerable<object>))
					{
						yield return e;
					}
				}
				else
				{
					yield return current;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="work">work的创建和销毁由外界处理</param>
		/// <param name="run"></param>
		/// <returns></returns>
		public void AddWork(IWork work,bool run=false)
		{
			if (work != null && !allWorks.Contains(work))
			{
				allWorks.Add(work);

				if (run)
				{
					if (runningCount < runningWorks.Count)
					{
						runningWorks[runningCount++] = work;
					}
					else
					{
						runningWorks.Add(work);
						runningCount++;
					}

					work.Start();
					(work as ITick).Tick(0);//有些需要立即生效
					if (work.IsFinished())
					{
						allWorks.Remove(work);
						runningWorks.Remove(work);
						runningCount--;
					}
				}
			}
		}

		public void AddWorks(IList<IWork> works, bool run = false)
		{
			if (works == null)
				return;

			for (int i = 0; i < works.Count; i++)
			{
				AddWork(works[i], run);
			}
		}

		protected override void OnTick(float deltaTime)
		{
			if (isPause)
				return;

			int k = 0;
			for (int i = 0; i < runningCount; i++)
			{
				var work = runningWorks[i];
				if (work.IsPlaying())
				{
					(work as ITick).Tick(deltaTime);
				}

				if (work.IsFinished())
				{
					allWorks.Remove(work);
				}
				else
				{
					runningWorks[k] = work;
					k++;
				}
			}
			for (int i = k; i < runningCount; i++)
			{
				runningWorks[i] = null;
			}
			runningCount = k;
		}

	}
}
