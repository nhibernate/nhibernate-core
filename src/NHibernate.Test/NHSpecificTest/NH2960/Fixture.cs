using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2960
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity {Name = "F100"};
				session.Save("FooCode", e1);

				var e2 = new Entity {Name = "B100"};
				session.Save("BarCode", e2);

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
		public void QueryWithExplicitEntityNameOnlyReturnsEntitiesOfSameTypeWithMatchingEntityName()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session
					.CreateQuery("from BarCode")
					.List();
				Assert.AreEqual(1, result.Count);
			}
		}

		[Test]
		public void CriteriaQueryWithExplicitEntityNameOnlyReturnsEntitiesOfSameTypeWithMatchingEntityName()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session
					.CreateCriteria("BarCode")
					.List();
				Assert.AreEqual(1, result.Count);
			}
		}

		[Test]
		public void QueryWithImplicitEntityNameReturnsAllEntitiesOfSameType()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session
					.CreateQuery("from " + typeof(Entity).FullName)
					.List();
				Assert.AreEqual(2, result.Count);
			}
		}

		[Test]
		public void CriteriaQueryWithImplicitEntityNameReturnsAllEntitiesOfSameType()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var result = session
					.CreateCriteria(typeof(Entity))
					.List();
				Assert.AreEqual(2, result.Count);
			}
		}
	}
}