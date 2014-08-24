using System;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.DomainModel;
using NUnit.Framework;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Test.QueryTest
{
	[TestFixture]
	public class CountFixture
	{
		[Test]
		public void Default()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.AddResource("NHibernate.DomainModel.Simple.hbm.xml", typeof(Simple).Assembly);
			cfg.SetProperty(Environment.Hbm2ddlAuto, "create-drop");
			ISessionFactory sf = cfg.BuildSessionFactory();

			using (ISession s = sf.OpenSession())
			{
				object count = s.CreateQuery("select count(*) from Simple").UniqueResult();
				Assert.IsTrue(count is Int64);
			}
			sf.Close();
		}

		[Test]
		public void Overridden()
		{
			Configuration cfg = TestConfigurationHelper.GetDefaultConfiguration();
			cfg.SetProperty(Environment.Hbm2ddlAuto, "create-drop");
			cfg.AddResource("NHibernate.DomainModel.Simple.hbm.xml", typeof(Simple).Assembly);
			cfg.AddSqlFunction("count", new ClassicCountFunction());

			ISessionFactory sf = cfg.BuildSessionFactory();

			using (ISession s = sf.OpenSession())
			{
				object count = s.CreateQuery("select count(*) from Simple").UniqueResult();
				Assert.IsTrue(count is Int32);
			}
			sf.Close();
		}
	}
}