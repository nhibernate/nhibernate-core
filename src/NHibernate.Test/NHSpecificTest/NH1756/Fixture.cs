using System.Collections.Generic;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1756
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void SaveTransient_Then_Update_Ok()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var book = new BookNotGenerated {Name = "test book", Pages = new List<Page>(),};
					session.Save(book);
					book.Name = "modified test book";
					transaction.Commit();
				}
			}
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.CreateQuery("delete from BookNotGenerated").ExecuteUpdate();
					transaction.Commit();
				}
			}
		}

		[Test]
		[Description("Work with AutoFlush on commit")]
		public void SaveTransient_Then_Update()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var book = new Book { Name = "test book", Pages = new List<Page>(), };
					session.Save(book);
					book.Name = "modified test book";
					transaction.Commit();
				}
			}
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.CreateQuery("delete from Book").ExecuteUpdate();
					transaction.Commit();
				}
			}
		}

		[Test]
		public void SaveTransient_Then_Update_Bug()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var book = new Book {Name = "test book", Pages = new List<Page>(),};
					session.Save(book);
					book.Name = "modified test book";
					session.Flush();
					transaction.Commit();
				}
			}
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					session.CreateQuery("delete from Book").ExecuteUpdate();
					transaction.Commit();
				}
			}
		}
	}
}