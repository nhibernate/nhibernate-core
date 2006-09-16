#if NET_2_0

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Collections;
using NUnit.Framework;
using NHibernate.Expression;
using NHibernate.Type;

namespace NHibernate.Test.ExpressionTest.Projection
{
    [TestFixture]
    public class ProjectionSqlFixture : TestCase
    {
        protected override string MappingsAssembly
        {
            get { return "NHibernate.Test"; }
        }

        protected override IList Mappings
        {
            get
            {
                return new string[] { "ExpressionTest.Projection.ProjectionClass.hbm.xml" };
            }
        }

        protected override void OnSetUp()
        {
            base.OnSetUp();

            // Create some objects
            using (ISession session = OpenSession())
            {
                session.Save(new ProjectionTestClass(1));
                session.Save(new ProjectionTestClass(2));
                session.Save(new ProjectionTestClass(3));
                session.Save(new ProjectionTestClass(4));
                session.Flush();
            }
        }

        protected override void OnTearDown()
        {
            using (ISession s = sessions.OpenSession())
            {
                s.Delete("from ProjectionTestClass");
                s.Flush();
            }
        }



        [Test]
        public void QueryTest1()
        {
            using (ISession session = OpenSession())
            {
                ICriteria c = session.CreateCriteria(typeof(ProjectionTestClass));

                c.Projection = Projections.ProjectionList()
                    .Add(Projections.Avg("Pay"))
                    .Add(Projections.Max("Pay"))
                    .Add(Projections.Min("Pay"))
                    ;
                IList result = c.List();// c.UniqueResult();
                Assert.IsTrue(result.Count == 1, "More than one record was found, while just one was expected");
                Assert.IsTrue(result[0] is object[], 
                    "expected object[] as result, but found " + result[0].GetType().Name);
                object[] results = (object[])result[0];
                Assert.AreEqual(results.Length, 3);
                Assert.AreEqual(results[0], 2.5);
                Assert.AreEqual(results[1], 4);
                Assert.AreEqual(results[2], 1);
            }
        }

        [Test]
        public void SelectSqlProjectionTest()
        {
            using (ISession session = OpenSession())
            {
                ICriteria c = session.CreateCriteria(typeof(ProjectionTestClass));

                c.Projection = Projections.ProjectionList()
                    .Add(Projections.SqlProjection("Avg({alias}.Pay) MyPay",
                    new string[] { "MyPay" },
                    new IType[] { NHibernateUtil.Double }));

                IList result = c.List();// c.UniqueResult();
                Assert.IsTrue(result.Count == 1);
                object results = (object)result[0];
                Assert.AreEqual(results, 2.5);
            }
        }
    }
}
#endif