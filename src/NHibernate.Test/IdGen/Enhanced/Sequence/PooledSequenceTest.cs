using NHibernate.Id.Enhanced;
using NHibernate.Test.MultiTenancy;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.Enhanced.Sequence
{
	[TestFixture(null)]
	[TestFixture("test")]
	public class PooledSequenceTest : TestCaseWithMultiTenancy
	{
		public PooledSequenceTest(string tenantIdentifier) : base(tenantIdentifier)
		{
		}
		protected override string[] Mappings
		{
			get { return new[] { "IdGen.Enhanced.Sequence.Pooled.hbm.xml" }; }
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
			Assert.That(generator.Optimizer, Is.TypeOf<OptimizerFactory.PooledOptimizer>());

			var optimizer = (OptimizerFactory.PooledOptimizer)generator.Optimizer;

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
						Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(2)); // initialization calls seq twice
						Assert.That(optimizer.GetLastSourceValue(TenantIdentifier), Is.EqualTo(increment + 1)); // initialization calls seq twice
						Assert.That(optimizer.GetLastValue(TenantIdentifier), Is.EqualTo(i + 1));
					}

					// now force a "clock over"
					entities[increment] = new Entity("" + increment);
					session.Save(entities[increment]);
					Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(3)); // initialization (2) + clock over
					Assert.That(optimizer.GetLastSourceValue(TenantIdentifier), Is.EqualTo(increment * 2 + 1)); // initialization (2) + clock over
					Assert.That(optimizer.GetLastValue(TenantIdentifier), Is.EqualTo(increment + 1));

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
