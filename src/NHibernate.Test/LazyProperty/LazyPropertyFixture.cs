using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.LazyProperty
{
	[TestFixture]
	public class LazyPropertyFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"LazyProperty.Mappings.hbm.xml"}; }
		}

		public void LoadData()
		{
			Book b = new Book
			         	{
			         		Name = "some name",
			         		ALotOfText = "a lot of text ..."
			         	};

			using(var s = OpenSession())
			using(var tx = s.BeginTransaction())
			{
				s.Persist(b);
				tx.Commit();
			}
			
		}

		public void CleanUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				Assert.That(s.CreateSQLQuery("delete from Book").ExecuteUpdate(), Is.EqualTo(1));
				tx.Commit();
			}
		}

		[Test,Ignore("Not supported yet, waiting for a field-interceptor provider, probably Linfu.")]
		public void PropertyLoadedNotInitialized()
		{
			LoadData();

			using(ISession s = OpenSession())
			{
				var book = s.Load<Book>(1);

				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "Id"));
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "Name"));
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"));
				var name = book.Name;
				Assert.True(NHibernateUtil.IsPropertyInitialized(book, "Id"));
				Assert.True(NHibernateUtil.IsPropertyInitialized(book, "Name"));
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"));
			}

			CleanUp();
		}
	}
}