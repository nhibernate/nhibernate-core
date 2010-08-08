using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Logging
{
	public class LogggerProviderTest
	{
		[Test]
		public void LogggerProviderCanCreateLobbers()
		{
			LogggerProvider.LoggerFor("pizza").Should().Not.Be.Null();
			LogggerProvider.LoggerFor(typeof(LogggerProviderTest)).Should().Not.Be.Null();
		}
	}
}