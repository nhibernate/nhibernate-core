using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1300
{
	// http://jira.nhibernate.org/browse/NH-1300
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void LazyIssueAfterDetach()
		{
			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Parent p = new Parent("parent");
				p.AddChild().Description= "c1";
				p.AddChild().Description = "c2";
				savedId = s.Save(p);
				t.Commit();
			}

			Parent pDetacced;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				pDetacced = s.Get<Parent>(savedId);
				pDetacced.Childs.GetEnumerator(); // force initialization of collection
				t.Commit();
			}

			// now the Parent is detached and its collection was initialized
			// we are going to access the many-to-one relationship.
			foreach (Child child in pDetacced.Childs)
			{
				Assert.IsNotNull(child.Owner);
				Assert.AreEqual("parent", child.Owner.Description);
			}
			
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Parent");
				t.Commit();
			}
		}

		[Test]
		public void LazyIssueAfterDetachProxy()
		{
			object savedId;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				Parent p = new Parent("parent");
				p.AddChild().Description = "c1";
				p.AddChild().Description = "c2";
				savedId = s.Save(p);
				t.Commit();
			}

			Parent pDetacced;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				pDetacced = s.Load<Parent>(savedId);
				pDetacced.Childs.GetEnumerator(); // force initialization of proxy and collection
				t.Commit();
			}

			// now the Parent is detached and its collection was initialized
			// we are going to access the many-to-one relationship.
			foreach (Child child in pDetacced.Childs)
			{
				Assert.IsNotNull(child.Owner);
				Assert.AreEqual("parent", child.Owner.Description);
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete("from Parent");
				t.Commit();
			}
		}

	}
}