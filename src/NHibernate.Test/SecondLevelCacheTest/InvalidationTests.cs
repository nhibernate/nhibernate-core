using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Impl;
using NHibernate.Test.SecondLevelCacheTests;
using NSubstitute;
using NUnit.Framework;

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
			configuration.Properties[Environment.CacheProvider] = typeof(HashtableCacheProvider).AssemblyQualifiedName;
			configuration.Properties[Environment.UseQueryCache] = "true";
		}

		[Test]
		public void InvalidatesEntities()
		{
			var cache = Substitute.For<UpdateTimestampsCache>(Sfi.Settings, new Dictionary<string, string>());
			((SessionFactoryImpl) (Sfi as DebugSessionFactory).ActualFactory).SetPropertyUsingReflection(
				x => x.UpdateTimestampsCache,
				cache);

			//"Received" assertions can not be used since the collection is reused and cleared between calls.
			//The received args are cloned and stored
			var preInvalidations = new List<IReadOnlyCollection<string>>();
			var invalidations = new List<IReadOnlyCollection<string>>();

			cache
				.When(x=>x.PreInvalidate(Arg.Any<IReadOnlyCollection<string>>()))
				.Do(x=>preInvalidations.Add(((IReadOnlyCollection<string>) x[0]).ToList()));
			cache
				.When(x => x.Invalidate(Arg.Any<IReadOnlyCollection<string>>()))
				.Do(x => invalidations.Add(((IReadOnlyCollection<string>) x[0]).ToList()));

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
			Assert.That(preInvalidations.Count,Is.EqualTo(3));
			Assert.That(preInvalidations.All(x => x.Count == 1 && x.First() == "Item"), Is.True);

			Assert.That(invalidations.Count, Is.EqualTo(3));
			Assert.That(invalidations.All(x => x.Count == 1 && x.First() == "Item"), Is.True);

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
