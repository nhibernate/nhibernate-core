using System;

namespace NHibernate.Example.Web.Infrastructure
{
	public class NHibernateToMicrosoftLoggerFactory : INHibernateLoggerFactory
	{
		private readonly Microsoft.Extensions.Logging.ILoggerFactory _loggerFactory;

		public NHibernateToMicrosoftLoggerFactory(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
		}

		public IInternalLogger2 LoggerFor(string keyName)
		{
			var msLogger = _loggerFactory.CreateLogger(keyName);
			return new NHibernateToMicrosoftLogger(msLogger);
		}

		public IInternalLogger2 LoggerFor(System.Type type)
		{
			return LoggerFor(
				Microsoft.Extensions.Logging.Abstractions.Internal.TypeNameHelper.GetTypeDisplayName(type));
		}
	}
}
