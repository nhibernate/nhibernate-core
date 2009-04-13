using System.Collections;

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
			if (((SessionFactoryImpl)sessions)
				.ConnectionProvider.Driver.SupportsMultipleQueries == false)
			{
				Assert.Ignore("Not applicable for dialects that do not support multiple queries");
			}
		}
	}
}