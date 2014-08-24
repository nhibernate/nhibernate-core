using System;

using NHibernate.Cfg;
using NHibernate.Dialect;

using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH930
{
	[TestFixture]
	public class NH930Fixture
	{
		[Test]
		public void DuplicateConstraints()
		{
			Configuration cfg = new Configuration();
			cfg.AddResource(GetType().Namespace + ".Mappings.hbm.xml", GetType().Assembly);
			string[] script = cfg.GenerateSchemaCreationScript(new MsSql2000Dialect());

			int constraintCount = 0;
			foreach (string str in script)
			{
				if (str.IndexOf("foreign key (DependentVariableId) references NVariable") >= 0)
				{
					constraintCount++;
				}
			}
			Assert.AreEqual(1, constraintCount);
		}
	}
}
