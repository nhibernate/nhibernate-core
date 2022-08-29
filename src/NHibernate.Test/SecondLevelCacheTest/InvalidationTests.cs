using System.Collections;
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
	public class InvalidationTests : TestCase
	{
		protected override string MappingsAssembly => "NHibernate.Test";

		protected override string[] Mappings => new[] { "SecondLevelCacheTest.Item.hbm.xml" };

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.CacheProvider, typeof(HashtableCacheProvider).AssemblyQualifiedName);
			configuration.SetProperty(Environment.UseQueryCache, "true");
		}

		[Test]
		public void InvalidatesEntities()
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
				//Add Item
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = new Item { Id = i };
						session.Save(item);
					}

					tx.Commit();
				}

				//Update Item
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = session.Get<Item>(i);
						item.Name = item.Id.ToString();
					}

					tx.Commit();
				}

				//Delete Item
				using (var tx = session.BeginTransaction())
				{
					foreach (var i in Enumerable.Range(1, 10))
					{
						var item = session.Get<Item>(i);
						session.Delete(item);
					}

					tx.Commit();
				}

				//Update Item using HQL
				using (var tx = session.BeginTransaction())
				{
					session.CreateQuery("UPDATE Item SET Name='Test'").ExecuteUpdate();

					tx.Commit();
				}

				//Update Item using LINQ
				using (var tx = session.BeginTransaction())
				{
					session.Query<Item>()
						   .UpdateBuilder()
						   .Set(x => x.Name, "Test")
						   .Update();

					tx.Commit();
				}

				//Update Item using SQL
				using (var tx = session.BeginTransaction())
				{
					session.CreateSQLQuery("UPDATE Item SET Name='Test'")
						   .AddSynchronizedQuerySpace("Item")
						   .ExecuteUpdate();

					tx.Commit();
				}
			}

			//Should receive one preinvalidation per non-DML commit
			Assert.That(preInvalidations, Has.Count.EqualTo(3));
			Assert.That(preInvalidations, Has.All.Count.EqualTo(1).And.Contains("Item"));

			///...and one invalidation per commit
			Assert.That(invalidations, Has.Count.EqualTo(6));
			Assert.That(invalidations, Has.All.Count.EqualTo(1).And.Contains("Item"));
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Item");
				tx.Commit();
			}
		}
	}
}
