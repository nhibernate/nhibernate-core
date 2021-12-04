using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2750
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<TestEntity>(rc => { rc.Id(x => x.Id, m => m.Generator(Generators.Assigned)); });

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.BatchSize, 10.ToString());
			configuration.SetProperty(Cfg.Environment.UseConnectionOnSystemTransactionPrepare, true.ToString());
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.CreateQuery("delete from System.Object").ExecuteUpdate();

				transaction.Commit();
			}
		}

		[Test]
		public void ShouldWorkWithOuterSystemTransaction()
		{
			if (!Sfi.ConnectionProvider.Driver.SupportsSystemTransactions)
				Assert.Ignore("System.Transactions support is required by this test");

			const int count = 3;
			var entities = new List<TestEntity>(count);
			for (var i = 0; i < count; i++)
			{
				entities.Add(new TestEntity { Id = Guid.NewGuid() });
			}

			using (var transaction = new TransactionScope())
			{
				using (var session = Sfi.OpenStatelessSession())
				{
					foreach (var entity in entities)
					{
						session.Insert(entity);
					}
				}

				transaction.Complete();
			}

			using (var session = OpenSession())
			{
				var results = session.QueryOver<TestEntity>().List<TestEntity>();

				Assert.That(results.Count, Is.EqualTo(count));
			}
		}

		[Test]
		public void ShouldWorkWithInnerSystemTransaction()
		{
			if (!Sfi.ConnectionProvider.Driver.SupportsSystemTransactions)
				Assert.Ignore("System.Transactions support is required by this test");

			const int count = 3;
			var entities = new List<TestEntity>(count);
			for (var i = 0; i < count; i++)
			{
				entities.Add(new TestEntity { Id = Guid.NewGuid() });
			}

			using (var session = Sfi.OpenStatelessSession())
			{
				using (var transaction = new TransactionScope())
				{
					foreach (var entity in entities)
					{
						session.Insert(entity);
					}
					session.FlushBatcher();

					transaction.Complete();
				}
			}

			using (var session = OpenSession())
			{
				var results = session.QueryOver<TestEntity>().List<TestEntity>();

				Assert.That(results.Count, Is.EqualTo(count));
			}
		}

		[Test]
		public void ShouldWorkWithoutTransaction()
		{
			const int count = 3;
			var entities = new List<TestEntity>(count);
			for (var i = 0; i < count; i++)
			{
				entities.Add(new TestEntity { Id = Guid.NewGuid() });
			}

			using (var session = Sfi.OpenStatelessSession())
			{
				foreach (var entity in entities)
				{
					session.Insert(entity);
				}
			}

			using (var session = OpenSession())
			{
				var results = session.QueryOver<TestEntity>().List<TestEntity>();

				Assert.That(results.Count, Is.EqualTo(count));
			}
		}

		[Test]
		public void ShouldGetEntityAfterInsert()
		{
			var entity = new TestEntity { Id = Guid.NewGuid() };
			using (var session = Sfi.OpenStatelessSession())
			{
				session.Insert(entity);
				Assert.That(session.Get<TestEntity>(entity.Id), Is.Not.Null);
			}
		}

		[Test]
		public void ShouldQueryEntityAfterInsert()
		{
			var entity = new TestEntity { Id = Guid.NewGuid() };
			using (var session = Sfi.OpenStatelessSession())
			{
				session.Insert(entity);
				Assert.That(session.Query<TestEntity>().FirstOrDefault(e => e.Id == entity.Id), Is.Not.Null);
			}
		}

		[Test]
		public void ShouldQueryOverEntityAfterInsert()
		{
			var entity = new TestEntity { Id = Guid.NewGuid() };
			using (var session = Sfi.OpenStatelessSession())
			{
				session.Insert(entity);
				Assert.That(session.QueryOver<TestEntity>().Where(e => e.Id == entity.Id).SingleOrDefault(), Is.Not.Null);
			}
		}

		[Test]
		public void ShouldHqlQueryEntityAfterInsert()
		{
			var entity = new TestEntity { Id = Guid.NewGuid() };
			using (var session = Sfi.OpenStatelessSession())
			{
				session.Insert(entity);
				Assert.That(session.CreateQuery("from TestEntity where Id = :id").SetGuid("id", entity.Id).UniqueResult(), Is.Not.Null);
			}
		}
	}
}
