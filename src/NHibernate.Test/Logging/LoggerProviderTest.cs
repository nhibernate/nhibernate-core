using System;
using System.Reflection;
using NUnit.Framework;

namespace NHibernate.Test.Logging
{
	[TestFixture]
	public class LoggerProviderTest
	{
		[Test]
		public void LoggerProviderCanCreateLoggers()
		{
			Assert.That(NHibernateLogger.For("pizza"), Is.Not.Null);
			Assert.That(NHibernateLogger.For(typeof (LoggerProviderTest)), Is.Not.Null);
		}

		[Test, Obsolete]
		public void LoggerProviderCanCreateLoggers_Obsolete()
		{
			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.Not.Null);
			Assert.That(LoggerProvider.LoggerFor(typeof (LoggerProviderTest)), Is.Not.Null);
		}

		[Test]
		public void WhenNotConfiguredAndLog4NetExistsThenUseLog4NetFactory()
		{
			// NoLoggingNHibernateLogger is internal
			Assert.That(NHibernateLogger.For("pizza").GetType().Name, Is.Not.EqualTo("NoLoggingNHibernateLogger"));
		}

		[Test, Obsolete]
		public void WhenNotConfiguredAndLog4NetExistsThenUseLog4NetFactory_Obsolete()
		{
			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.Not.InstanceOf<NoLoggingInternalLogger>());

			// works because this is the legacy provider with a legacy logger
			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.InstanceOf<Log4NetLogger>());
		}

		[Test, Explicit("Changes global state.")]
		public void WhenConfiguredAsNullThenNoLoggingFactoryIsUsed()
		{
			NHibernateLogger.SetLoggersFactory(default(INHibernateLoggerFactory));

			// NoLoggingNHibernateLogger is internal
			Assert.That(NHibernateLogger.For("pizza").GetType().Name, Is.EqualTo("NoLoggingNHibernateLogger"));
		}

		[Test, Explicit("Changes global state."), Obsolete]
		public void WhenConfiguredAsNullThenNoLoggingFactoryIsUsed_Obsolete()
		{
			NHibernateLogger.SetLoggersFactory(default(INHibernateLoggerFactory));

			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.InstanceOf<NoLoggingInternalLogger>());
		}

		[Test, Explicit("Changes global state."), Obsolete]
		public void WhenNoLoggingFactoryIsUsedThenNoLoggingInternalLoggerIsReturned()
		{
			LoggerProvider.SetLoggersFactory(new NoLoggingLoggerFactory());

			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.InstanceOf<NoLoggingInternalLogger>());
		}

		[Test, Explicit("Changes global state."), Obsolete]
		public void WhenNoLoggingFactoryIsUsedThenNoLoggingNHibernateLoggerIsReturned()
		{
			LoggerProvider.SetLoggersFactory(new NoLoggingLoggerFactory());

			// NoLoggingNHibernateLogger is internal
			Assert.That(NHibernateLogger.For("pizza").GetType().Name, Is.EqualTo("NoLoggingNHibernateLogger"));
		}
	}
}
