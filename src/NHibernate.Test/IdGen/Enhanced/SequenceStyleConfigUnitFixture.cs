using System.Collections.Generic;
using NHibernate.Id.Enhanced;
using NUnit.Framework;

namespace NHibernate.Test.IdGen.Enhanced
{
	/// <summary>
	/// Tests that SequenceStyleGenerator configures itself as expected in various scenarios
	/// </summary>
	[TestFixture]
	public class SequenceStyleConfigUnitFixture
	{
		private class TableDialect : Dialect.Dialect
		{
			public override bool SupportsSequences
			{
				get { return false; }
			}
		}

		private class SequenceDialect : Dialect.Dialect
		{
			public override bool SupportsSequences
			{
				get { return true; }
			}

			public override bool SupportsPooledSequences
			{
				get { return false; }
			}

			public override string GetSequenceNextValString(string sequenceName)
			{
				return string.Empty;
			}
		}

		private class PooledSequenceDialect : SequenceDialect
		{
			public override bool SupportsPooledSequences
			{
				get { return true; }
			}
		}

		// Test explicitly specifying both optimizer and increment
		[Test]
		public void ExplicitOptimizerWithExplicitIncrementSize()
		{
			// with sequence ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
			Dialect.Dialect dialect = new SequenceDialect();

			// optimizer=none w/ increment > 1 => should honor optimizer
			IDictionary<string, string> props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.None;
			props[SequenceStyleGenerator.IncrementParam] = "20";
			SequenceStyleGenerator generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.NoopOptimizer)));
			Assert.AreEqual(1, generator.Optimizer.IncrementSize);
			Assert.AreEqual(1, generator.DatabaseStructure.IncrementSize);

			// optimizer=hilo w/ increment > 1 => hilo
			props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.HiLo;
			props[SequenceStyleGenerator.IncrementParam] = "20";
			generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.HiLoOptimizer)));
			Assert.AreEqual(20, generator.Optimizer.IncrementSize);
			Assert.AreEqual(20, generator.DatabaseStructure.IncrementSize);

			// optimizer=pooled w/ increment > 1 => hilo
			props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.Pool;
			props[SequenceStyleGenerator.IncrementParam] = "20";
			generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.HiLoOptimizer)));
			Assert.AreEqual(20, generator.Optimizer.IncrementSize);
			Assert.AreEqual(20, generator.DatabaseStructure.IncrementSize);
		}

		// Test all params defaulted with a dialect supporting sequences
		[Test]
		public void DefaultedSequenceBackedConfiguration()
		{
			Dialect.Dialect dialect = new SequenceDialect();
			IDictionary<string, string> props = new Dictionary<string, string>();
			SequenceStyleGenerator generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);

			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.NoopOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}

		// Test all params defaulted with a dialect which does not support sequences
		[Test]
		public void DefaultedTableBackedConfiguration()
		{
			Dialect.Dialect dialect = new TableDialect();
			IDictionary<string, string> props = new Dictionary<string, string>();
			SequenceStyleGenerator generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);

			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (TableStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.NoopOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}

		//Test default optimizer selection for sequence backed generators
		//based on the configured increment size; both in the case of the
		//dialect supporting pooled sequences (pooled) and not (hilo)
		[Test]
		public void DefaultOptimizerBasedOnIncrementBackedBySequence()
		{
			IDictionary<string, string> props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.IncrementParam] = "10";

			// for dialects which do not support pooled sequences, we default to hilo
			Dialect.Dialect dialect = new SequenceDialect();
			SequenceStyleGenerator generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.HiLoOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));

			// for dialects which do support pooled sequences, we default to pooled
			dialect = new PooledSequenceDialect();
			generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.PooledOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}

		// Test default optimizer selection for table backed generators
		// based on the configured increment size.  Here we always prefer	 pooled.
		[Test]
		public void DefaultOptimizerBasedOnIncrementBackedByTable()
		{
			IDictionary<string, string> props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.IncrementParam] = "10";
			Dialect.Dialect dialect = new TableDialect();
			SequenceStyleGenerator generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (TableStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.PooledOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}

		// Test forcing of table as backing strucuture with dialect supporting sequences
		[Test]
		public void ForceTableUse()
		{
			Dialect.Dialect dialect = new SequenceDialect();
			IDictionary<string, string> props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.ForceTableParam] = "true";
			SequenceStyleGenerator generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (TableStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.NoopOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}
	}
}