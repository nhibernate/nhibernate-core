using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NHibernate.Test.Logging
{
	[TestFixture]
	public class LoggerPerformanceTest
	{
		private static int errorLoggedCount = 0;
		private static int fatalLoggedCount = 0;
		private static int debugLoggedCount = 0;
		private static int infoLoggedCount = 0;
		private static int warnLoggedCount = 0;

		[Obsolete("Tests obsolete methods")]
		private class MockLoggerFactory : ILoggerFactory
		{
			public IInternalLogger LoggerFor(string keyName)
			{
				return new MockLogger();
			}

			public IInternalLogger LoggerFor(System.Type type)
			{
				return new MockLogger();
			}
		}

		[Obsolete("Tests obsolete methods")]
		private class MockLogger : IInternalLogger
		{
			public bool IsErrorEnabled { get; } = true;
			public bool IsFatalEnabled { get; } = true;
			public bool IsDebugEnabled { get; } = false;
			public bool IsInfoEnabled { get; } = false;
			public bool IsWarnEnabled { get; } = true;

			public void Error(object message)
			{
				if (IsErrorEnabled) errorLoggedCount += message.ToString().Length;
			}

			public void Error(object message, Exception exception)
			{
				if (IsErrorEnabled) errorLoggedCount += message.ToString().Length;
			}

			public void ErrorFormat(string format, params object[] args)
			{
				if (IsErrorEnabled) errorLoggedCount += string.Format(format, args).Length;
			}

			public void Fatal(object message)
			{
				if (IsFatalEnabled) fatalLoggedCount += message.ToString().Length;
			}

			public void Fatal(object message, Exception exception)
			{
				if (IsFatalEnabled) fatalLoggedCount += message.ToString().Length;
			}

			public void Debug(object message)
			{
				if (IsDebugEnabled) debugLoggedCount += message.ToString().Length;
			}

			public void Debug(object message, Exception exception)
			{
				if (IsDebugEnabled) debugLoggedCount += message.ToString().Length;
			}

			public void DebugFormat(string format, params object[] args)
			{
				if (IsDebugEnabled) debugLoggedCount += string.Format(format, args).Length;
			}

			public void Info(object message)
			{
				if (IsInfoEnabled) infoLoggedCount += message.ToString().Length;
			}

			public void Info(object message, Exception exception)
			{
				if (IsInfoEnabled) infoLoggedCount += message.ToString().Length;
			}

			public void InfoFormat(string format, params object[] args)
			{
				if (IsInfoEnabled) infoLoggedCount += string.Format(format, args).Length;
			}

			public void Warn(object message)
			{
				if (IsWarnEnabled) warnLoggedCount += message.ToString().Length;
			}

			public void Warn(object message, Exception exception)
			{
				if (IsWarnEnabled) warnLoggedCount += message.ToString().Length;
			}

			public void WarnFormat(string format, params object[] args)
			{
				if (IsWarnEnabled) warnLoggedCount += string.Format(format, args).Length;
			}
		}

		private class MockNHibernateLoggerFactory : INHibernateLoggerFactory
		{
			public INHibernateLogger LoggerFor(string keyName)
			{
				return new MockLogger2();
			}

			public INHibernateLogger LoggerFor(System.Type type)
			{
				return new MockLogger2();
			}
		}

		private class MockLogger2 : INHibernateLogger
		{
			private bool IsErrorEnabled { get; } = true;
			private bool IsFatalEnabled { get; } = true;
			private bool IsDebugEnabled { get; } = false;
			private bool IsInfoEnabled { get; } = false;
			private bool IsWarnEnabled { get; } = true;

			public void Log(NHibernateLogLevel logLevel, NHibernateLogValues state, Exception exception)
			{
				if (!IsEnabled(logLevel)) return;

				if (state.Args?.Length > 0)
				{
					errorLoggedCount += string.Format(state.Format, state.Args).Length;
				}
				else
				{
					errorLoggedCount += state.Format.Length;
				}
			}

			public bool IsEnabled(NHibernateLogLevel logLevel)
			{
				switch (logLevel)
				{
					case NHibernateLogLevel.Trace:
					case NHibernateLogLevel.Debug:
						return IsDebugEnabled;
					case NHibernateLogLevel.Info:
						return IsInfoEnabled;
					case NHibernateLogLevel.Warn:
						return IsWarnEnabled;
					case NHibernateLogLevel.Error:
						return IsErrorEnabled;
					case NHibernateLogLevel.Fatal:
						return IsFatalEnabled;
					case NHibernateLogLevel.None:
						return !IsFatalEnabled;
					default:
						throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
				}
			}
		}

		private static void ResetCounts()
		{
			errorLoggedCount = 0;
			fatalLoggedCount = 0;
			debugLoggedCount = 0;
			infoLoggedCount = 0;
			warnLoggedCount = 0;
		}

		private static long GetCounts()
		{
			return (long) errorLoggedCount + fatalLoggedCount + debugLoggedCount + infoLoggedCount + warnLoggedCount;
		}

		[Test, Explicit("High-iteration performance test")]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Obsolete("Tests obsolete methods")]
		public void OldLoggerFactoryTimingsForDisabledLogging(int iteration)
		{
			ResetCounts();
			ILoggerFactory loggerFactory = new MockLoggerFactory();
			IInternalLogger logger = loggerFactory.LoggerFor(this.GetType());

			var stopwatch = Stopwatch.StartNew();
			var iterationCount = 10000000;
			for (int i = 0; i < iterationCount; i++)
			{
				logger.Debug("message");
				logger.DebugFormat("message with parameters {0}, {1}", "string", 5);
			}

			stopwatch.Stop();
			Console.WriteLine(
				"{0} wrote {1:N0} characters to log in {2} ms",
				nameof(OldLoggerFactoryTimingsForDisabledLogging),
				GetCounts(),
				stopwatch.ElapsedMilliseconds);
		}

		[Test, Explicit("High-iteration performance test")]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Obsolete("Tests obsolete methods")]
		public void OldLoggerFactoryTimingsForEnabledLogging(int iteration)
		{
			ResetCounts();
			ILoggerFactory loggerFactory = new MockLoggerFactory();
			IInternalLogger logger = loggerFactory.LoggerFor(this.GetType());

			var stopwatch = Stopwatch.StartNew();
			var iterationCount = 10000000;
			for (int i = 0; i < iterationCount; i++)
			{
				logger.Warn("message");
				logger.WarnFormat("message with parameters {0}, {1}", "string", 5);
			}

			stopwatch.Stop();
			Console.WriteLine(
				"{0} wrote {1:N0} characters to log in {2} ms",
				nameof(OldLoggerFactoryTimingsForEnabledLogging),
				GetCounts(),
				stopwatch.ElapsedMilliseconds);
		}

		[Test, Explicit("High-iteration performance test")]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Obsolete("Tests obsolete methods")]
		public void OldLoggerFactoryThunkedTimingsForDisabledLogging(int iteration)
		{
			ResetCounts();
			ILoggerFactory loggerFactory = new MockLoggerFactory();
			LoggerProvider.SetLoggersFactory(loggerFactory);
			INHibernateLogger logger2 = NHibernateLogger.For(this.GetType());

			var stopwatch = Stopwatch.StartNew();
			var iterationCount = 10000000;
			for (int i = 0; i < iterationCount; i++)
			{
				logger2.Debug("message");
				logger2.Debug("message with parameters {0}, {1}", "string", 5);
			}

			stopwatch.Stop();
			Console.WriteLine(
				"{0} wrote {1:N0} characters to log in {2} ms",
				nameof(OldLoggerFactoryThunkedTimingsForDisabledLogging),
				GetCounts(),
				stopwatch.ElapsedMilliseconds);
		}

		[Test, Explicit("High-iteration performance test")]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Obsolete("Tests obsolete methods")]
		public void OldLoggerFactoryThunkedTimingsForEnabledLogging(int iteration)
		{
			ResetCounts();
			ILoggerFactory loggerFactory = new MockLoggerFactory();
			LoggerProvider.SetLoggersFactory(loggerFactory);
			INHibernateLogger logger2 = NHibernateLogger.For(this.GetType());

			var stopwatch = Stopwatch.StartNew();
			var iterationCount = 10000000;
			for (int i = 0; i < iterationCount; i++)
			{
				logger2.Warn("message");
				logger2.Warn("message with parameters {0}, {1}", "string", 5);
			}

			stopwatch.Stop();
			Console.WriteLine(
				"{0} wrote {1:N0} characters to log in {2} ms",
				nameof(OldLoggerFactoryThunkedTimingsForEnabledLogging),
				GetCounts(),
				stopwatch.ElapsedMilliseconds);
		}

		[Test, Explicit("High-iteration performance test")]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Obsolete("Tests obsolete methods")]
		public void NewLoggerFactoryTimingsForDisabledLogging(int iteration)
		{
			ResetCounts();
			INHibernateLoggerFactory loggerFactory = new MockNHibernateLoggerFactory();
			NHibernateLogger.SetLoggersFactory(loggerFactory);
			INHibernateLogger logger2 = NHibernateLogger.For(this.GetType());

			var stopwatch = Stopwatch.StartNew();
			var iterationCount = 10000000;
			for (int i = 0; i < iterationCount; i++)
			{
				logger2.Debug("message");
				logger2.Debug("message with parameters {0}, {1}", "string", 5);
			}

			stopwatch.Stop();
			Console.WriteLine(
				"{0} wrote {1:N0} characters to log in {2} ms",
				nameof(NewLoggerFactoryTimingsForDisabledLogging),
				GetCounts(),
				stopwatch.ElapsedMilliseconds);
		}

		[Test, Explicit("High-iteration performance test")]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Obsolete("Tests obsolete methods")]
		public void NewLoggerFactoryTimingsForEnabledLogging(int iteration)
		{
			ResetCounts();
			INHibernateLoggerFactory loggerFactory = new MockNHibernateLoggerFactory();
			NHibernateLogger.SetLoggersFactory(loggerFactory);
			INHibernateLogger logger2 = NHibernateLogger.For(this.GetType());

			var stopwatch = Stopwatch.StartNew();
			var iterationCount = 10000000;
			for (int i = 0; i < iterationCount; i++)
			{
				logger2.Warn("message");
				logger2.Warn("message with parameters {0}, {1}", "string", 5);
			}

			stopwatch.Stop();
			Console.WriteLine(
				"{0} wrote {1:N0} characters to log in {2} ms",
				nameof(NewLoggerFactoryTimingsForEnabledLogging),
				GetCounts(),
				stopwatch.ElapsedMilliseconds);
		}

		[Test, Explicit("High-iteration performance test")]
		[TestCase(1)]
		[TestCase(2)]
		[TestCase(3)]
		[TestCase(4)]
		[TestCase(5)]
		[Obsolete("Tests obsolete methods")]
		public void NewLoggerFactoryTimingsForNoLogging(int iteration)
		{
			ResetCounts();
			NHibernateLogger.SetLoggersFactory((INHibernateLoggerFactory) null);
			INHibernateLogger logger2 = NHibernateLogger.For(this.GetType());

			var stopwatch = Stopwatch.StartNew();
			var iterationCount = 10000000;
			for (int i = 0; i < iterationCount; i++)
			{
				logger2.Debug("message");
				logger2.Debug("message with parameters {0}, {1}", "string", 5);
			}

			stopwatch.Stop();
			Console.WriteLine(
				"{0} wrote {1:N0} characters to log in {2} ms",
				nameof(NewLoggerFactoryTimingsForDisabledLogging),
				GetCounts(),
				stopwatch.ElapsedMilliseconds);
		}
	}
}
