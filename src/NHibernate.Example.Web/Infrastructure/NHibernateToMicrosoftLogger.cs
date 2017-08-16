using System;
using Microsoft.Extensions.Logging;

namespace NHibernate.Example.Web.Infrastructure
{
	public class NHibernateToMicrosoftLogger : IInternalLogger
	{
		private readonly ILogger _msLogger;

		public NHibernateToMicrosoftLogger(ILogger msLogger)
		{
			_msLogger = msLogger ?? throw new ArgumentNullException(nameof(msLogger));
		}

		public void Error(object message)
		{
			if (IsErrorEnabled)
			{
				_msLogger.LogError(message.ToString());
			}
		}

		public void Error(object message, Exception exception)
		{
			if (IsErrorEnabled)
			{
				_msLogger.LogError(exception, message.ToString());
			}
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (IsErrorEnabled)
			{
				_msLogger.LogError(format, args);
			}
		}

		public void Fatal(object message)
		{
			if (IsFatalEnabled)
			{
				_msLogger.LogCritical(message.ToString());
			}
		}

		public void Fatal(object message, Exception exception)
		{
			if (IsFatalEnabled)
			{
				_msLogger.LogCritical(exception, message.ToString());
			}
		}

		public void Debug(object message)
		{
			if (IsDebugEnabled)
			{
				_msLogger.LogDebug(message.ToString());
			}
		}

		public void Debug(object message, Exception exception)
		{
			if (IsDebugEnabled)
			{
				_msLogger.LogDebug(exception, message.ToString());
			}
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (IsDebugEnabled)
			{
				_msLogger.LogDebug(format, args);
			}
		}

		public void Info(object message)
		{
			if (IsInfoEnabled)
			{
				_msLogger.LogInformation(message.ToString());
			}
		}

		public void Info(object message, Exception exception)
		{
			if (IsInfoEnabled)
			{
				_msLogger.LogInformation(exception, message.ToString());
			}
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (IsInfoEnabled)
			{
				_msLogger.LogInformation(format, args);
			}
		}

		public void Warn(object message)
		{
			if (IsWarnEnabled)
			{
				_msLogger.LogWarning(message.ToString());
			}
		}

		public void Warn(object message, Exception exception)
		{
			if (IsWarnEnabled)
			{
				_msLogger.LogWarning(exception, message.ToString());
			}
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (IsWarnEnabled)
			{
				_msLogger.LogWarning(format, args);
			}
		}

		public bool IsErrorEnabled => _msLogger.IsEnabled(LogLevel.Error);
		public bool IsFatalEnabled => _msLogger.IsEnabled(LogLevel.Critical);
		public bool IsDebugEnabled => _msLogger.IsEnabled(LogLevel.Debug);
		public bool IsInfoEnabled => _msLogger.IsEnabled(LogLevel.Information);
		public bool IsWarnEnabled => _msLogger.IsEnabled(LogLevel.Warning);
	}
}