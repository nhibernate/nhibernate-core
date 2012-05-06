using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Test.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2812
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void OnSetUp()
        {
            using (ISession session = OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    var entity = new EntityWithAByteValue();
                    entity.ByteValue = 1;
                    session.Save(entity);
                    tx.Commit();
                }
            }
        }

        protected override void OnTearDown()
        {
            base.OnTearDown();            
            using (ISession session = OpenSession())
            {
                using (ITransaction tx = session.BeginTransaction())
                {
                    session.Delete("from EntityWithAByteValue");
                    tx.Commit();
                }
            }
        }

        [Test]
        public void Performing_a_query_on_a_byte_column_should_not_throw()
        {
            using (var session = sessions.OpenSession())
            {
                var query = (from e in session.Query<EntityWithAByteValue>()
                             where e.ByteValue == 1
                             select e)
                             .ToList();

                // this should not fail if fixed
                Assert.AreEqual(1, query.Count);
            }
        }
    }
}
