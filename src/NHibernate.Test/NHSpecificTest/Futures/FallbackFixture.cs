using System;
using System.Collections;

using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Driver;

using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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
			configuration.Properties[Environment.ConnectionDriver] = 
				"NHibernate.Test.NHSpecificTest.Futures.TestDriverThatDoesntSupportQueryBatching, NHibernate.Test";
			base.Configure(configuration);
		}

		protected override void OnTearDown()
		{
			using (var session = sessions.OpenSession())
			{
				session.Delete("from Person");
				session.Flush();
			}

			base.OnTearDown();
		}

		[Test]
		public void FutureOfCriteriaFallsBackToListImplementationWhenQueryBatchingIsNotSupported()
		{
			using (var session = sessions.OpenSession())
			{
				var results = session.CreateCriteria<Person>().Future<Person>();
				results.GetEnumerator().MoveNext();
			}
		}

		[Test]
		public void FutureOfQueryFallsBackToListImplementationWhenQueryBatchingIsNotSupported()
		{
			using (var session = sessions.OpenSession())
			{
				var results = session.CreateQuery("from Person").Future<Person>();
				results.GetEnumerator().MoveNext();
			}
		}

		[Test]
		public void FutureValueOfCriteriaCanGetSingleEntityWhenQueryBatchingIsNotSupported()
		{
			int personId = CreatePerson();

			using (var session = sessions.OpenSession())
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

			using (var session = sessions.OpenSession())
			{
				var futureCount = session.CreateCriteria<Person>()
					.SetProjection(Projections.RowCount())
					.FutureValue<int>();
				Assert.That(futureCount.Value, Is.EqualTo(1));
			}
		}

		[Test]
		public void FutureValueOfQueryCanGetSingleEntityWhenQueryBatchingIsNotSupported()
		{
			int personId = CreatePerson();

			using (var session = sessions.OpenSession())
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

			using (var session = sessions.OpenSession())
			{
				var futureCount = session.CreateQuery("select count(*) from Person")
					.FutureValue<long>();
				Assert.That(futureCount.Value, Is.EqualTo(1L));
			}
		}

		private int CreatePerson()
		{
			using (var session = sessions.OpenSession())
			{
				var person = new Person();
				session.Save(person);
				session.Flush();
				return person.Id;
			}
		}
	}
}