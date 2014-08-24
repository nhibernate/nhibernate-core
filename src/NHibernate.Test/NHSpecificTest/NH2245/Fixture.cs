using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2245
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void TestDelete_OptimisticLockNone()
		{
			Guid id;

			// persist a foo instance
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var f = new Foo();
					f.Name = "Henry";
					f.Description = "description";
					session.Save(f);
					tx.Commit();
					id = f.Id;
				}
			}

			using (ISession session1 = OpenSession())
			using (ISession session2 = OpenSession())
			{
				// Load the foo from two different sessions. Modify the foo in one session to bump the version in the database, and save. 
				// Then try to delete the foo from the other session. With optimistic lock set to none, this should succeed when the 2245
				// patch is applied to AbstractEntityPersister.cs. Without the patch, NH adds the version to the where clause of the delete
				// statement, and the delete fails.
				var f1 = session1.Get<Foo>(id);
				var f2 = session2.Get<Foo>(id);

				// Bump version
				using (ITransaction trans1 = session1.BeginTransaction())
				{
					f1.Description = "modified description";
					session1.Update(f1);
					trans1.Commit();
				}

				// Now delete from second session
				using (ITransaction trans2 = session2.BeginTransaction())
				{
					session2.Executing(s=> s.Delete(f2)).NotThrows();
					trans2.Commit();
				}
			}

			// Assert that row is really gone
			using (ISession assertSession = OpenSession())
			{
				Assert.IsNull(assertSession.Get<Foo>(id));
			}
		}
	}
}