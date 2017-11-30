using NUnit.Framework;

namespace NHibernate.Test.Logging
{
	[TestFixture]
	public class LoggerProviderTest
	{
		[Test]
		public void LoggerProviderCanCreateLoggers()
		{
			Assert.That(LoggerProvider.For("pizza"), Is.Not.Null);
			Assert.That(LoggerProvider.For(typeof (LoggerProviderTest)), Is.Not.Null);
		}

		[Test]
		public void WhenNotConfiguredAndLog4NetExistsThenUseLog4NetFactory()
		{
#pragma warning disable 618
			Assert.That(LoggerProvider.For("pizza"), Is.Not.InstanceOf<NoLoggingInternalLogger>());
#pragma warning restore 618

			// NoLoggingNHibernateLogger is internal
			Assert.That(LoggerProvider.For("pizza").GetType().Name, Is.Not.EqualTo("NoLoggingNHibernateLogger"));
		}

		[Test, Explicit("Changes global state.")]
		public void WhenConfiguredAsNullThenNoLoggingFactoryIsUsed()
		{
			LoggerProvider.SetLoggersFactory(default(INHibernateLoggerFactory));

			// NoLoggingNHibernateLogger is internal
			Assert.That(LoggerProvider.For("pizza").GetType().Name, Is.EqualTo("NoLoggingNHibernateLogger"));
		}
	}
}
