using System;
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
		public void SaveTransient_Then_Update([Range(1, 200)] int attempt)
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					var book = new Book {Name = "test book", Pages = new List<Page>(),};
					session.Save(book);
					Console.WriteLine("Book #{0} saved with version {1}.", book.Id, book.Version.ToString("O"));

					var bookCopy = new Book { Name = "test book", Pages = new List<Page>(), PreviousVersion = book.Version};
					session.Save(bookCopy);


					using (var cmd = session.Connection.CreateCommand())
					{
						cmd.CommandText = "select id, version_column, previousversion_column from book";
						transaction.Enlist(cmd);
						using (var reader = cmd.ExecuteReader())
						{
							Console.WriteLine("Read back from table (id, version, previousversion_column):");
							while (reader.Read())
							{
								Console.WriteLine("{0}    {1:O}    {2:O}", reader.GetValue(0), reader.GetValue(1), reader.GetValue(2));
							}
						}
					}

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
