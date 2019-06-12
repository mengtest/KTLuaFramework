using System;

namespace Kernel.core
{
	public enum WorkState
	{
		WAITING,
		PLAYING,
		FINISHED
	}

	public abstract class CommonWork :IWork,ITick,IDisposable
	{
		private Action onFinish;
		private Action onStart;
		private Action<float> onTick;
		private Action<Exception> exceptionHandler;
		private WorkState state = WorkState.WAITING;

		public WorkState State
		{
			get
			{
				return state;
			}
			protected set
			{
				state = value;
			}
		}

		public void Start()
		{
			state = WorkState.WAITING;
			OnStart();
			onStart?.Invoke();
		}

		public void Finish()
		{
			state = WorkState.FINISHED;
			OnFinished();
			onFinish?.Invoke();
		}

		public bool IsFinished()
		{
			return state == WorkState.FINISHED;
		}

		public bool IsStarted()
		{
			return state > WorkState.WAITING;
		}

		public bool IsPlaying()
		{
			return state == WorkState.PLAYING;
		}

		public virtual float GetTotal()
		{
			return 1;
		}

		public virtual float GetCurrent()
		{
			return 0;
		}

		public float GetProgress()
		{
			if (State == WorkState.PLAYING)
			{
				var total = GetTotal();
				return total > 0 ? GetCurrent() / total : 0;
			}
			if (State < WorkState.WAITING)
			{
				return 0;
			}
			return 1;
		}

		public void Tick(float deltaTime)
		{
			try
			{
				if (state != WorkState.PLAYING)
					return;

				OnTick(deltaTime);
				onTick?.Invoke(deltaTime);

				if (GetCurrent() >= GetTotal())
				{
					state = WorkState.FINISHED;
					OnFinished();
					onFinish?.Invoke();
				}
			}
			catch(Exception e)
			{
				if(exceptionHandler != null)
				{
					exceptionHandler(e);
				}
				else
				{
					throw;
				}
			}
		}

		protected virtual void OnStart()
		{
			
		}

		protected virtual void OnFinished()
		{
			
		}

		protected virtual void OnTick(float deltaTime)
		{

		}

		public void AddOnStart(Action startAction)
		{
			onStart += startAction;
		}

		public void AddOnFinish(Action finishAction)
		{
			onFinish += finishAction;
		}

		public void AddOnTick(Action<float> tickAction)
		{
			onTick += tickAction;
		}

		public void AddExceptionHandler(Action<Exception> handler)
		{
#if !CODE_GEN
			exceptionHandler += handler;
#endif
		}

		public virtual  void Dispose()
		{
			onStart = null;
			onFinish = null;
			onTick = null;
			exceptionHandler = null;
		}
	}

	public class EmptyWork : CommonWork
	{
		public static EmptyWork Empty = new EmptyWork();
	}
}