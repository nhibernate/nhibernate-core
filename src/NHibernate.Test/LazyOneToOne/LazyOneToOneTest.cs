using System;
using System.Collections;
using NHibernate.Intercept;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.LazyOneToOne
{
	[TestFixture]
	public class LazyOneToOneTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] {"LazyOneToOne.Person.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		//protected override bool AppliesTo(Dialect.Dialect dialect)
		//{
		//  // this test work only with Field interception (NH-1618)
		//  return FieldInterceptionHelper.IsInstrumented( new Person() );
		//}
		protected override void Configure(Cfg.Configuration configuration)
		{
			configuration.SetProperty(Environment.MaxFetchDepth, "2");
			configuration.SetProperty(Environment.UseSecondLevelCache, "false");
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		[Test]
		public void Lazy()
		{
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			var p = new Person {Name = "Gavin"};
			var p2 = new Person {Name = "Emmanuel"};
			var e = new Employee(p);
			new Employment(e, "JBoss");
			var old = new Employment(e, "IFA") {EndDate = DateTime.Today};
			s.Persist(p);
			s.Persist(p2);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateQuery("from Person where name='Gavin'").UniqueResult<Person>();
			Assert.That(!NHibernateUtil.IsPropertyInitialized(p, "Employee"));

			Assert.That(p.Employee.Person, Is.SameAs(p));
			Assert.That(NHibernateUtil.IsInitialized(p.Employee.Employments));
			Assert.That(p.Employee.Employments.Count, Is.EqualTo(1));

			p2 = s.CreateQuery("from Person where name='Emmanuel'").UniqueResult<Person>();
			Assert.That(p2.Employee, Is.Null);
			t.Commit();
			s.Close();

			s = OpenSession();
			t = s.BeginTransaction();
			p = s.Get<Person>("Gavin");
			Assert.That(!NHibernateUtil.IsPropertyInitialized(p, "Employee"));

			Assert.That(p.Employee.Person, Is.SameAs(p));
			Assert.That(NHibernateUtil.IsInitialized(p.Employee.Employments));
			Assert.That(p.Employee.Employments.Count, Is.EqualTo(1));

			p2 = s.Get<Person>("Emmanuel");
			Assert.That(p2.Employee, Is.Null);
			s.Delete(p2);
			s.Delete(old);
			s.Delete(p);
			t.Commit();
			s.Close();
		}
	}
}