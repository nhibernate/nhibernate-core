using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.Futures
{
	public abstract class FutureFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "Futures.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected void IgnoreThisTestIfMultipleQueriesArentSupportedByDriver()
		{
			var driver = Sfi.ConnectionProvider.Driver;
			if (driver.SupportsMultipleQueries == false)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);
		}
	}
}