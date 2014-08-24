using System.Collections;
using NUnit.Framework;
using NHibernate.Id.Enhanced;

namespace NHibernate.Test.IdGen.Enhanced.Sequence
{
	[TestFixture(true, typeof(OptimizerFactory.PooledLoOptimizer))]
	[TestFixture(false, typeof(OptimizerFactory.PooledOptimizer))]
	public class DefaultOptimizedSequenceTest : TestCase
	{
		private readonly bool _preferLo;
		private readonly System.Type _expectedOptimizerType;

		public DefaultOptimizedSequenceTest(bool preferLo, System.Type expectedOptimizerType)
		{
			_preferLo = preferLo;
			_expectedOptimizerType = expectedOptimizerType;
		}

		protected override IList Mappings
		{
			get { return new[] { "IdGen.Enhanced.Sequence.DefaultOptimized.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override void AddMappings(Cfg.Configuration configuration)
		{
			// Set some properties that must be set before the mappings are added.
			// (The overridable Configure(cfg) is called AFTER AddMappings(cfg).)
			configuration.SetProperty(Cfg.Environment.PreferPooledValuesLo, _preferLo.ToString().ToLower());

			base.AddMappings(configuration);
		}

		[Test]
		public void CorrectOptimizerChosenAsDefault()
		{
			// This test is to verify that a pooled optimizer is chosen by default
			// if an increment size greater than 1 is specified, and that the global
			// property Cfg.Environment.PreferPooledValuesLo takes effect.

			var persister = sessions.GetEntityPersister(typeof(Entity).FullName);
			Assert.That(persister.IdentifierGenerator, Is.TypeOf<SequenceStyleGenerator>());

			var generator = (SequenceStyleGenerator)persister.IdentifierGenerator;
			Assert.That(generator.Optimizer, Is.TypeOf(_expectedOptimizerType));
		}
	}
}