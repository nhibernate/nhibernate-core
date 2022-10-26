using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Test.CacheTest.Caches;
using NUnit.Framework;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class SerializingCacheFixture : TestCase
	{
		protected override string[] Mappings => new[]
		{
			"CacheTest.ReadOnly.hbm.xml"
		};

		protected override string MappingsAssembly => "NHibernate.Test";

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.UseSecondLevelCache, "true");
			configuration.SetProperty(Environment.UseQueryCache, "true");
			configuration.SetProperty(Environment.CacheProvider, typeof(SerializingCacheProvider).AssemblyQualifiedName);
			configuration.SetProperty(Environment.SessionFactoryName, "SerializingCacheFactory");
		}

		protected override void OnSetUp()
		{
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var totalItems = 6;
				for (var i = 1; i <= totalItems; i++)
				{
					var parent = new ReadOnly
					{
						Name = $"Name{i}"
					};
					for (var j = 1; j <= totalItems; j++)
					{
						var child = new ReadOnlyItem
						{
							Parent = parent
						};
						parent.Items.Add(child);
					}
					s.Save(parent);
				}
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from ReadOnlyItem").ExecuteUpdate();
				s.CreateQuery("delete from ReadOnly").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test]
		public void CachedQueryTest()
		{
			// Put things in cache
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var items = s.Query<ReadOnly>().WithOptions(o => o.SetCacheable(true)).ToList();
				Assert.That(items, Has.Count.GreaterThan(0));
				tx.Commit();
			}

			RebuildSessionFactory();
		}
	}
}
