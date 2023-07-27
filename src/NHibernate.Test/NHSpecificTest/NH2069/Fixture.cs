using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2069
{
    [TestFixture]
    public class Fixture : BugTestCase
    {
        protected override void OnSetUp()
        {
            using (var s = OpenSession())
            using (var t = s.BeginTransaction())
            {
                var test2 = new Test2();
                test2.Cid = 5;
                test2.Description = "Test 2: CID = 5";
                              
                var test = new Test();
                test.Cid = 1;
                test.Description = "Test: CID = 1";
                test.Category = test2;
                
                s.Save(test2);
                s.Save(test);

                t.Commit();
            }
        }

        protected override void OnTearDown()
        {
            using (var s = OpenSession())
            using (var t = s.BeginTransaction())
            {
                s.Delete("from Test");
                s.Delete("from Test2");
                t.Commit();
            }
         }

        [Test]
        public void ProxyRemainsUninitializedWhenReferencingIdProperty()
        {
            using (ISession session = base.OpenSession())
            {
                ITest b = session.CreateQuery("from Test").UniqueResult<Test>();
    
                Assert.IsNotNull(b);
    
                INHibernateProxy proxy = b.Category as INHibernateProxy;
    
                Assert.That(proxy, Is.Not.Null);
    
                Assert.That(proxy.HibernateLazyInitializer.IsUninitialized, "Proxy should be uninitialized.");
    
                long cid = b.Category.Cid;
                
                Assert.That(proxy.HibernateLazyInitializer.IsUninitialized, "Proxy should still be uninitialized.");
            }
        }
    }
}
