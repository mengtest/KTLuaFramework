using System;

namespace Kernel.Log
{
	public struct LogEvent
	{
		public DateTime DateTime;
		public int Thread;
		public LogLevel Level;
		public Type Type;
		public string Message;
		public Exception Exception;

		public override string ToString()
		{
			return $"{DateTime:yyyy-MM-dd HH:mm:ss.fff} {Level,-5} [{Thread,3:000}] {Type,10} {Message}{(Exception == null ? "" : $" {Exception.GetType().FullName} {Exception.Message} {Exception.StackTrace}")}";
		}
	}
}