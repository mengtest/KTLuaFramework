using System;

namespace Kernel.Log
{
	[Flags]
	public enum LogLevel
	{
		TRACE = 1,
		INFO = 2,
		DEBUG = 4,
		WARN = 8,
		ERROR = 16,
		FATAL = 32,
	}
}