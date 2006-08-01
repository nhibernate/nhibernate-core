using System;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Text;
using NHibernate.Cache;
using NHibernate.Test.SecondLevelCacheTests;
using NUnit.Framework;

namespace NHibernate.Test.UserCollection
{
    [TestFixture]
    public class SecondLevelCacheTest : TestCase
    {
        protected override string MappingsAssembly
        {
            get
            {
                return "NHibernate.Test";
            }
        }
        protected override IList Mappings
        {
            get { return new string[] { "SecondLevelCacheTest.Item.hbm.xml" }; }
        }

        protected override void OnSetUp()
        {
            cfg.Properties["hibernate.cache.provider_class"] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
            sessions = cfg.BuildSessionFactory();
        }

        [Ignore("Still an issue")]
        [Test]
        public void DeleteItemFromCollectionThatIsInTheSecondLevelCache()
        {
            using(ISession session = OpenSession())
            {
                Item item = new Item();
                item.Id = 1;
                session.Save(item);
                for (int i = 0; i < 4; i++)
                {
                    Item child = new Item();
                    child.Id = i + 2;
                    session.Save(child);
                    item.Children.Add(child);
                }
                session.Flush();
            }
            
            sessions.Evict(typeof(Item));
            sessions.EvictCollection(typeof(Item).FullName+".Children");
            
            using (ISession session = OpenSession())
            {
                Item item = (Item)session.Load(typeof(Item), 1);
                Assert.IsTrue(item.Children.Count == 4); // just force it into the second level cache here
            }
            int childId = -1;
            using (ISession session = OpenSession())
            {
                Item item = (Item)session.Load(typeof(Item), 1);
                Item child = (Item)item.Children[0];
                childId = child.Id;
                session.Delete(child);
                session.Flush();
            }

            using (ISession session = OpenSession())
            {
                Item item = (Item)session.Load(typeof(Item), 1);
                Assert.AreEqual(3, item.Children.Count );
                foreach (Item child in item.Children)
                {
                    NHibernateUtil.Initialize(child);
                    Assert.IsFalse(child.Id == childId);
                }
            }

            using(ISession session = OpenSession())
            {
                session.Delete("from Item");//cleaning up
                session.Flush();
            }
        }
    }
}
