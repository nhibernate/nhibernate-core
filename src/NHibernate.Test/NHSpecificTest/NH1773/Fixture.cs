using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1773
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			//configuration.SetProperty(NHibernate.Cfg.Environment.QueryTranslator, typeof(NHibernate.Hql.Classic.ClassicQueryTranslatorFactory).AssemblyQualifiedName);
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "NHSpecificTest.NH1773.Person.hbm.xml"}; }
		}

		[Test]
		public void CustomHQLFunctionsShouldBeRecognizedByTheParser()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Country c = new Country() {Id = 100, Name = "US"};
					Person p = new Person() {Age = 35, Name = "My Name", Id=1, Country = c};
					s.Save(c);
					s.Save(p);
					tx.Commit();
				}
			}

			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				IList result = s.CreateQuery("select new PersonResult(p, current_timestamp()) from Person p left join fetch p.Country").List();

				Assert.AreEqual("My Name", ((PersonResult)result[0]).Person.Name);
				Assert.IsTrue(NHibernateUtil.IsInitialized(((PersonResult)result[0]).Person.Country));
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				s.Delete("from Country");
				tx.Commit();
			}
		}
	}
}
