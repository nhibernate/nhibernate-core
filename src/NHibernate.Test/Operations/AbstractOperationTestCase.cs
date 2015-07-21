using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Operations
{
	public abstract class AbstractOperationTestCase : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new[]
				       	{
				       		"Operations.Node.hbm.xml", "Operations.Employer.hbm.xml", "Operations.OptLockEntity.hbm.xml",
				       		"Operations.OneToOne.hbm.xml", "Operations.Competition.hbm.xml"
				       	};
			}
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			configuration.SetProperty(Environment.BatchSize, "0");
		}

		protected void ClearCounts()
		{
			sessions.Statistics.Clear();
		}

		protected void AssertInsertCount(long expected)
		{
			Assert.That(sessions.Statistics.EntityInsertCount, Is.EqualTo(expected), "unexpected insert count");
		}

		protected void AssertUpdateCount(int expected)
		{
			Assert.That(sessions.Statistics.EntityUpdateCount, Is.EqualTo(expected), "unexpected update count");
		}

		protected void AssertDeleteCount(int expected)
		{
			Assert.That(sessions.Statistics.EntityDeleteCount, Is.EqualTo(expected), "unexpected delete count");
		}
	}
}