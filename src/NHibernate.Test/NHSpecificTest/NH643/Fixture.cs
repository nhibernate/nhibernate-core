using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH643
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}

		private object parentId;

		[Test]
		public void CacheAndLazyCollections()
		{
			PrepareData();
			try
			{
				AddChild();
				CheckChildrenCount(1);
				AddChild();
				CheckChildrenCount(2);
			}
			finally
			{
				CleanUp();
			}
		}

		private void PrepareData()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				parentId = session.Save(new Parent());
				tx.Commit();
			}
		}

		private void CleanUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Delete(session.Get<Parent>(parentId));
				tx.Commit();
			}
		}

		private void AddChild()
		{
			using (ISession session = OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				var parent = session.Get<Parent>(parentId);
				Child child = new Child();
				parent.AddChild(child);
				NHibernateUtil.Initialize(parent.Children);
				tx.Commit();
			}
		}

		private void CheckChildrenCount(int count)
		{
			using (ISession session = OpenSession())
			{
				var parent = session.Get<Parent>(parentId);
				Assert.AreEqual(count, parent.Children.Count);
			}
		}
	}
}
