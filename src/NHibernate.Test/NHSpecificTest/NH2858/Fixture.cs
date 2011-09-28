using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NHibernate.Dialect;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH2858
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = this.OpenSession())
            {
                DomainClass entity = new DomainClass();
                entity.Id = 1;
                entity.TheGuid = Guid.Empty;
                session.Save(entity);
                session.Flush();
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();
            using (ISession session = this.OpenSession())
            {
                string hql = "from System.Object";
                session.Delete(hql);
                session.Flush();
            }
        }

        protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
        {
            return dialect as MsSql2005Dialect != null;
        }

        [Test]
        public void Guid_ToString_ShouldBeRetrievedCorrectly_In_Linq_Projection()
        {
            using (ISession session = this.OpenSession())
            {
                var guidToString = session.Query<DomainClass>().Select(x => x.TheGuid.ToString()).First();
                Assert.AreEqual(Guid.Empty.ToString(), guidToString);
            }
        }
    }
}
