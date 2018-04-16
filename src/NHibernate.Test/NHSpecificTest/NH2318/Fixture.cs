using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;
using NHibernate.Linq;
using System.Linq;
using NHibernate.Linq.Functions;

namespace NHibernate.Test.NHSpecificTest.NH2318
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		protected override void Configure(Cfg.Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty("linqtohql.generatorsregistry", "NHibernate.Test.NHSpecificTest.NH2318.ExtendedLinqtoHqlGeneratorsRegistry, NHibernate.Test");
		}

		protected override void OnTearDown()
		{
			using( ISession s = Sfi.OpenSession() )
			{
				s.Delete( "from A" );
				s.Flush();
			}
		}

        private void AddObjects()
        {
            ISession s = OpenSession();
            try
            {
                s.Save(new A { Name = "first" });
                s.Save(new A { Name = "second" });
                s.Save(new A { Name = "aba" });
                s.Flush();
            }
            finally
            {
                s.Close();
            }
        }

        [Test]
        public void CriteriaTrimFunctionsWithParameters()
        {
            AddObjects();

            ISession s = OpenSession();
            try
            {
                ICriteria criteria = s.CreateCriteria(typeof (A)).Add(
                    Restrictions.Eq(
                        Projections.SqlFunction(
                            "trim",
                            NHibernateUtil.String,
                            Projections.Constant("f"),
                            Projections.SqlProjection("from", null, null), // Silly hack to get "from" as a second argument.
                            Projections.Property("Name")),
                        "irst"));
                IList<A> items = criteria.List<A>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("first", items[0].Name);
			}
            finally
            {
                s.Close();
            }
        }

        [Test]
        public void LinqTrimFunctionsWithParameters()
        {
            AddObjects();

            ISession s = OpenSession();
            try
            {
                IList<A> items = s.Query<A>().Where(a => a.Name.TrimLeading("f").TrimTrailing("t") == "irs").ToList();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("first", items[0].Name);

                string trimString = "a";
                items = s.Query<A>().Where(a => a.Name.TrimLeading(trimString).TrimTrailing(trimString) == "b").ToList();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("aba", items[0].Name);
            }
            finally
            {
                s.Close();
            }
        }

        [Test]
        public void HqlTrimFunctionsWithParameters()
        {
            AddObjects();

            ISession s = OpenSession();
            try
            {
                IList<A> items = s.CreateQuery("from A a where a.Name = :p0 or a.Name <> :p0 order by a.Name").SetParameter("p0", "first").List<A>();
                Assert.AreEqual(3, items.Count);
                Assert.AreEqual("aba", items[0].Name);
                Assert.AreEqual("first", items[1].Name);
                Assert.AreEqual("second", items[2].Name);

                items = s.CreateQuery("from A a where a.Name = ? or a.Name <> ? order by a.Name").SetParameter(0, "first").SetParameter(1, "first").List<A>();
                Assert.AreEqual(3, items.Count);
                Assert.AreEqual("aba", items[0].Name);
                Assert.AreEqual("first", items[1].Name);
                Assert.AreEqual("second", items[2].Name);

                items = s.CreateQuery("from A a where TRIM(LEADING :p0 FROM a.Name) = 'irst'").SetParameter("p0", "f").List<A>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("first", items[0].Name);

                items = s.CreateQuery("from A a where TRIM(TRAILING :p0 FROM TRIM(LEADING :p1 FROM a.Name)) = 'irs'").SetParameter("p0", "t").SetParameter("p1", "f").List<A>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("first", items[0].Name);

                items = s.CreateQuery("from A a where TRIM(TRAILING :p0 FROM TRIM(LEADING :p0 FROM a.Name)) = 'b'").SetParameter("p0", "a").List<A>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("aba", items[0].Name);

                items = s.CreateQuery("from A a where TRIM(TRAILING :p0 FROM a.Name) = 'firs'").SetParameter("p0", "t").List<A>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("first", items[0].Name);

                items = s.CreateQuery("from A a where TRIM(LEADING ? FROM a.Name) = 'irst'").SetParameter(0, "f").List<A>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("first", items[0].Name);

                items = s.CreateQuery("from A a where TRIM(TRAILING ? FROM a.Name) = 'firs'").SetParameter(0, "t").List<A>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("first", items[0].Name);

                items = s.CreateQuery("from A a where TRIM(TRAILING ? FROM TRIM(LEADING ? FROM a.Name)) = 'irs'").SetParameter(0, "t").SetParameter(1, "f").List<A>();
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual("first", items[0].Name);
            }
            finally
            {
                s.Close();
            }
        }
    }
}
