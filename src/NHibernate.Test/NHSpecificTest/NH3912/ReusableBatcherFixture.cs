using System;
using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3912
{
	[TestFixture]
	public class ReusableBatcherFixture : TestCaseMappingByCode
	{
		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			var driver = factory.ConnectionProvider.Driver;
			return driver.IsOracleDataClientDriver() ||
			       driver.IsOracleLiteDataClientDriver() ||
			       driver.IsOracleManagedDataClientDriver();
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<BatcherLovingEntity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.Name, m => m.Unique(true));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Cfg.Environment.BatchStrategy,
			                          typeof(OracleDataClientBatchingBatcherFactory).AssemblyQualifiedName);
		}

		protected override void OnSetUp()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var e1 = new BatcherLovingEntity { Name = "Bob" };
				session.Save(e1);

				var e2 = new BatcherLovingEntity { Name = "Sally" };
				session.Save(e2);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		/// <summary> Batch operations with the same IBatcher instance fail on expect rows count after single failed operation.  </summary>
		[Test]
		public void Test_Batcher_Is_Reusable_After_Failed_Operation()
		{
			using (var session = OpenSession())
			{
				try
				{
					using (session.BeginTransaction())
					{
						var valid = new BatcherLovingEntity { Name = "Bill" };
						session.Save(valid);

						Assert.That(() => session.Query<BatcherLovingEntity>().Count(x => x.Name == "Bob"), Is.EqualTo(1));
						var bob = new BatcherLovingEntity { Name = "Bob" };
						session.Save(bob);

						// Should fail on unique constraint violation
						// Expected behavior
						session.Flush();
					}
				}
				catch (Exception)
				{
					// Opening next transaction in the same session after rollback
					// to log the problem, for instance.
					// Executing in the same session with the same instance of IBatcher
					using (session.BeginTransaction())
					{
						// Inserting any valid entity will fail on expected rows count assert in batcher
						var e1 = new BatcherLovingEntity { Name = "Robert (because Bob already exists)" };
						session.Save(e1);
						// Batch update returned unexpected row count from update; actual row count: 1; expected: 2
						session.Flush();
					}
				}
			}
		}
	}
}
