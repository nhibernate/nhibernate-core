using System.Collections;
using NHibernate.Driver;
using NHibernate.Impl;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.Futures
{
	public abstract class FutureFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "NHSpecificTest.Futures.Mappings.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected void IgnoreThisTestIfMultipleQueriesArentSupportedByDriver()
		{
			var driver = sessions.ConnectionProvider.Driver;
			if (driver.SupportsMultipleQueries == false)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);
		}
	}
}