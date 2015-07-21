using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3428
{
	using NHibernate.Criterion;

	[TestFixture]
	public class Fixture : BugTestCase
	{

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Dialect.MsSql2005Dialect;
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity { Name = "Bob" };
				session.Save(e1);

				var e2 = new Entity { Name = "Sally" };
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
		public void QueryFailsWhenDistinctOrderedResultIsPagedPastPageOne()
		{
			using (ISession session = this.OpenSession())
			using (session.BeginTransaction())
			{
				var criteria = session.CreateCriteria<Entity>();
				var projectionList = Projections.ProjectionList().Add(Projections.Property("Name"), "Name");

				criteria.SetProjection(Projections.Distinct(projectionList));

				criteria.SetFirstResult(1).SetMaxResults(1);
				criteria.AddOrder(Order.Asc("Name"));

				var result = criteria.List();

				Assert.AreEqual(1, result.Count);
				Assert.AreEqual("Sally", result[0]);
			}
		}
	}
}