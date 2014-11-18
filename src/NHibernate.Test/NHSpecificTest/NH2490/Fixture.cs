using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2490
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void BadSqlFromJoinLogicError()
		{
			try
			{
				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					Derived item1 = new Derived()
					{
						ShortContent = "Short",
						ShortContent2 = "Short2",
						LongContent = "LongLongLongLongLong",
						LongContent2 = "LongLongLongLongLong2",
					};
					s.Save(item1);
					t.Commit();
				}

				// this is the real meat of the test
				// for most edifying results, run this with show_sql enabled

				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					var q = s.CreateQuery("from Base");
					Assert.That(() => q.List(), Throws.Nothing);
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				using (ITransaction t = s.BeginTransaction())
				{
					s.Delete("from Derived");
					t.Commit();
				}
			}
		}

	}
}
