using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1531
{
	// This test is only an Example to show the wrong mapping in the original issue.

	[TestFixture]
	public class SampleTest : BugTestCase
	{
		private void FillDb()
		{
			using (ISession session = OpenSession())
			{
				var entity = new Parent {Id = 1};
				entity.AddNewChild();
				session.Save(entity);

				var entity2 = new Parent {Id = 2};
				session.Save(entity2);

				session.Flush();
			}
		}

		private void CleanDb()
		{
			using (ISession session = OpenSession())
			{
				session.Delete("from Parent");
				session.Flush();
			}
		}

		[Test]
		public void ReparentingShouldNotFail()
		{
			FillDb();
			using (ISession session = OpenSession())
			{
				var parent1 = session.Get<Parent>(1);
				var parent2 = session.Get<Parent>(2);

				Assert.AreEqual(1, parent1.Children.Count);
				Assert.AreEqual(0, parent2.Children.Count);

				Child p1Child = parent1.Children[0];

				Assert.IsNotNull(p1Child);

				parent1.DetachAllChildren();
				parent2.AttachNewChild(p1Child);

				session.SaveOrUpdate(parent1);
				session.SaveOrUpdate(parent2);

				// NHibernate.ObjectDeletedException : 
				// deleted object would be re-saved by cascade (remove deleted object from associations)[NHibernate.Test.NHSpecificTest.NH1531.Child#0]

				session.Flush();
			}

			using (ISession session = OpenSession())
			{
				// should exist only one child
				var l = session.CreateQuery("from Child").List();
				Assert.That(l.Count, Is.EqualTo(1));
			}
			CleanDb();
		}

		[Test]
		public void DeleteParentDeleteChildInCascade()
		{
			FillDb();
			CleanDb();

			// The TestCase is checking the empty db
		}
	}
}