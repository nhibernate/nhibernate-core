using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.DomainModel;
using NHibernate.Loader;
using NHibernate.Test.CacheTest.Caches;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest
{
	[TestFixture(BatchFetchStyle.Dynamic)]
	[TestFixture(BatchFetchStyle.Legacy)]
	public class BatchableCacheSubclassFixture : TestCase
	{
		private readonly BatchFetchStyle _fetchStyle;

		public BatchableCacheSubclassFixture(BatchFetchStyle fetchStyle)
		{
			_fetchStyle = fetchStyle;
		}

		protected override string[] Mappings
		{
			get
			{
				return new string[]
				{
					"FooBar.hbm.xml",
					"Baz.hbm.xml",
					"Qux.hbm.xml",
					"Glarch.hbm.xml",
					"Fum.hbm.xml",
					"Fumm.hbm.xml",
					"Fo.hbm.xml",
					"One.hbm.xml",
					"Many.hbm.xml",
					"Immutable.hbm.xml",
					"Fee.hbm.xml",
					"Vetoer.hbm.xml",
					"Holder.hbm.xml",
					"Location.hbm.xml",
					"Stuff.hbm.xml",
					"Container.hbm.xml",
					"Simple.hbm.xml"
				};
			}
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.UseSecondLevelCache, "true");
			configuration.SetProperty(Cfg.Environment.UseQueryCache, "true");
			configuration.SetProperty(Cfg.Environment.CacheProvider, typeof(BatchableCacheProvider).AssemblyQualifiedName);
			configuration.SetProperty(Cfg.Environment.BatchFetchStyle, _fetchStyle.ToString());
		}

		protected override void OnSetUp()
		{
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				FooProxy flast = new Bar();
				s.Save(flast);
				for (int i = 0; i < 5; i++)
				{
					FooProxy foo = new Bar();
					s.Save(foo);
					flast.TheFoo = foo;
					flast = flast.TheFoo;
					flast.String = "foo" + (i + 1);
				}
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from NHibernate.DomainModel.Foo as foo");
				tx.Commit();
			}
		}

		[Test]
		public void BatchableRootEntityTest()
		{
			var persister = Sfi.GetEntityPersister(typeof(Foo).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var fooCache = (BatchableCache) persister.Cache.Cache;

			persister = Sfi.GetEntityPersister(typeof(Bar).FullName);
			Assert.That(persister.Cache.Cache, Is.Not.Null);
			Assert.That(persister.Cache.Cache, Is.TypeOf<BatchableCache>());
			var barCache = (BatchableCache) persister.Cache.Cache;

			Assert.That(barCache, Is.EqualTo(fooCache));

			// Add Bar to cache
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var list = s.CreateQuery("from foo in class NHibernate.DomainModel.Foo").List();
				Assert.AreEqual(6, list.Count);
				tx.Commit();
			}

			Assert.That(fooCache.PutCalls, Has.Count.EqualTo(6)); // Bar is not batchable
			Assert.That(fooCache.PutMultipleCalls, Has.Count.EqualTo(0));

			// Batch fetch by two from cache
			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var enumerator =
					s.CreateQuery("from foo in class NHibernate.DomainModel.Foo order by foo.String").Enumerable().GetEnumerator();
				var i = 1;
				while (enumerator.MoveNext())
				{
					BarProxy bar = (BarProxy) enumerator.Current;
					if (i % 2 == 0)
					{
						string theString = bar.String; // Load the entity
					}
					i++;
				}
				tx.Commit();
			}

			Assert.That(fooCache.GetMultipleCalls, Has.Count.EqualTo(3));

			// Check that each key was used only once when retriving objects from the cache
			var uniqueKeys = new HashSet<string>();
			foreach (var keys in fooCache.GetMultipleCalls)
			{
				Assert.That(keys, Has.Length.EqualTo(2));
				foreach (var key in keys.OfType<CacheKey>().Select(o => (string) o.Key))
				{
					Assert.That(uniqueKeys, Does.Not.Contains(key));
					uniqueKeys.Add(key);
				}
			}
		}
	}
}
