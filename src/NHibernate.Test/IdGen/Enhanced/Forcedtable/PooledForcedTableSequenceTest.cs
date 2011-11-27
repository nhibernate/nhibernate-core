using System;
using NUnit.Framework;
using NHibernate.Id.Enhanced;
using System.Collections;


namespace NHibernate.Test.IdGen.Enhanced.Forcedtable
{
	[TestFixture]
	public class PooledForcedTableSequenceTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] { "IdGen.Enhanced.Forcedtable.Pooled.hbm.xml" }; }
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
			Assert.That(generator.DatabaseStructure, Is.TypeOf<TableStructure>());
			Assert.That(generator.Optimizer, Is.TypeOf<OptimizerFactory.PooledOptimizer>());

			OptimizerFactory.PooledOptimizer optimizer = (OptimizerFactory.PooledOptimizer)generator.Optimizer;

			int increment = optimizer.IncrementSize;
			Entity[] entities = new Entity[increment + 1];
			ISession s = OpenSession();
			s.BeginTransaction();
			for (int i = 0; i < increment; i++)
			{
				entities[i] = new Entity("" + (i + 1));
				s.Save(entities[i]);
				long expectedId = i + 1;
				Assert.That(entities[i].Id, Is.EqualTo(expectedId));
				// NOTE : initialization calls table twice
				Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(2)); // initialization
				Assert.That(optimizer.LastSourceValue, Is.EqualTo(increment + 1)); // initialization
				Assert.That(optimizer.LastValue, Is.EqualTo(i + 1));
			}
			// now force a "clock over"
			entities[increment] = new Entity("" + increment);
			s.Save(entities[increment]);
			Assert.That(entities[optimizer.IncrementSize].Id, Is.EqualTo(optimizer.IncrementSize + 1));
			// initialization (2) + clock over
			Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(3));
			Assert.That(optimizer.LastSourceValue, Is.EqualTo(increment * 2 + 1));
			Assert.That(optimizer.LastValue, Is.EqualTo(increment + 1));

			s.Transaction.Commit();

			s.BeginTransaction();
			for (int i = 0; i < entities.Length; i++)
			{
				Assert.That(entities[i].Id, Is.EqualTo(i + 1));
				s.Delete(entities[i]);
			}
			s.Transaction.Commit();
			s.Close();
		}
	}
}
