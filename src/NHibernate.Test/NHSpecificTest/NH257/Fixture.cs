using System;
using System.Reflection;

using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH257
{
	[TestFixture]
	public class Fixture
	{
		[Test]
		public void ManyToManyTableCreationScript()
		{
			Configuration cfg = new Configuration();
			Assembly assembly = Assembly.GetExecutingAssembly();
			cfg.AddResource( "NHibernate.Test.NHSpecificTest.NH257.Mappings.hbm.xml", assembly );
			
			string[] script = cfg.GenerateSchemaCreationScript(new Dialect.MsSql2000Dialect());
			string createManyToManyTable = script[1];
			Assert.AreEqual("create table users_in_groups (group_id INT not null, user_id INT not null, primary key (user_id, group_id))",
				createManyToManyTable);
		}
	}
}
