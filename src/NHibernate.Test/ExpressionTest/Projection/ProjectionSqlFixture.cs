using System;
using System.Collections;
using NUnit.Framework;
using NHibernate.Criterion;
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

        protected override string[] Mappings
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
            using (ISession s = Sfi.OpenSession())
            {
                s.Delete("from ProjectionTestClass");
                s.Flush();
            }
        }

		[Test]
		public void QueryTestWithStrongTypeReturnValue()
		{
			using (ISession session = OpenSession())
			{
				ICriteria c = session.CreateCriteria(typeof(ProjectionTestClass));

				NHibernate.Transform.IResultTransformer trans = new NHibernate.Transform.AliasToBeanConstructorResultTransformer(
					typeof(ProjectionReport).GetConstructors()[0]
					);
				
				c.SetProjection(Projections.ProjectionList()
		                			.Add(Projections.Avg("Pay"))
		                			.Add(Projections.Max("Pay"))
		                			.Add(Projections.Min("Pay")));
				c.SetResultTransformer(trans);
				ProjectionReport report = c.UniqueResult<ProjectionReport>();
				Assert.AreEqual(2.5, report.AvgPay);
				Assert.AreEqual(4, report.MaxPay);
				Assert.AreEqual(1, report.MinPay);
			}
		}

        [Test]
        public void QueryTest1()
        {
            using (ISession session = OpenSession())
            {
                ICriteria c = session.CreateCriteria(typeof(ProjectionTestClass));

                c.SetProjection(Projections.ProjectionList()
                    .Add(Projections.Avg("Pay"))
                    .Add(Projections.Max("Pay"))
                    .Add(Projections.Min("Pay")))
                    ;
				IList result = c.List(); // c.UniqueResult();
                Assert.IsTrue(result.Count == 1, "More than one record was found, while just one was expected");
                Assert.IsTrue(result[0] is object[], 
                    "expected object[] as result, but found " + result[0].GetType().Name);
                object[] results = (object[])result[0];
                Assert.AreEqual(3, results.Length);
                Assert.AreEqual(2.5, results[0]);
                Assert.AreEqual(4, results[1]);
                Assert.AreEqual(1, results[2]);
            }
        }

        [Test]
        public void SelectSqlProjectionTest()
        {
            using (ISession session = OpenSession())
            {
                ICriteria c = session.CreateCriteria(typeof(ProjectionTestClass));

                c.SetProjection(Projections.ProjectionList()
                    .Add(Projections.SqlProjection("Avg({alias}.Pay) as MyPay",
                    new string[] { "MyPay" },
                    new IType[] { NHibernateUtil.Double })));

                IList result = c.List(); // c.UniqueResult();
                Assert.IsTrue(result.Count == 1);
                object results = result[0];
                Assert.AreEqual(2.5, results);
            }
        }
    }
	
	public class ProjectionReport
	{
		double minPay;
		double maxPay;
		double avgPay;

		public ProjectionReport(double avgPay, double maxPay, double minPay)
		{
			this.minPay = minPay;
			this.maxPay = maxPay;
			this.avgPay = avgPay;
		}

		public double MinPay
		{
			get { return minPay; }
			set { minPay = value; }
		}

		public double MaxPay
		{
			get { return maxPay; }
			set { maxPay = value; }
		}

		public double AvgPay
		{
			get { return avgPay; }
			set { avgPay = value; }
		}
	}
}
