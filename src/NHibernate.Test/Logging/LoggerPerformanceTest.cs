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
	}
}
