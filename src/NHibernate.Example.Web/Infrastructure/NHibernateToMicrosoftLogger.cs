using System;
using Microsoft.Extensions.Logging;

namespace NHibernate.Example.Web.Infrastructure
{
	public class NHibernateToMicrosoftLogger : IInternalLogger2
	{
		private readonly ILogger _msLogger;

		public NHibernateToMicrosoftLogger(ILogger msLogger)
		{
			_msLogger = msLogger ?? throw new ArgumentNullException(nameof(msLogger));
		}

		public void Fatal(Exception exception, string format, params object[] args)
		{
			_msLogger.LogCritical(exception, format, args);
		}

		public void Fatal(string format, params object[] args)
		{
			_msLogger.LogCritical(format, args);
		}

		public void Error(Exception exception, string format, params object[] args)
		{
			_msLogger.LogError(exception, format, args);
		}

		public void Error(string format, params object[] args)
		{
			_msLogger.LogError(format, args);
		}

		public void Warn(Exception exception, string format, params object[] args)
		{
			_msLogger.LogWarning(exception, format, args);
		}

		public void Warn(string format, params object[] args)
		{
			_msLogger.LogWarning(format, args);
		}

		public void Info(Exception exception, string format, params object[] args)
		{
			_msLogger.LogInformation(exception, format, args);
		}

		public void Info(string format, params object[] args)
		{
			_msLogger.LogInformation(format, args);
		}

		public void Debug(Exception exception, string format, params object[] args)
		{
			_msLogger.LogDebug(exception, format, args);
		}

		public void Debug(string format, params object[] args)
		{
			_msLogger.LogDebug(format, args);
		}

		public void Fatal(string message, Exception ex)
		{
			throw new NotImplementedException();
		}

		public void Error(string message, Exception ex)
		{
			throw new NotImplementedException();
		}

		public void Warn(string message, Exception ex)
		{
			throw new NotImplementedException();
		}

		public void Info(string message, Exception ex)
		{
			throw new NotImplementedException();
		}

		public void Debug(string message, Exception ex)
		{
			throw new NotImplementedException();
		}

		public bool IsErrorEnabled => _msLogger.IsEnabled(LogLevel.Error);
		public bool IsFatalEnabled => _msLogger.IsEnabled(LogLevel.Critical);
		public bool IsDebugEnabled => _msLogger.IsEnabled(LogLevel.Debug);
		public bool IsInfoEnabled => _msLogger.IsEnabled(LogLevel.Information);
		public bool IsWarnEnabled => _msLogger.IsEnabled(LogLevel.Warning);
	}
}
