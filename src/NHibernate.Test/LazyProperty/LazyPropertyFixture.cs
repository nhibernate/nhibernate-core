using System.Collections;
using NHibernate.Intercept;
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

		protected override DebugSessionFactory BuildSessionFactory()
		{
			using (var logSpy = new LogSpy(typeof(EntityMetamodel)))
			{
				var factory = base.BuildSessionFactory();
				log = logSpy.GetWholeLog();
				return factory;
			}
		}

		protected override void OnSetUp()
		{
			Assert.That(
				nameof(Book.FieldInterceptor),
				Is.EqualTo(nameof(IFieldInterceptorAccessor.FieldInterceptor)),
				$"Test pre-condition not met: entity property {nameof(Book.FieldInterceptor)} should have the same " +
				$"name than {nameof(IFieldInterceptorAccessor)}.{nameof(IFieldInterceptorAccessor.FieldInterceptor)}");
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Persist(new Book
				{
					Name = "some name",
					Id = 1,
					ALotOfText = "a lot of text ...",
					FieldInterceptor = "Why not that name?"
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
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, nameof(Book.FieldInterceptor)), Is.False);

				NHibernateUtil.Initialize(book);

				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Id"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Name"), Is.True);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False);
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, nameof(Book.FieldInterceptor)), Is.True);
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
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, nameof(Book.FieldInterceptor)), Is.True);
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
				Assert.That(book.FieldInterceptor, Is.EqualTo("Why not that name?"));
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
				book.FieldInterceptor += "updated";

				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False, "Before flush and commit");
				trans.Commit();
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False, "After flush and commit");
			}

			using (ISession s = OpenSession())
			{
				book = s.Get<Book>(1);
				Assert.That(book.Name, Is.EqualTo("some nameupdated"));
				Assert.That(book.FieldInterceptor, Is.EqualTo("Why not that name?updated"));
			}
		}
	}
}
