using System;
using System.Collections;
using NUnit.Framework;
using NHibernate.Id.Enhanced;


namespace NHibernate.Test.IdGen.Enhanced.Sequence
{
	[TestFixture]
	public class PooledSequenceTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] { "IdGen.Enhanced.Sequence.Pooled.hbm.xml" }; }
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
				Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(2)); // initialization calls seq twice
				Assert.That(optimizer.LastSourceValue, Is.EqualTo(increment + 1)); // initialization calls seq twice
				Assert.That(optimizer.LastValue, Is.EqualTo(i + 1));
			}
			// now force a "clock over"
			entities[increment] = new Entity("" + increment);
			s.Save(entities[increment]);
			Assert.That(generator.DatabaseStructure.TimesAccessed, Is.EqualTo(3)); // initialization (2) + clock over
			Assert.That(optimizer.LastSourceValue, Is.EqualTo(increment * 2 + 1)); // initialization (2) + clock over
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
