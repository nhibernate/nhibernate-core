using NUnit.Framework;

namespace NHibernate.Test.Logging
{
	[TestFixture]
	public class LoggerProviderTest
	{
		[Test]
		public void LoggerProviderCanCreateLoggers()
		{
			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.Not.Null);
			Assert.That(LoggerProvider.LoggerFor(typeof (LoggerProviderTest)), Is.Not.Null);
		}

		[Test]
		public void WhenNotConfiguredAndLog4NetExistsThenUseLog4NetFactory()
		{
			Assert.That(LoggerProvider.LoggerFor("pizza"), Is.Not.InstanceOf<NoLoggingInternalLogger>());
		}
	}
}
