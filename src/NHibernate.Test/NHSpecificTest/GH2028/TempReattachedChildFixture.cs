using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2028
{
	[TestFixture]
	public class TempReattachedChildFixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void WhenRemoveTempChild_ChildShouldNotInsert()
		{
			Book book = null;
			Word word = null;
			using (var s = OpenSession())
			using (var tr = s.BeginTransaction())
			{
				book = new Book { Id = 1 };
				book.Words = new List<Word> { new Word { Id = 1, Parent = book } };
				s.Save(book);
				tr.Commit();
			}

			word = new Word { Id = 2, Parent = book };
			book.Words.Add(word);

			using (var s = OpenSession())
			using (var tr = s.BeginTransaction())
			{
				s.Update(book);
				book.Words.Remove(word);
				using (var sl = new SqlLogSpy())
				{
					tr.Commit();
					var logs = sl.GetWholeLog();
					Assert.That(logs, Does.Not.Contain("INSERT \n    INTO\n        Word").IgnoreCase, "Сollection record should not be inserted");
				}
			}
		}
	}
}
