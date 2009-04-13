using System;
using System.Collections;

using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;

using NUnit.Framework;

using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.Futures
{
	public class TestDriver : SqlClientDriver
	{
		public override bool SupportsMultipleQueries
		{
			get { return false; }
		}
	}

	/// <summary>
	/// I've restricted this fixture to MsSql and am using a Driver which derives from SqlClientDriver to
	/// return false for the SupportsMultipleQueries property. This is purely to test the way NHibernate
	/// will behave when the driver that's being used does not support multiple queries... so even though
	/// the test is restricted to MsSql, it's only relevant for databases that don't support multiple queries
	/// but this way it's just much easier to test this
	/// </summary>
	[TestFixture]
	public class FallbackFixture : FutureFixture
	{
		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.Properties[Environment.ConnectionDriver] = "NHibernate.Test.NHSpecificTest.Futures.TestDriver, NHibernate.Test";
			base.Configure(configuration);
		}

		[Test]
		public void CriteriaFallsBackToListImplementationWhenQueryBatchingIsNotSupported()
		{
			using (var session = sessions.OpenSession())
			{
				var results = session.CreateCriteria<Person>().Future<Person>();
				results.GetEnumerator().MoveNext();
			}
		}

		[Test]
		public void QueryFallsBackToListImplementationWhenQueryBatchingIsNotSupported()
		{
			using (var session = sessions.OpenSession())
			{
				var results = session.CreateQuery("from Person").Future<Person>();
				results.GetEnumerator().MoveNext();
			}
		}
	}
}