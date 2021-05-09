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
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class NeverCachedEntityTestsAsync : TestCase
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
		public async Task NeverInvalidateEntitiesAsync()
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

			await (cache.PreInvalidateAsync(Arg.Do<IReadOnlyCollection<string>>(x => preInvalidations.Add(x.ToList())), CancellationToken.None));
			await (cache.InvalidateAsync(Arg.Do<IReadOnlyCollection<string>>(x => invalidations.Add(x.ToList())), CancellationToken.None));

			using (var session = OpenSession())
			{
				List<int> ids = new List<int>();
				//Add NeverItem
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = new NeverItem { Name = "Abatay" };
						item.Childrens.Add(new NeverChildItem()
						{
							Name = "Child",
							Parent = item
						});
						await (session.SaveAsync(item));
						ids.Add(item.Id);
					}

					await (tx.CommitAsync());
				}

				//Update NeverItem
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in ids)
					{
						var item = await (session.GetAsync<NeverItem>(i));
						item.Name = item.Id.ToString();
					}

					await (tx.CommitAsync());
				}

				//Delete NeverItem
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in ids)
					{
						var item = await (session.GetAsync<NeverItem>(i));
						await (session.DeleteAsync(item));
					}

					await (tx.CommitAsync());
				}

				//Update NeverItem using HQL
				using (var tx = session.BeginTransaction())
				{
					await (session.CreateQuery("UPDATE NeverItem SET Name='Test'").ExecuteUpdateAsync());

					await (tx.CommitAsync());
				}

				//Update NeverItem using LINQ
				using (var tx = session.BeginTransaction())
				{
					await (session.Query<NeverItem>()
						   .UpdateBuilder()
						   .Set(x => x.Name, "Test")
						   .UpdateAsync());

					await (tx.CommitAsync());
				}
			}

			//Should receive none preinvalidation when Cache is configured as never
			Assert.That(preInvalidations, Has.Count.EqualTo(0));

			//Should receive none invalidation when Cache is configured as never
			Assert.That(invalidations, Has.Count.EqualTo(0));
		}

		[Test]
		public async Task QueryCache_ThrowsExceptionAsync()
		{
			using (var session = OpenSession())
			{
				//Linq
				using (var tx = session.BeginTransaction())
				{
					Assert.ThrowsAsync<QueryException>(() => session
					.Query<NeverItem>().WithOptions(x => x.SetCacheable(true)).ToListAsync());

					await (tx.CommitAsync());
				}

				//Linq Multiple with error message we will quarantied that gets 2 class in error message
				using (var tx = session.BeginTransaction())
				{
					Assert.ThrowsAsync<QueryException>(() => session
					.Query<NeverItem>().Where(x => x.Childrens.Any())
					.WithOptions(x => x.SetCacheable(true))
					.ToListAsync(),
					$"Never cached entity:{string.Join(", ", typeof(NeverItem).FullName, typeof(NeverChildItem).FullName)} cannot be used in cacheable query");

					await (tx.CommitAsync());
				}

				//Hql
				using (var tx = session.BeginTransaction())
				{
					Assert.ThrowsAsync<QueryException>(() => session
					.CreateQuery("from NeverItem").SetCacheable(true).ListAsync<NeverItem>());

					await (tx.CommitAsync());
				}

				//ICriteria
				using (var tx = session.BeginTransaction())
				{
					Assert.ThrowsAsync<QueryException>(() => session
					.CreateCriteria<NeverItem>()
					.SetCacheable(true)
					.ListAsync<NeverItem>());

					await (tx.CommitAsync());
				}

				//Native Sql
				using (var tx = session.BeginTransaction())
				{
					Assert.ThrowsAsync<QueryException>(() => session
					.CreateSQLQuery("select * from NeverItem")
					.AddSynchronizedQuerySpace("NeverItem")
					.SetCacheable(true)
					.ListAsync<NeverItem>());

					await (tx.CommitAsync());
				}
			}
		}

		[Test]
		public async Task ShouldAutoFlushAsync()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var e1 = new NeverItem { Name = "Abatay" };
				e1.Childrens.Add(new NeverChildItem()
				{
					Name = "Child",
					Parent = e1
				});
				await (session.SaveAsync(e1));

				var result = await ((from e in session.Query<NeverItem>()
							  where e.Name == "Abatay"
									 select e).ToListAsync());

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
