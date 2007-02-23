using System.Collections;
using NHibernate.Cache;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.SecondLevelCacheTests
{
	[TestFixture]
	public class SecondLevelCacheTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] {"SecondLevelCacheTest.Item.hbm.xml"}; }
		}

		protected override void OnSetUp()
		{
			cfg.Properties[Environment.CacheProvider] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
			cfg.Properties[Environment.UseQueryCache] = "true";
			sessions = cfg.BuildSessionFactory();

			using (ISession session = OpenSession())
			{
				Item item = new Item();
				item.Id = 1;
				session.Save(item);
				for (int i = 0; i < 4; i++)
				{
					Item child = new Item();
					child.Id = i + 2;
					child.Parent = item;
					session.Save(child);
					item.Children.Add(child);
				}
				session.Flush();
			}

			sessions.Evict(typeof(Item));
			sessions.EvictCollection(typeof(Item).FullName + ".Children");
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				session.Delete("from Item"); //cleaning up
				session.Flush();
			}
		}

		[Test]
		public void CachedQueriesHandlesEntitiesParametersCorrectly()
		{
			using (ISession session = OpenSession())
			{
				Item one = (Item) session.Load(typeof(Item), 1);
				IList results = session.CreateQuery("from Item item where item.Parent = :parent")
					.SetEntity("parent", one)
					.SetCacheable(true).List();
				Assert.AreEqual(4, results.Count);
				foreach (Item item in results)
				{
					Assert.AreEqual(1, item.Parent.Id);
				}
			}

			using (ISession session = OpenSession())
			{
				Item two = (Item) session.Load(typeof(Item), 2);
				IList results = session.CreateQuery("from Item item where item.Parent = :parent")
					.SetEntity("parent", two)
					.SetCacheable(true).List();
				Assert.AreEqual(0, results.Count);
			}
		}

		[Test]
		public void DeleteItemFromCollectionThatIsInTheSecondLevelCache()
		{
			using (ISession session = OpenSession())
			{
				Item item = (Item) session.Load(typeof(Item), 1);
				Assert.IsTrue(item.Children.Count == 4); // just force it into the second level cache here
			}
			int childId = -1;
			using (ISession session = OpenSession())
			{
				Item item = (Item) session.Load(typeof(Item), 1);
				Item child = (Item) item.Children[0];
				childId = child.Id;
				session.Delete(child);
				item.Children.Remove(child);
				session.Flush();
			}

			using (ISession session = OpenSession())
			{
				Item item = (Item) session.Load(typeof(Item), 1);
				Assert.AreEqual(3, item.Children.Count);
				foreach (Item child in item.Children)
				{
					NHibernateUtil.Initialize(child);
					Assert.IsFalse(child.Id == childId);
				}
			}
		}

		[Test]
		public void InsertItemToCollectionOnTheSecondLevelCache()
		{
			using (ISession session = OpenSession())
			{
				Item item = (Item) session.Load(typeof(Item), 1);
				Item child = new Item();
				child.Id = 6;
				item.Children.Add(child);
				session.Save(child);
				session.Flush();
			}

			using (ISession session = OpenSession())
			{
				Item item = (Item) session.Load(typeof(Item), 1);
				int count = item.Children.Count;
				Assert.AreEqual(5, count);
			}
		}
	}
}