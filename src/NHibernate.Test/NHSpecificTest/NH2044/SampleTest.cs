using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2044
{
    [TestFixture]
    public class SampleTest : BugTestCase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = this.OpenSession())
            {
                DomainClass entity = new DomainClass();
                entity.Id = 1;
                entity.Symbol = 'S';
                session.Save(entity);
                session.Flush();
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession session = this.OpenSession())
            {
                string hql = "from DomainClass";
                session.Delete(hql);
                session.Flush();
            }
        }


        [Test]
        public void IgnoreCaseShouldWorkWithCharCorrectly()
        {
            using (ISession session = this.OpenSession())
            {
                ICriteria criteria = session.CreateCriteria(typeof(DomainClass), "domain");
                criteria.Add(NHibernate.Criterion.Expression.Eq("Symbol", 's').IgnoreCase());
                IList<DomainClass> list = criteria.List<DomainClass>();

                Assert.AreEqual(1, list.Count);
                
            }
        }
    }
}
