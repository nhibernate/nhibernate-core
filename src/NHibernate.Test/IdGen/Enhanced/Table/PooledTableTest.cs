using System.Collections;
using NHibernate.Id.Enhanced;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.Enhanced.Table
{
	[TestFixture]
	public class PooledTableTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "IdGen.Enhanced.Table.Pooled.hbm.xml" }; }
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
			Assert.That(generator.Optimizer, Is.TypeOf<OptimizerFactory.PooledOptimizer>());
			var optimizer = (OptimizerFactory.PooledOptimizer)generator.Optimizer;

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
						Assert.That(generator.TableAccessCount, Is.EqualTo(2)); // initialization calls seq twice
						Assert.That(optimizer.LastSourceValue, Is.EqualTo(increment + 1));  // initialization calls seq twice
						Assert.That(optimizer.LastValue, Is.EqualTo(i + 1));
					}

					// now force a "clock over"
					entities[increment] = new Entity("" + increment);
					s.Save(entities[increment]);
					Assert.That(generator.TableAccessCount, Is.EqualTo(3)); // initialization (2) + clock over
					Assert.That(optimizer.LastSourceValue, Is.EqualTo((increment * 2) + 1)); // initialization (2) + clock over
					Assert.That(optimizer.LastValue, Is.EqualTo(increment + 1));

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
