using NHibernate.Id.Enhanced;
using NHibernate.Test.MultiTenancy;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.Enhanced.Forcedtable
{
	[TestFixture(null)]
	[TestFixture("test")]
	public class HiLoForcedTableSequenceTest : TestCaseWithMultiTenancy
	{
		public HiLoForcedTableSequenceTest(string tenantIdentifier) : base(tenantIdentifier)
		{
		}
		protected override string[] Mappings
		{
			get { return new[] { "IdGen.Enhanced.Forcedtable.HiLo.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void TestNormalBoundary()
		{
			var persister = Sfi.GetEntityPersister(typeof(Entity).FullName);
			Assert.That(persister.IdentifierGenerator, Is.TypeOf<SequenceStyleGenerator>());

			var generator = (SequenceStyleGenerator)persister.IdentifierGenerator;
			Assert.That(generator.DatabaseStructure, Is.TypeOf<TableStructure>());
			Assert.That(generator.Optimizer, Is.TypeOf<OptimizerFactory.HiLoOptimizer>());

			var optimizer = (OptimizerFactory.HiLoOptimizer)generator.Optimizer;

			int increment = optimizer.IncrementSize;
			var entities = new Entity[increment + 1];

			using (ISession session = OpenSession())
			{
				using (ITransaction transaction = session.BeginTransaction())
				{
					for (int i = 0; i < increment; i++)
					{
						entities[i] = new Entity("" + (i + 1));
						session.Save(entities[i]);
						long expectedId = i + 1;
						Assert.That(entities[i].Id, Is.EqualTo(expectedId));
						Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(1)); // initialization
						if (TenantIdentifier == null)
						{
							Assert.That(optimizer.LastSourceValue, Is.EqualTo(1)); // initialization
							Assert.That(optimizer.LastValue, Is.EqualTo(i + 1));
							Assert.That(optimizer.HiValue, Is.EqualTo(increment + 1));
						}
						Assert.That(optimizer.GetLastSourceValue(TenantIdentifier), Is.EqualTo(1)); // initialization
						Assert.That(optimizer.GetLastValue(TenantIdentifier), Is.EqualTo(i + 1));
						Assert.That(optimizer.GetHiValue(TenantIdentifier), Is.EqualTo(increment + 1));
					}

					// now force a "clock over"
					entities[increment] = new Entity("" + increment);
					session.Save(entities[increment]);
					Assert.That(entities[optimizer.IncrementSize].Id, Is.EqualTo(optimizer.IncrementSize + 1));
					Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(2)); // initialization + clock-over
					if (TenantIdentifier == null)
					{
						Assert.That(optimizer.LastSourceValue, Is.EqualTo(2)); // initialization + clock-over
						Assert.That(optimizer.LastValue, Is.EqualTo(increment + 1));
						Assert.That(optimizer.HiValue, Is.EqualTo(increment * 2 + 1));
					}
					Assert.That(optimizer.GetLastSourceValue(TenantIdentifier), Is.EqualTo(2)); // initialization + clock-over
					Assert.That(optimizer.GetLastValue(TenantIdentifier), Is.EqualTo(increment + 1));
					Assert.That(optimizer.GetHiValue(TenantIdentifier), Is.EqualTo(increment * 2 + 1));

					transaction.Commit();
				}

				using (ITransaction transaction = session.BeginTransaction())
				{
					for (int i = 0; i < entities.Length; i++)
					{
						Assert.That(entities[i].Id, Is.EqualTo(i + 1));
						session.Delete(entities[i]);
					}
					transaction.Commit();
				}

				session.Close();
			}
		}
	}
}
