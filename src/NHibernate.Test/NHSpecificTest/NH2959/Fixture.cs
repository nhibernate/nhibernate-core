using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2959
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
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new DerivedEntity { Name = "Bob" };
				session.Save(e1);

				var e2 = new AnotherDerivedEntity { Name = "Sally" };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");
				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CanUsePolymorphicCriteriaInMultiCriteria()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var results = session.CreateMultiCriteria()
					.Add(session.CreateCriteria(typeof(BaseEntity)))
					.List();

				Assert.That(results, Has.Count.EqualTo(1));
				Assert.That(results[0], Has.Count.EqualTo(2));
			}
		}

		[Test]
		public void CanUsePolymorphicQueryInMultiQuery()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var results = session.CreateMultiQuery()
					.Add(session.CreateQuery("from " + typeof(BaseEntity).FullName))
					.List();

				Assert.That(results, Has.Count.EqualTo(1));
				Assert.That(results[0], Has.Count.EqualTo(2));
			}
		}
	}
}