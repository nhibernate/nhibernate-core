using System.Collections;
using NHibernate.Id.Enhanced;
using NHibernate.Test.MultiTenancy;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.Enhanced.Table
{
	[TestFixture(null)]
	[TestFixture("test")]
	public class PooledLoTableTest : TestCaseWithMultiTenancy
	{
		public PooledLoTableTest(string tenantIdentifier) : base(tenantIdentifier)
		{
		}
		protected override string[] Mappings
		{
			get { return new[] { "IdGen.Enhanced.Table.PooledLo.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void TestNormalBoundary()
		{
			var persister = Sfi.GetEntityPersister(typeof(Entity).FullName);
			Assert.That(persister.IdentifierGenerator, Is.TypeOf<TableGenerator>());

			var generator = (TableGenerator)persister.IdentifierGenerator;
			Assert.That(generator.Optimizer, Is.TypeOf<OptimizerFactory.PooledLoOptimizer>());
			var optimizer = (OptimizerFactory.PooledLoOptimizer)generator.Optimizer;

			int increment = optimizer.IncrementSize;
			Entity[] entities = new Entity[increment + 1];

			using (ISession s = OpenSession())
			{
				using (ITransaction transaction = s.BeginTransaction())
				{
					for (int i = 0; i < increment; i++)
					{
						entities[i] = new Entity("" + (i + 1));
						s.Save(entities[i]);
						Assert.That(generator.TableAccessCount, Is.EqualTo(1)); // initialization
						if (TenantIdentifier == null)
						{
							Assert.That(optimizer.LastSourceValue, Is.EqualTo(1)); // initialization
							Assert.That(optimizer.LastValue, Is.EqualTo(i + 1));
						}
						Assert.That(optimizer.GetLastSourceValue(TenantIdentifier), Is.EqualTo(1)); // initialization
						Assert.That(optimizer.GetLastValue(TenantIdentifier), Is.EqualTo(i + 1));
					}

					// now force a "clock over"
					entities[increment] = new Entity("" + increment);
					s.Save(entities[increment]);
					Assert.That(generator.TableAccessCount, Is.EqualTo(2));
					if (TenantIdentifier == null)
					{
						Assert.That(optimizer.LastSourceValue, Is.EqualTo(increment + 1));
						Assert.That(optimizer.LastValue, Is.EqualTo(increment + 1));
					}
					Assert.That(optimizer.GetLastSourceValue(TenantIdentifier), Is.EqualTo(increment + 1));
					Assert.That(optimizer.GetLastValue(TenantIdentifier), Is.EqualTo(increment + 1));

					transaction.Commit();
				}

				using (ITransaction transaction = s.BeginTransaction())
				{
					for (int i = 0; i < entities.Length; i++)
					{
						Assert.That(entities[i].Id, Is.EqualTo(i + 1));
						s.Delete(entities[i]);
					}
					transaction.Commit();
				}
			}
		}
	}
}
