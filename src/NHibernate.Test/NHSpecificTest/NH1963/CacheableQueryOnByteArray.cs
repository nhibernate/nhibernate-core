using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1963
{
    [TestFixture]
    public class CacheableQueryOnByteArray : BugTestCase
    {
        protected override void OnSetUp()
        {
            base.OnSetUp();
            using (ISession session = this.OpenSession())
            {
                DomainClass entity = new DomainClass();
                entity.Id = 1;
                entity.ByteData = new byte[] {1, 2, 3};
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
            return dialect as MsSql2000Dialect != null;
        }

        [Test]
        public void Should_be_able_to_do_cacheable_query_on_byte_array_field()
        {
            using (ISession session = this.OpenSession())
            {
                var data = new byte[] { 1, 2, 3 };

                var result = session.CreateQuery("from DomainClass d where d.ByteData = :data")
                    .SetBinary("data", data)
                    .SetCacheable(true)
                    .UniqueResult<DomainClass>();

                Assert.IsNotNull(result);
            }

            using (ISession session = this.OpenSession())
            {
                var data = new byte[] { 1, 2, 3 };

                var result = session.CreateQuery("from DomainClass d where d.ByteData = :data")
										.SetBinary("data", data)
                    .SetCacheable(true)
                    .UniqueResult<DomainClass>();

                Assert.IsNotNull(result);
            }
        }

        [Test]
        public void Should_work_when_query_is_not_cachable()
        {
            using (ISession session = this.OpenSession())
            {
                var data = new byte[] { 1, 2, 3 };

                var result = session.CreateQuery("from DomainClass d where d.ByteData = :data")
                    .SetParameter("data", data)
                    .UniqueResult<DomainClass>();

                Assert.IsNotNull(result);
            }

            using (ISession session = this.OpenSession())
            {
                var data = new byte[] { 1, 2, 3 };

                var result = session.CreateQuery("from DomainClass d where d.ByteData = :data")
                    .SetParameter("data", data)
                    .UniqueResult<DomainClass>();

                Assert.IsNotNull(result);
            }
        }
    }
}
