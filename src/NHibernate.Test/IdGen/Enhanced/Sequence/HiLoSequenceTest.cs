using System.Collections;
using NUnit.Framework;
using NHibernate.Id.Enhanced;

namespace NHibernate.Test.IdGen.Enhanced.Sequence
{
	[TestFixture]
	public class HiLoSequenceTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "IdGen.Enhanced.Sequence.HiLo.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		[Test]
		public void TestNormalBoundary()
		{
			var persister = sessions.GetEntityPersister(typeof(Entity).FullName);
			Assert.That(persister.IdentifierGenerator, Is.TypeOf<SequenceStyleGenerator>());

			var generator = (SequenceStyleGenerator)persister.IdentifierGenerator;
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
						Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(1)); // initialization
						Assert.That(optimizer.LastSourceValue, Is.EqualTo(1)); // initialization
						Assert.That(optimizer.LastValue, Is.EqualTo(i + 1));
						Assert.That(optimizer.HiValue, Is.EqualTo(increment + 1));
					}

					// now force a "clock over"
					entities[increment] = new Entity("" + increment);
					session.Save(entities[increment]);
					Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(2));
					Assert.That(optimizer.LastSourceValue, Is.EqualTo(2));
					Assert.That(optimizer.LastValue, Is.EqualTo(increment + 1));
					Assert.That(optimizer.HiValue, Is.EqualTo(increment * 2 + 1));

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