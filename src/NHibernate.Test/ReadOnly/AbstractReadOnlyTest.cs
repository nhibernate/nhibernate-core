using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.ReadOnly
{
	public abstract class AbstractReadOnlyTest : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}
		
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.GenerateStatistics, "true");
			configuration.SetProperty(Environment.BatchSize, "0");
		}

		protected override ISession OpenSession()
		{
			ISession session = base.OpenSession();
			session.CacheMode = CacheMode.Ignore;
			return session;
		}
		
		protected void ClearCounts()
		{
			Sfi.Statistics.Clear();
		}
		
		protected void AssertUpdateCount(int count)
		{
			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(count));
		}
		
		protected void AssertInsertCount(int count)
		{
			Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(count));
		}

		protected void AssertDeleteCount(int count)
		{
			Assert.That(Sfi.Statistics.EntityDeleteCount, Is.EqualTo(count));
		}
	}
}
