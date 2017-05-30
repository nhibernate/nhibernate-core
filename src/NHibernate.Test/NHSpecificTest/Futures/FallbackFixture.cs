using System.Linq;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Linq;
using NUnit.Framework;

using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.Futures
{
	public class TestDriverThatDoesntSupportQueryBatching : SqlClientDriver
	{
		public override bool SupportsMultipleQueries
		{
			get { return false; }
		}
	}

	/// <summary>
	/// I'm using a Driver which derives from SqlClientDriver to
	/// return false for the SupportsMultipleQueries property. This is purely to test the way NHibernate
	/// will behave when the driver that's being used does not support multiple queries... so even though
	/// the test is using MsSql, it's only relevant for databases that don't support multiple queries
	/// but this way it's just much easier to test this
	/// </summary>
	[TestFixture]
	public class FallbackFixture : FutureFixture
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			var cp = ConnectionProviderFactory.NewConnectionProvider(cfg.Properties);
			return !cp.Driver.SupportsMultipleQueries;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			if (Dialect is MsSql2000Dialect)
			{
				configuration.Properties[Environment.ConnectionDriver] =
					typeof (TestDriverThatDoesntSupportQueryBatching).AssemblyQualifiedName;
			}
		}

		protected override void OnTearDown()
		{
			using (var session = Sfi.OpenSession())
			{
				session.Delete("from Person");
				session.Flush();
			}

			base.OnTearDown();
		}

		[Test]
		public void FutureOfCriteriaFallsBackToListImplementationWhenQueryBatchingIsNotSupported()
		{
			using (var session = Sfi.OpenSession())
			{
				var results = session.CreateCriteria<Person>().Future<Person>();
				results.GetEnumerator().MoveNext();
			}
		}

		[Test]
		public void FutureValueOfCriteriaCanGetSingleEntityWhenQueryBatchingIsNotSupported()
		{
			int personId = CreatePerson();

			using (var session = Sfi.OpenSession())
			{
				var futurePerson = session.CreateCriteria<Person>()
					.Add(Restrictions.Eq("Id", personId))
					.FutureValue<Person>();
				Assert.IsNotNull(futurePerson.Value);
			}
		}

		[Test]
		public void FutureValueOfCriteriaCanGetScalarValueWhenQueryBatchingIsNotSupported()
		{
			CreatePerson();

			using (var session = Sfi.OpenSession())
			{
				var futureCount = session.CreateCriteria<Person>()
					.SetProjection(Projections.RowCount())
					.FutureValue<int>();
				Assert.That(futureCount.Value, Is.EqualTo(1));
			}
		}

		[Test]
		public void FutureOfQueryFallsBackToListImplementationWhenQueryBatchingIsNotSupported()
		{
			using (var session = Sfi.OpenSession())
			{
				var results = session.CreateQuery("from Person").Future<Person>();
				results.GetEnumerator().MoveNext();
			}
		}

		[Test]
		public void FutureValueOfQueryCanGetSingleEntityWhenQueryBatchingIsNotSupported()
		{
			int personId = CreatePerson();

			using (var session = Sfi.OpenSession())
			{
				var futurePerson = session.CreateQuery("from Person where Id = :id")
					.SetInt32("id", personId)
					.FutureValue<Person>();
				Assert.IsNotNull(futurePerson.Value);
			}
		}

		[Test]
		public void FutureValueOfQueryCanGetScalarValueWhenQueryBatchingIsNotSupported()
		{
			CreatePerson();

			using (var session = Sfi.OpenSession())
			{
				var futureCount = session.CreateQuery("select count(*) from Person")
					.FutureValue<long>();
				Assert.That(futureCount.Value, Is.EqualTo(1L));
			}
		}

		[Test]
		public void FutureOfLinqFallsBackToListImplementationWhenQueryBatchingIsNotSupported()
		{
			using (var session = Sfi.OpenSession())
			{
				var results = session.Query<Person>().ToFuture();
				results.GetEnumerator().MoveNext();
			}
		}

		[Test]
		public void FutureValueOfLinqCanGetSingleEntityWhenQueryBatchingIsNotSupported()
		{
			var personId = CreatePerson();

			using (var session = Sfi.OpenSession())
			{
				var futurePerson = session.Query<Person>()
					.Where(x => x.Id == personId)
					.ToFutureValue();
				Assert.IsNotNull(futurePerson.Value);
			}
		}

		private int CreatePerson()
		{
			using (var session = Sfi.OpenSession())
			{
				var person = new Person();
				session.Save(person);
				session.Flush();
				return person.Id;
			}
		}
	}
}