using System;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH283
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void ForeignKeyNames()
		{
			Configuration cfg = new Configuration();
			Assembly assembly = Assembly.GetExecutingAssembly();
			cfg.AddResource("NHibernate.DomainModel.MasterDetail.hbm.xml",
			                Assembly.GetAssembly(typeof(Master))
				);


			string script = string.Join("\n",
			                            cfg.GenerateSchemaCreationScript(new MsSql2000Dialect()));

			Assert.IsTrue(script.IndexOf("add constraint AA") >= 0);
			Assert.IsTrue(script.IndexOf("add constraint BB") >= 0);
			Assert.IsTrue(script.IndexOf("add constraint CC") >= 0);
		}
	}
}