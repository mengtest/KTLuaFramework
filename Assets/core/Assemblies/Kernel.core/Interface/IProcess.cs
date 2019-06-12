
namespace Kernel.core
{
	public interface IProcess
	{
		void Start();
		void Stop();
		bool IsStart { get; }
		bool IsStop { get; }
	}

	public interface IPause
	{
		void Pause();
		void Resume();
		bool IsPaused { get; }
	}
}



