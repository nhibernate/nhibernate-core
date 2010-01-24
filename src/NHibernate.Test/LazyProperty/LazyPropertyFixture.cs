using System.Collections;
using log4net.Core;
using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
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

		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.SetProperty(Environment.ProxyFactoryFactoryClass,
									  typeof(ProxyFactoryFactory).AssemblyQualifiedName);
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

				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "Id"));
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "Name"));
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"));

				NHibernateUtil.Initialize(book);

				Assert.True(NHibernateUtil.IsPropertyInitialized(book, "Id"));
				Assert.True(NHibernateUtil.IsPropertyInitialized(book, "Name"));
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"));
			}
		}

		[Test]
		public void ShouldGenerateErrorForNonAutoPropLazyProp()
		{
			Assert.IsTrue(log.Contains("Lazy or ghost property NHibernate.Test.LazyProperty.Book.ALotOfText is not an auto property, which may result in uninitialized property access"));
		}

		[Test]
		public void PropertyLoadedNotInitializedWhenUsingGet()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1);

				Assert.True(NHibernateUtil.IsPropertyInitialized(book, "Id"));
				Assert.True(NHibernateUtil.IsPropertyInitialized(book, "Name"));
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"));
			}
		}

		[Test]
		public void CanGetValueForLazyProperty()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1);

				Assert.AreEqual("a lot of text ...", book.ALotOfText);
				Assert.True(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"));
			}
		}

		[Test]
		public void CanGetValueForNonLazyProperty()
		{
			using (ISession s = OpenSession())
			{
				var book = s.Get<Book>(1);

				Assert.AreEqual("some name", book.Name);
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"));
			}
		}

	}
}