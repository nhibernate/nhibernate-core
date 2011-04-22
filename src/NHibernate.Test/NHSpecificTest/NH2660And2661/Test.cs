using System;
using NHibernate.Dialect;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2660And2661
{
    [TestFixture]
    public class Test : BugTestCase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = OpenSession())
            {
                DomainClass entity = new DomainClass { Id = 1, Data = DateTime.Parse("10:00") };
                session.Save(entity);
                session.Flush();
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession session = OpenSession())
            {
								session.CreateQuery("delete from DomainClass").ExecuteUpdate();
                session.Flush();
            }
        }

        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            return dialect is MsSql2008Dialect;
        }

        [Test, Ignore("workaround to sqlserver DP, not fixed yet")]
        public void ShouldBeAbleToQueryEntity()
        {
            using (ISession session = OpenSession())
            {
               var query =
                    session.CreateQuery(
                        @"from DomainClass entity where Data = :data");
                query.SetParameter("data", DateTime.Parse("10:00"), NHibernateUtil.Time);
                query.Executing(x=> x.List()).NotThrows();
            }
        }
    }
}
