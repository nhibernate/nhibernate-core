using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Impl;
using NHibernate.Test.SecondLevelCacheTests;
using NSubstitute;
using NUnit.Framework;
using Environment = System.Environment;

namespace NHibernate.Test.SecondLevelCacheTest
{
	[TestFixture]
	public class InvalidationTests : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override IList Mappings => new[] { "SecondLevelCacheTest.Item.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.Properties[Cfg.Environment.CacheProvider] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
			configuration.Properties[Cfg.Environment.UseQueryCache] = "true";
		}

		[Test]
		public void InvalidatesEntities()
		{
			var cache = Substitute.For<UpdateTimestampsCache>(Sfi.Settings, new Dictionary<string, string>());
			((SessionFactoryImpl) (Sfi as DebugSessionFactory).ActualFactory).SetPropertyUsingReflection(
				x => x.UpdateTimestampsCache,
				cache);

			var items = new List<Item>();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = new Item {Id = i};
						session.Save(item);
					}

					tx.Commit();
				}

				using (ITransaction tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = session.Get<Item>(i);
						item.Name = item.Id.ToString();
					}

					tx.Commit();
				}

				using (ITransaction tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = session.Get<Item>(i);
						session.Delete(item);
					}

					tx.Commit();
				}
			}

			//Should receive one preinvalidation and one invalidation per commit
			cache.Received(3).PreInvalidate(Arg.Is<IReadOnlyCollection<object>>(x => x.Count == 1 && (string) x.First() == "Item"));
			cache.Received(3).Invalidate(Arg.Is<IReadOnlyCollection<object>>(x => x.Count == 1 && (string) x.First() == "Item"));
		}

		public void CleanUp()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Item");
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			CleanUp();
			base.OnTearDown();
		}
	}
}
