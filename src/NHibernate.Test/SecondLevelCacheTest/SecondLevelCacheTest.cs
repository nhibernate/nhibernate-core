using System;
using System.Collections;
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

        [Test]
        public void DeleteItemFromCollectionThatIsInTheSecondLevelCache()
        {
            Item parent;
            Item firstChild=null;
            using (ISession session = OpenSession())
            {
                parent = new Item();
                session.Save(parent);
                for (int i = 0; i < 5; i++)
                {
                    Item child = new Item();
                    if (i == 0)
                        firstChild = child;
                    parent.Children.Add(child);
                    session.Save(child);
                }
                session.Flush();
            }

            using (ISession session = OpenSession())
            {
                Item child2 = session.Load(typeof(Item), firstChild.Id) as Item;
                session.Delete(child2);
                session.Flush();
            }

            using (ISession session = OpenSession())
            {
                Item parent3 = session.Load(typeof(Item), parent.Id) as Item;
                int count = parent3.Children.Count;//force lazy load
                Assert.AreEqual(4, count);
            }

            //cleanup
            using (ISession session = OpenSession())
            {
                session.Delete("from Item");
                session.Flush();
            }
        }
    }
}
