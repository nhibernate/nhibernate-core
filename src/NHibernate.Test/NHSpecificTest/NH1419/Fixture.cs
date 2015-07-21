using NHibernate;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1419
{
	[TestFixture]
	public class Tests : BugTestCase
	{
		[Test]
		public void Bug()
		{
			using (ISession session = OpenSession())
			{
				ITransaction transaction = session.BeginTransaction();

				Blog blog = new Blog();
				blog.Name = "Test Blog 1";

				Entry entry = new Entry();
				entry.Subject = "Test Entry 1";

				blog.AddEntry(entry);

				session.SaveOrUpdate(blog);

				transaction.Commit();
			}
			using (ISession session = OpenSession())
			{
				ITransaction transaction = session.BeginTransaction();
				session.Delete("from Blog");
				transaction.Commit();
			}
		}

		[Test]
		public void WithEmptyCollection()
		{
			using (ISession session = OpenSession())
			{
				ITransaction transaction = session.BeginTransaction();

				Blog blog = new Blog();
				blog.Name = "Test Blog 1";

				session.SaveOrUpdate(blog);

				transaction.Commit();
			}
			using (ISession session = OpenSession())
			{
				ITransaction transaction = session.BeginTransaction();
				session.Delete("from Blog");
				transaction.Commit();
			}
		}
	}
}
