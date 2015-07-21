using NHibernate.Context;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3058
{
	[TestFixture]
	public class SampleTest : BugTestCase
	{
		public static ISessionFactoryImplementor AmbientSfi { get; private set; }

		protected override void BuildSessionFactory()
		{
			base.BuildSessionFactory();

			AmbientSfi = Sfi;
		}

		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);

			configuration.Properties.Add("current_session_context_class", "thread_static");
		}

		protected override ISession OpenSession()
		{
			var session = base.OpenSession();

			CurrentSessionContext.Bind(session);

			return session;
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var book = new DomainClass
				{
					Name = "Some name",
					ALotOfText = "Some text",
					Id = 1
				};

				s.Persist(book);
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				Assert.That(s.CreateSQLQuery("delete from DomainClass").ExecuteUpdate(), Is.EqualTo(1));
				tx.Commit();
			}
		}

		[Test]
		public void MethodShouldLoadLazyProperty()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var book = s.Load<DomainClass>(1);
				
				Assert.False(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"));

				string value = book.LoadLazyProperty();

				Assert.That(value, Is.EqualTo("Some text"));
				Assert.That(NHibernateUtil.IsPropertyInitialized(book, "ALotOfText"), Is.True);

				tx.Commit();
			}
		}
	}
}
