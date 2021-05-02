using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.Test.SecondLevelCacheTests;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.SecondLevelCacheTest
{
	[TestFixture]
	public class NeverCachedEntityTests : TestCase
	{
		protected override string CacheConcurrencyStrategy => null;
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "SecondLevelCacheTest.Item.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.CacheProvider, typeof(HashtableCacheProvider).AssemblyQualifiedName);
			configuration.SetProperty(Environment.UseQueryCache, "true");
		}

		[Test]
		public void NeverInvalidateEntities()
		{
			var debugSessionFactory = (DebugSessionFactory) Sfi;

			var cache = Substitute.For<UpdateTimestampsCache>(Sfi.Settings, new Dictionary<string, string>());

			var updateTimestampsCacheField = typeof(SessionFactoryImpl).GetField(
				"updateTimestampsCache",
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			updateTimestampsCacheField.SetValue(debugSessionFactory.ActualFactory, cache);

			//"Received" assertions can not be used since the collection is reused and cleared between calls.
			//The received args are cloned and stored
			var preInvalidations = new List<IReadOnlyCollection<string>>();
			var invalidations = new List<IReadOnlyCollection<string>>();

			cache.PreInvalidate(Arg.Do<IReadOnlyCollection<string>>(x => preInvalidations.Add(x.ToList())));
			cache.Invalidate(Arg.Do<IReadOnlyCollection<string>>(x => invalidations.Add(x.ToList())));

			using (var session = OpenSession())
			{
				//Add NeverItem
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = new NeverItem 
						{
							Id = i, Name="NHibernate"+i,
							Description="Cache.Never() triggers UpdateTimestampsCache" 
						};
						session.Save(item);
					}

					tx.Commit();
				}

				//Update NeverItem
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = session.Get<NeverItem>(i);
						item.Name = item.Id.ToString();
					}

					tx.Commit();
				}

				//Delete NeverItem
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = session.Get<NeverItem>(i);
						session.Delete(item);
					}

					tx.Commit();
				}

				//Update NeverItem using HQL
				using (var tx = session.BeginTransaction())
				{
					session.CreateQuery("UPDATE NeverItem SET Name='Test'").ExecuteUpdate();

					tx.Commit();
				}

				//Update NeverItem using LINQ
				using (var tx = session.BeginTransaction())
				{
					session.Query<NeverItem>()
						   .UpdateBuilder()
						   .Set(x => x.Name, "Test")
						   .Update();

					tx.Commit();
				}
			}

			//Should receive none preinvalidation when Cache is configured as never
			Assert.That(preInvalidations, Has.Count.EqualTo(0));

			//Should receive none invalidation when Cache is configured as never
			Assert.That(invalidations, Has.Count.EqualTo(0));
		}

		[Test]
		public void QueryCache_ThrowsException()
		{
			using (var session = OpenSession())
			{
				//Linq
				using (var tx = session.BeginTransaction())
				{
					Assert.Throws<QueryException>(() => session
					.Query<NeverItem>().WithOptions(x => x.SetCacheable(true)).ToList());

					tx.Commit();
				}

				//Hql
				using (var tx = session.BeginTransaction())
				{
					Assert.Throws<QueryException>(() => session
					.CreateQuery("from NeverItem").SetCacheable(true).List());

					tx.Commit();
				}
			}
		}

		[Test]
		public void ShouldAutoFlush()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var e1 = new NeverItem { Id= 100, Name = "Abatay" };
				session.Save(e1);

				var result =(from e in session.Query<NeverItem>()
									 where e.Name == "Abatay"
							 select e).ToList();

				Assert.That(result.Count, Is.EqualTo(1));
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from NeverItem");
				tx.Commit();
			}
		}
	}
}
