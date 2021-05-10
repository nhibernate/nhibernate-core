using System;
using System.Collections.Generic;
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
			mapper.Class<TestEntity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.Assigned));
			});

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
		public void ShouldWorkWithSystemTransaction()
		{
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
	}
}
