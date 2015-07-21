using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2148
{
	public class BugFixture : BugTestCase
	{
		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Persist(new Book
				{
					Id = 1,
					ALotOfText = "a lot of text ..."
				});
				tx.Commit();
			}

		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				Assert.That(s.CreateSQLQuery("delete from Book").ExecuteUpdate(), Is.EqualTo(1));
				tx.Commit();
			}
		}

		[Test]
		public void CanCallLazyPropertyEntityMethod()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1) as IBook;

				Assert.IsNotNull(book);

				string s1 = "testing1";
				string s2 = book.SomeMethod(s1);
				Assert.AreEqual(s1, s2);
			}
		}
	}
}