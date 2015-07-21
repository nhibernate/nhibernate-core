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
		private static readonly string[] SequenceString = new[] { "SEQUENCE STRING" };
		private static readonly string[] PoolSequenceString = new[] { "SEQUENCE STRING", "WITH INTIAL VALUE", "AND INCREMENT" };

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

			public override string GetCreateSequenceString(string sequenceName)
			{
				return SequenceString[0];
			}
		}

		private class PooledSequenceDialect : SequenceDialect
		{
			public override bool SupportsPooledSequences
			{
				get { return true; }
			}

			public override string[] GetCreateSequenceStrings(string sequenceName, int initialValue, int incrementSize)
			{
				return PoolSequenceString;
			}
		}

		/// <summary>
		/// Test explicitly specifying both optimizer and increment.
		/// </summary>
		[Test]
		public void ExplicitOptimizerWithExplicitIncrementSize()
		{
			var dialect = new SequenceDialect();

			// optimizer=none w/ increment > 1 => should honor optimizer
			var props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.None;
			props[SequenceStyleGenerator.IncrementParam] = "20";
			
			var generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.NoopOptimizer)));
			Assert.That(generator.Optimizer.IncrementSize, Is.EqualTo(1));
			Assert.That(generator.DatabaseStructure.IncrementSize, Is.EqualTo(1));

			// optimizer=hilo w/ increment > 1 => hilo
			props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.HiLo;
			props[SequenceStyleGenerator.IncrementParam] = "20";
			generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.HiLoOptimizer)));
			Assert.That(generator.Optimizer.IncrementSize, Is.EqualTo(20));
			Assert.That(generator.DatabaseStructure.IncrementSize, Is.EqualTo(20));

			// optimizer=pooled w/ increment > 1 => pooled+table
			props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.Pool;
			props[SequenceStyleGenerator.IncrementParam] = "20";
			generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			// because the dialect reports that it does not support pooled sequences, the expectation is that we will
			// use a table for the backing structure...
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (TableStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.PooledOptimizer)));
			Assert.That(generator.Optimizer.IncrementSize, Is.EqualTo(20));
			Assert.That(generator.DatabaseStructure.IncrementSize, Is.EqualTo(20));
		}

		[Test]
		public void PreferPooledLoSettingHonored()
		{
			var dialect = new PooledSequenceDialect();
			
			var props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.IncrementParam] = "20";

			var generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof(SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof(OptimizerFactory.PooledOptimizer)));

			props[Cfg.Environment.PreferPooledValuesLo] = "true";
			generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof(SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof(OptimizerFactory.PooledLoOptimizer)));
		}

		// Test all params defaulted with a dialect supporting sequences
		[Test]
		public void DefaultedSequenceBackedConfiguration()
		{
			var dialect = new SequenceDialect();
			var props = new Dictionary<string, string>();
			var generator = new SequenceStyleGenerator();

			generator.Configure(NHibernateUtil.Int64, props, dialect);

			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.NoopOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}

		// Test all params defaulted with a dialect which does not support sequences
		[Test]
		public void DefaultedTableBackedConfiguration()
		{
			var dialect = new TableDialect();
			var props = new Dictionary<string, string>();
			var generator = new SequenceStyleGenerator();

			generator.Configure(NHibernateUtil.Int64, props, dialect);

			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (TableStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.NoopOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}

		/// <summary>
		/// Test default optimizer selection for sequence backed generators
		/// based on the configured increment size; both in the case of the
		/// dialect supporting pooled sequences (pooled) and not (hilo)
		/// </summary>
		[Test]
		public void DefaultOptimizerBasedOnIncrementBackedBySequence()
		{
			var props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.IncrementParam] = "10";

			// for dialects which do not support pooled sequences, we default to pooled+table
			var dialect = new SequenceDialect();
			var generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (TableStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.PooledOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));

			// for dialects which do support pooled sequences, we default to pooled+sequence
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
			var dialect = new TableDialect();
			var props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.IncrementParam] = "10";

			var generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);

			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (TableStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.PooledOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}

		// Test forcing of table as backing strucuture with dialect supporting sequences
		[Test]
		public void ForceTableUse()
		{
			var dialect = new SequenceDialect();
			var props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.ForceTableParam] = "true";
			var generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);
			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof (TableStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof (OptimizerFactory.NoopOptimizer)));
			Assert.That(generator.DatabaseStructure.Name, Is.EqualTo(SequenceStyleGenerator.DefaultSequenceName));
		}

		[Test]
		public void NonPoolOptimizerUsedWithExplicitInitialValueUsesPooledSequenceGenerator()
		{
			var dialect = new PooledSequenceDialect();

			var props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.None;
			props[SequenceStyleGenerator.InitialParam] = "20";

			var generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);

			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof(SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof(OptimizerFactory.NoopOptimizer)));
			//Assert.That(generator.SqlCreateStrings(dialect).Length, Is.EqualTo(3));
			Assert.That(generator.SqlCreateStrings(dialect), Is.EqualTo(PoolSequenceString));
		}

		[Test]
		public void HiLoOptimizerUsedWithExplicitInitialValueUsesPooledSequenceGenerator()
		{
			Dialect.Dialect dialect = new PooledSequenceDialect();

			var props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.HiLo;
			props[SequenceStyleGenerator.InitialParam] = "20";

			var generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);

			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof(SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof(OptimizerFactory.HiLoOptimizer)));
			//Assert.That(generator.SqlCreateStrings(dialect).Length, Is.EqualTo(3));
			Assert.That(generator.SqlCreateStrings(dialect), Is.EqualTo(PoolSequenceString));
		}

		[Test]
		public void PoolOptimizerUsedWithExplicitIncrementAndInitialValueOfOneUsesNonPooledSequenceGenerator()
		{
			Dialect.Dialect dialect = new PooledSequenceDialect();

			var props = new Dictionary<string, string>();
			props[SequenceStyleGenerator.OptimizerParam] = OptimizerFactory.Pool;
			props[SequenceStyleGenerator.InitialParam] = "1";
			props[SequenceStyleGenerator.IncrementParam] = "1";

			var generator = new SequenceStyleGenerator();
			generator.Configure(NHibernateUtil.Int64, props, dialect);

			Assert.That(generator.DatabaseStructure, Is.AssignableFrom(typeof(SequenceStructure)));
			Assert.That(generator.Optimizer, Is.AssignableFrom(typeof(OptimizerFactory.PooledOptimizer)));
			//Assert.That(generator.SqlCreateStrings(dialect).Length, Is.EqualTo(1));
			Assert.That(generator.SqlCreateStrings(dialect), Is.EqualTo(SequenceString));
		}
	}
}