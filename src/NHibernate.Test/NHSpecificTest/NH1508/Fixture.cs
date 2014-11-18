using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1508
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver.SupportsMultipleQueries;
		}

		protected override void OnSetUp()
		{
			var john = new Person();
			john.Name = "John";

			var doc1 = new Document();
			doc1.Person = john;
			doc1.Title = "John's Doc";

			var doc2 = new Document();
			doc2.Title = "Spec";
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Save(john);
				session.Save(doc1);
				session.Save(doc2);

				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var tx = session.BeginTransaction())
			{
				session.Delete("from Person");
				session.Delete("from Document");
				tx.Commit();
			}
		}

		[Test]
		public void DoesntThrowExceptionWhenHqlQueryIsGiven()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var sqlQuery = session.CreateQuery("from Document");
				var q = session
					.CreateMultiQuery()
					.Add(sqlQuery);
				q.List();
			}
		}

		[Test]
		public void DoesntThrowsExceptionWhenSqlQueryIsGiven()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var sqlQuery = session.CreateSQLQuery("select * from Document");
				var multiquery = session.CreateMultiQuery();
				Assert.That(() => multiquery.Add(sqlQuery), Throws.Nothing);
			}
		}

		[Test]
		public void DoesntThrowsExceptionWhenNamedSqlQueryIsGiven()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{

				var multiquery = session.CreateMultiQuery();
				Assert.That(() => multiquery.AddNamedQuery("SampleSqlQuery"), Throws.Nothing);
			}
		}
	}
}
