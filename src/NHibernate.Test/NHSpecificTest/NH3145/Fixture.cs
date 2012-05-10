using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3145
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		[Ignore("Not fixed yet.")]
		public void QueryWithLazyBaseClassShouldNotThrowNoPersisterForError()
		{
			try
			{
				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var item1 = new Derived
					{
						LongContent = "LongLongLongLongLong"
					};
					var root = new Root
					{
						Base = item1
					};
					s.Save(item1);
					s.Save(root);
					t.Commit();
				}

				// This will succeed if either:
				// a) we do not initialize root.Base
				// or
				// b) Base.LongContent is made non-lazy (remove lazy properties)

				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var root = s.CreateQuery("from Root").UniqueResult<Root>();
					NHibernateUtil.Initialize(root.Base);
					var q = s.CreateQuery("from Derived d where d = ?")
						.SetEntity(0, root.Base);
					q.Executing(query => query.List()).NotThrows();
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					s.Delete("from Root");
					s.Delete("from Derived");
					t.Commit();
				}
			}
		}
	}
}
