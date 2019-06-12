using System;

namespace Kernel.core
{
	public interface IWork
	{
		void Start();
		void Finish();
		bool IsStarted();
		bool IsPlaying();
		bool IsFinished();
		float GetTotal();
		float GetCurrent();
		float GetProgress();
		void AddOnStart(Action onStart);
		void AddOnFinish(Action onFinish);
		void AddExceptionHandler(Action<Exception> handler);
	}
}
