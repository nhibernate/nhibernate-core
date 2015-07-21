using System;
using NHibernate.Cfg;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.NHSpecificTest.NH873
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void CacheDisabled()
		{
			Configuration cfg = new Configuration();
			cfg.SetProperty(Environment.UseSecondLevelCache, "false");
			cfg.SetProperty(Environment.UseQueryCache, "false");
			cfg.SetProperty(Environment.CacheProvider, null);
			cfg.BuildSessionFactory().Close();
		}
	}
}