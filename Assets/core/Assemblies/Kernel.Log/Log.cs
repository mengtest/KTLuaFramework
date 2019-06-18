using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Kernel.Log
{
    public static class Logger
    {
        /// <summary>
        /// 每个LogSpi都不是线程安全的，需要自己做lock。
        ///     += / -= 都是线程安全的。由于以上两个操作都是直接改变值内容，而引用赋值在C#中是线程安全的，所以我们只需要使用
        ///     logSpi = LogSpi，并判断logSpi不是null的情况，调用就可以了，加delegate的含义是一定不会为null，也就省去了null的判断。
        ///     且保证了线程安全。
        /// </summary>
        public static event Action<LogEvent> LogSpi = delegate
        {
        };

        private static readonly Dictionary<Type, ILogAppender> appenders = new Dictionary<Type, ILogAppender>();

        /// <summary>
        ///     由于C# 3.5中没有ThreadLocal，所以这里使用ThreadStatic，这个只能用于静态成员，满足每个线程一个的要求。
        /// </summary>
        [ThreadStatic]
        private static StringBuilder builder;

        /// <summary>
        ///     由于C# 3.5中没有ThreadLocal，所以这里使用ThreadStatic，这个只能用于静态成员，满足每个线程一个的要求。
        /// </summary>
        [ThreadStatic]
        private static bool logging;

        private static readonly List<ILogAppender> tempAppenders = new List<ILogAppender>();

		private static LogLevel sFilter = LogLevel.TRACE |
											 LogLevel.DEBUG | LogLevel.ERROR | LogLevel.FATAL |
											 LogLevel.INFO | LogLevel.WARN;

		public static void SetLogTopLevel(LogLevel targetLevel)
		{
			var loglevels=Enum.GetValues(typeof(LogLevel));
			for (int i = 0; i < loglevels.Length; i++) {
				var logLevel = (LogLevel)(loglevels.GetValue(i));
				if (logLevel>=targetLevel)
				{
					EnableLogLevel(logLevel);
				}
				else
				{
					DisableLogLevel(logLevel);
				}
			}
		}

		public static bool IsLogLevelEnable(LogLevel level)
		{
			return (sFilter & level)== level;
		}
		public static void EnableLogLevel(LogLevel level)
		{
			sFilter |= level;
		}
		public static void DisableLogLevel(LogLevel level)
		{
			sFilter &= ~level;
		}
		static Logger()
        {
            Enable = true;
        }

        /// <summary>
        ///     是否开启Log，Enable本身是bool，所以get和set是线程安全的。
        /// </summary>
        public static bool Enable
        {
            get;
            set;
        }

        public static void AddAppender<TA>(TA appender) where TA : class, ILogAppender
        {
            lock (appenders)
            {
                appenders[appender.GetType()] = appender;
            }
        }

        public static TA GetAppender<TA>() where TA : class, ILogAppender
        {
            Type type = typeof(TA);
            lock (appenders)
            {
                if (appenders.ContainsKey(type))
                {
                    return appenders[type] as TA;
                }
            }
            return default(TA);
        }

        public static void RemoveAppender<TA>() where TA : class, ILogAppender
        {
            Type type = typeof(TA);
            lock (appenders)
            {
                if (appenders.ContainsKey(type))
                {
                    appenders.Remove(type);
                }
            }
        }

        public static void Fatal(string msg)
        {
			if (!IsLogLevelEnable(LogLevel.FATAL)) return;
            Log(LogLevel.FATAL, typeof(Logger), msg, null);
        }

        public static void Fatal(string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.FATAL)) return;
			Log(LogLevel.FATAL, typeof(Logger), msg, e);
        }

        public static void Fatal(string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.FATAL)) return;
			Log(LogLevel.FATAL, typeof(Logger), string.Format(msg, args), null);
        }

        public static void Error(string msg)
        {
			if (!IsLogLevelEnable(LogLevel.ERROR)) return;
			Log(LogLevel.ERROR, typeof(Logger), msg, null);
        }

        public static void Error(string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.ERROR)) return;
			Log(LogLevel.ERROR, typeof(Logger), msg, e);
        }

        public static void Error(string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.ERROR)) return;
			Log(LogLevel.ERROR, typeof(Logger), string.Format(msg, args), null);
        }

        public static void Warn(string msg)
        {
			if (!IsLogLevelEnable(LogLevel.WARN)) return;
			Log(LogLevel.WARN, typeof(Logger), msg, null);
        }

        public static void GetStackTraceModelName()
        {
            //当前堆栈信息
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.StackFrame[] sfs = st.GetFrames();
            //过虑的方法名称,以下方法将不会出现在返回的方法调用列表中
            string _filterdName = "ResponseWrite,ResponseWriteError,";
            StringBuilder sb = new StringBuilder();
	        sb.AppendLine("-----------------------Start Stack Trace:----------------------");
            for (int i = 1; i < sfs.Length; ++i)
            {
                //非用户代码,系统方法及后面的都是系统调用，不获取用户代码调用结束
                if (System.Diagnostics.StackFrame.OFFSET_UNKNOWN == sfs[i].GetILOffset()) break;
                var _methodName = sfs[i].GetMethod().Name;//方法名称

                //sfs[i].GetFileLineNumber();//没有PDB文件的情况下将始终返回0
                if (_filterdName.Contains(_methodName)) continue;
	            sb.AppendLine(sfs[i].GetMethod().DeclaringType.Name + ":" + _methodName);
            }
	        sb.AppendLine("-----------------------End Stack Trace:----------------------");
            Log(LogLevel.WARN, typeof(Logger), sb.ToString(), null);
        }



        public static void Warn(string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.WARN)) return;
			Log(LogLevel.WARN, typeof(Logger), msg, e);
        }

        public static void Warn(string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.WARN)) return;
			Log(LogLevel.WARN, typeof(Logger), string.Format(msg, args), null);
        }

        public static void Info(string msg)
        {
			if (!IsLogLevelEnable(LogLevel.INFO)) return;
			Log(LogLevel.INFO, typeof(Logger), msg, null);
        }

        public static void Info(string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.INFO)) return;
			Log(LogLevel.INFO, typeof(Logger), msg, e);
        }

        public static void Info(string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.INFO)) return;
			Log(LogLevel.INFO, typeof(Logger), string.Format(msg, args), null);
        }

        public static void Debug(string msg)
        {
			if (!IsLogLevelEnable(LogLevel.DEBUG)) return;
			Log(LogLevel.DEBUG, typeof(Logger), msg, null);
        }

        public static void Debug(string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.DEBUG)) return;
			Log(LogLevel.DEBUG, typeof(Logger), msg, e);
        }

        public static void Debug(string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.DEBUG)) return;
			Log(LogLevel.DEBUG, typeof(Logger), string.Format(msg, args), null);
        }

        public static void Trace(string msg)
        {
			if (!IsLogLevelEnable(LogLevel.TRACE)) return;
			Log(LogLevel.TRACE, typeof(Logger), msg, null);
        }

        public static void Trace(string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.TRACE)) return;
			Log(LogLevel.TRACE, typeof(Logger), msg, e);
        }

        public static void Trace(string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.TRACE)) return;
			Log(LogLevel.TRACE, typeof(Logger), string.Format(msg, args), null);
        }

        public static void Fatal(Type type, string msg)
        {
			if (!IsLogLevelEnable(LogLevel.FATAL)) return;
			Log(LogLevel.FATAL, type, msg, null);
        }

        public static void Fatal(Type type, string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.FATAL)) return;
			Log(LogLevel.FATAL, type, msg, e);
        }

        public static void Fatal(Type type, string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.FATAL)) return;
			Log(LogLevel.FATAL, type, string.Format(msg, args), null);
        }

        public static void Error(Type type, string msg)
        {
			if (!IsLogLevelEnable(LogLevel.ERROR)) return;
			Log(LogLevel.ERROR, type, msg, null);
        }

        public static void Error(Type type, string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.ERROR)) return;
			Log(LogLevel.ERROR, type, msg, e);
        }

        public static void Error(Type type, string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.ERROR)) return;
			Log(LogLevel.ERROR, type, string.Format(msg, args), null);
        }

        public static void Warn(Type type, string msg)
        {
			if (!IsLogLevelEnable(LogLevel.WARN)) return;
			Log(LogLevel.WARN, type, msg, null);
        }

        public static void Warn(Type type, string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.WARN)) return;
			Log(LogLevel.WARN, type, msg, e);
        }

        public static void Warn(Type type, string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.WARN)) return;
			Log(LogLevel.WARN, type, string.Format(msg, args), null);
        }

        public static void Info(Type type, string msg)
        {
			if (!IsLogLevelEnable(LogLevel.INFO)) return;
			Log(LogLevel.INFO, type, msg, null);
        }

        public static void Info(Type type, string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.INFO)) return;
			Log(LogLevel.INFO, type, msg, e);
        }

        public static void Info(Type type, string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.INFO)) return;
			Log(LogLevel.INFO, type, string.Format(msg, args), null);
        }

        public static void Debug(Type type, string msg)
        {
			if (!IsLogLevelEnable(LogLevel.DEBUG)) return;
			Log(LogLevel.DEBUG, type, msg, null);
        }

        public static void Debug(Type type, string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.DEBUG)) return;
			Log(LogLevel.DEBUG, type, msg, e);
        }

        public static void Debug(Type type, string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.DEBUG)) return;
			Log(LogLevel.DEBUG, type, string.Format(msg, args), null);
        }

        public static void Trace(Type type, string msg)
        {
			if (!IsLogLevelEnable(LogLevel.TRACE)) return;
			Log(LogLevel.TRACE, type, msg, null);
        }

        public static void Trace(Type type, string msg, Exception e)
        {
			if (!IsLogLevelEnable(LogLevel.TRACE)) return;
			Log(LogLevel.TRACE, type, msg, e);
        }

        public static void Trace(Type type, string msg, params object[] args)
        {
			if (!IsLogLevelEnable(LogLevel.TRACE)) return;
			Log(LogLevel.TRACE, type, string.Format(msg, args), null);
        }

        public static string GetStack(int skipFrames = 1)
        {
            return GetStack(new StackTrace(skipFrames, true));
        }

        public static string GetStack(Exception e)
        {
			return GetStack(new StackTrace(e, true));
        }

        private static string GetStack(StackTrace st)
        {
            if (st == null)
            {
                return "";
            }
            try
            {
                var sb = GetThreadLocalStringBuilder(512);

                for (var i = 0; i < st.FrameCount; ++i)
                {
                    if (i != 0)
                    {
                        sb.AppendFormat("\n");
                    }

                    var sf = st.GetFrame(i);
                    if (sf == null)
                    {
                        continue;
                    }

                    var method = sf.GetMethod();
                    if (method == null)
                    {
                        sb.Append(" unknown method");
                    }
                    else
                    {
                        sb.Append(" ");
                        if (method.ReflectedType != null)
                        {
                            sb.AppendFormat("{0}:{1}", method.ReflectedType.Name, method.Name);
                        }
                        else
                        {
                            sb.Append(method.Name);
                        }

                        sb.Append("(");
                        foreach (var param in method.GetParameters())
                        {
                            if (param.Position != 0)
                            {
                                sb.Append(", ");
                            }
                            if (param.IsOut)
                            {
                                sb.Append("out ");
                            }
                            sb.Append(param.ParameterType.Name);
                        }
                        sb.Append(")");
                    }

                    var filename = sf.GetFileName();
                    if (filename != null)
                    {
                        sb.AppendFormat(" in {0}:{1}", filename, sf.GetFileLineNumber());
                    }
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private static void Log(LogLevel level, Type type, string msg, Exception e)
        {
            if (!Enable)
            {
                return;
            }
            if (logging)
            {
                return;
            }

            try
            {
                logging = true;
                var ev = new LogEvent
                {
                    DateTime = DateTime.Now,
                    Thread = Thread.CurrentThread.ManagedThreadId,
                    Level = level,
                    Type = type,
                    Message = msg,
                    Exception = e
                };
                LogSpi(ev);
                lock (appenders)
                {
                    tempAppenders.AddRange(appenders.Values);
                    foreach (var appender in tempAppenders)
                    {
                        appender.Log(ev);
                    }
                    tempAppenders.Clear();
                }
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                logging = false;
            }
        }

        private static StringBuilder GetThreadLocalStringBuilder(int capacity)
        {
            if (builder == null)
            {
                builder = new StringBuilder(capacity);
            }
            else
            {
                builder.Length = 0;
            }
            return builder;
        }
    }
}