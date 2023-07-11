using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Intercept;
using NHibernate.Linq;
using NHibernate.Tuple.Entity;
using NUnit.Framework;
using NUnit.Framework.Constraints;

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

		protected override string[] Mappings
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

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
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
					Image = new byte[10],
					NoSetterImage = new byte[10],
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
				s.CreateQuery("delete from Word").ExecuteUpdate();
				s.CreateQuery("delete from Book").ExecuteUpdate();
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
		public void CanSetValueForLazyProperty()
		{
			Book book;
			using (ISession s = OpenSession())
			{
				book = s.Get<Book>(1);
			}

			book.ALotOfText = "text";

			Assert.That(book.ALotOfText, Is.EqualTo("text"));
			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.True);
		}

		[TestCase(false)]
		[TestCase(true)]
		public void CanUpdateValueForLazyProperty(bool initializeAfterSet)
		{
			Book book;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				book = s.Get<Book>(1);
				book.ALotOfText = "update-text";
				if (initializeAfterSet)
				{
					var image = book.Image;
				}

				tx.Commit();
			}

			using (var s = OpenSession())
			{
				book = s.Get<Book>(1);
				var text = book.ALotOfText;
			}

			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Image"), Is.True);
			Assert.That(book.ALotOfText, Is.EqualTo("update-text"));
			Assert.That(book.Image, Has.Length.EqualTo(10));
		}

		[TestCase(false)]
		[TestCase(true)]
		public void UpdateValueForLazyPropertyToSameValue(bool initializeAfterSet)
		{
			Book book;
			string text;

			using (var s = OpenSession())
			{
				book = s.Get<Book>(1);
				text = book.ALotOfText;
			}

			Sfi.Statistics.Clear();

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				book = s.Get<Book>(1);
				book.ALotOfText = text;
				if (initializeAfterSet)
				{
					var image = book.Image;
				}

				tx.Commit();
			}

			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(initializeAfterSet ? 0 : 1));
			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Image"), initializeAfterSet ? (Constraint) Is.True : Is.False);
			Assert.That(book.ALotOfText, Is.EqualTo(text));

			using (var s = OpenSession())
			{
				book = s.Get<Book>(1);
				text = book.ALotOfText;
			}

			Assert.That(book.Image, Has.Length.EqualTo(10));
			Assert.That(book.ALotOfText, Is.EqualTo(text));
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
			int bookCount;
			using (ISession s = OpenSession())
			{
				bookCount = s.Query<Book>().Count();
				book = s.Get<Book>(1);
			}

			book.Name += " updated";

			using (ISession s = OpenSession())
			{
				s.Merge(book);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				Assert.That(s.Query<Book>().Count(), Is.EqualTo(bookCount));
				Assert.That(s.Get<Book>(1).Name, Is.EqualTo(book.Name));
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

		[Test]
		public void CanMergeTransientWithLazyProperty()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(2);
				Assert.That(book, Is.Null);
			}

			using (ISession s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var book = new Book
				{
					Name = "some name two",
					Id = 2,
					ALotOfText = "a lot of text two..."
				};
				// This should insert a new entity.
				s.Merge(book);
				tx.Commit();
			}

			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(2);
				Assert.That(book, Is.Not.Null);
				Assert.That(book.Name, Is.EqualTo("some name two"));
				Assert.That(book.ALotOfText, Is.EqualTo("a lot of text two..."));
			}
		}

		[Test]
		public void CacheShouldNotContainLazyProperties()
		{
			Book book;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				book = s.CreateQuery("from Book b fetch all properties where b.Id = :id")
				        .SetParameter("id", 1)
				        .UniqueResult<Book>();
				tx.Commit();
			}

			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.True);
			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Image"), Is.True);

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				book = s.Get<Book>(1);
				tx.Commit();
			}

			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.False);
			Assert.That(NHibernateUtil.IsPropertyInitialized(book, "Image"), Is.False);
		}

		[Test]
		public void CanMergeTransientWithLazyPropertyInCollection()
		{
			Book book;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				book = new Book
				{
					Name = "some name two",
					Id = 3,
					ALotOfText = "a lot of text two..."
				};
				// This should insert a new entity.
				s.Merge(book);
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				book = s.Get<Book>(3);
				Assert.That(book, Is.Not.Null);
				Assert.That(book.Name, Is.EqualTo("some name two"));
				Assert.That(book.ALotOfText, Is.EqualTo("a lot of text two..."));
			}
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				book.Words = new List<Word>();
				var word = new Word
				{
					Id = 2,
					Parent = book,
					Content = new byte[1] {0}
				};

				book.Words.Add(word);
				s.Merge(book);
				tx.Commit();
			}

			using (var s = OpenSession())
			{
				book = s.Get<Book>(3);
				Assert.That(book.Words.Any(), Is.True);
				Assert.That(book.Words.First().Content, Is.EqualTo(new byte[1] { 0 }));
			}
		}

		[Test(Description = "GH-3333")]
		public void GetLazyPropertyWithNoSetterAccessor_PropertyShouldBeInitialized()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1);
				var image = book.NoSetterImage;
				// Fails. Property remains uninitialized after it has been accessed.
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "NoSetterImage"), Is.True);
			}
		}

		[Test(Description = "GH-3333")]
		public void GetLazyPropertyWithNoSetterAccessorTwice_ResultsAreSameObject()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1);
				var image = book.NoSetterImage;
				var sameImage = book.NoSetterImage;
				// Fails. Each call to a property getter returns a new object.
				Assert.That(ReferenceEquals(image, sameImage), Is.True);
			}
		}

		[Test]
		public void CanSetValueForLazyPropertyNoSetter()
		{
			Book book;
			using (ISession s = OpenSession())
			{
				book = s.Get<Book>(1);
				book.NoSetterImage = new byte[]{10};
			}

			Assert.That(NHibernateUtil.IsPropertyInitialized(book, nameof(book.NoSetterImage)), Is.True);
			CollectionAssert.AreEqual(book.NoSetterImage, new byte[] { 10 });
		}

		[Test]
		public void CanFetchLazyPropertyNoSetter()
		{
			using (ISession s = OpenSession())
			{
				var book = s
					.Query<Book>()
					.Fetch(x => x.NoSetterImage)
					.First(x => x.Id == 1);

				Assert.That(NHibernateUtil.IsPropertyInitialized(book, nameof(book.NoSetterImage)), Is.True);
			}
		}
	}
}
