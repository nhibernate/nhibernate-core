using System.Collections;
using NHibernate.Tuple.Entity;
using NUnit.Framework;

namespace NHibernate.Test.LazyProperty
{
	[TestFixture]
	public class LazyPropertyFixture : TestCase
	{
		private string log;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "LazyProperty.Mappings.hbm.xml" }; }
		}

		protected override void BuildSessionFactory()
		{
			using (var logSpy = new LogSpy(typeof(EntityMetamodel)))
			{
				base.BuildSessionFactory();
				log = logSpy.GetWholeLog();
			}
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Persist(new Book
				{
					Name = "some name",
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
		public void PropertyLoadedNotInitialized()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Load<Book>(1);

				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Id"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Name"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False);

				NHibernateUtil.Initialize(book);

				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Id"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Name"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False);
			}
		}

		[Test]
		public void ShouldGenerateErrorForNonAutoPropLazyProp()
		{
			Assert.IsTrue(log.Contains("NHibernate.Test.LazyProperty.Book.ALotOfText is not an auto property, which may result in uninitialized property access"));
		}

		[Test]
		public void PropertyLoadedNotInitializedWhenUsingGet()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1);

				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Id"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Name"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False);
			}
		}

		[Test]
		public void CanGetValueForLazyProperty()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1);

				Assert.That(book.ALotOfText, Is.EqualTo("a lot of text ..."));
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.True);
			}
		}

		[Test]
		public void CanGetValueForNonLazyProperty()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1);

				Assert.That(book.Name, Is.EqualTo("some name"));
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False);
			}
		}

		[Test]
		public void CanLoadAndSaveObjectInDifferentSessions()
		{
			Book book;
			using (ISession s = OpenSession())
			{
				book = s.Get<Book>(1);
			}

			using (ISession s = OpenSession())
			{
				s.Merge(book);
			}
		}


		[Test]
		public void CanUpdateNonLazyWithoutLoadingLazyProperty()
		{
			Book book;
			using (ISession s = OpenSession())
			using (var trans = s.BeginTransaction())
			{
				book = s.Get<Book>(1);
				book.Name += "updated";

				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False);
				trans.Commit();
			}


			using (ISession s = OpenSession())
			{
				book = s.Get<Book>(1);
				Assert.That(book.Name, Is.EqualTo("some nameupdated"));
			}
		}
	}
}