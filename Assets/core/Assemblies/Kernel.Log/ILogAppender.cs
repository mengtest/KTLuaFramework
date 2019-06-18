namespace Kernel.Log
{
	/// <summary>
	/// 非线程安全
	/// </summary>
	public interface ILogAppender
	{
		/// <summary>
		/// 非线程安全
		/// </summary>
		/// <param name="ev"></param>
		void Log(LogEvent ev);
	}
}