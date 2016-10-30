using System;
using System.Collections.Generic;
using System.Data.Odbc;
using NHibernate.Dialect;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH1756
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.ShowSql, "true");
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

					if (session.Connection is OdbcConnection)
					{
						using (var cmd = session.Connection.CreateCommand())
						{
							transaction.Enlist(cmd);
							cmd.CommandText = "select id, version_column, previousversion_column, (case when version_column = ? then 1 else 0 end) as versionIsEqual, ? as sentVersion, cast (version_column as datetime2) as vcdt2, cast (? as datetime2) as svdt2 from book";
							var param = cmd.CreateParameter();
							param.Value = book.Version;
							param.Scale = 3;
							cmd.Parameters.Add(param);
							param = cmd.CreateParameter();
							param.Value = book.Version;
							param.Scale = 3;
							cmd.Parameters.Add(param);
							param = cmd.CreateParameter();
							param.Value = book.Version;
							param.Scale = 3;
							cmd.Parameters.Add(param);

							using (var reader = cmd.ExecuteReader())
							{
								Console.WriteLine("Read back from table (id, version, previousversion_column, versionIsEqual, sentVersion, version as datetime2, sentVersion as datetime2):");
								while (reader.Read())
								{
									Console.WriteLine(
										"{0}    {1:O} ({2})    {3:O} ({4})    {5}    {6:O} ({7})    {8:O} ({9})    {10:O} ({11})",
										reader.GetValue(0),
										reader.GetValue(1),
										reader.GetDateTime(1).Ticks,
										reader.GetValue(2),
										reader.GetDateTime(2).Ticks,
										reader.GetValue(3),
										reader.GetValue(4),
										reader.GetDateTime(4).Ticks,
										reader.GetValue(5),
										reader.GetDateTime(5).Ticks,
										reader.GetValue(6),
										reader.GetDateTime(6).Ticks);
								}
							}
						}
					}


					var readBooks = session.CreateQuery("from Book b where b.Version = :v")
										   .SetParameter("v", book.Version, NHibernateUtil.Timestamp)
										   .List<Book>();
					Console.WriteLine("NH read books: {0}", readBooks.Count);

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
