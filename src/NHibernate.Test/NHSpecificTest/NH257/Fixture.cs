using System;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Dialect;
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
			cfg.AddResource("NHibernate.Test.NHSpecificTest.NH257.Mappings.hbm.xml", assembly);

			string[] script = cfg.GenerateSchemaCreationScript(new MsSql2000Dialect());

			bool found = false;

			foreach (string line in script)
			{
				if (string.Compare(
				    	line,
				    	"create table users_in_groups (group_id INT not null, user_id INT not null, primary key (user_id, group_id))",
				    	true) == 0)
				{
					found = true;
				}
			}

			Assert.IsTrue(found, "Script should contain the correct create table statement");
		}
	}
}