using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH873
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void CacheDisabled()
		{
			Configuration cfg = new Configuration();
			cfg.SetProperty(Cfg.Environment.UseSecondLevelCache, "false");
			cfg.SetProperty(Cfg.Environment.UseQueryCache, "false");
			cfg.SetProperty(Cfg.Environment.CacheProvider, null);
			cfg.BuildSessionFactory().Close();
		}
	}
}
