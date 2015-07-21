using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Id;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1061
{
	[TestFixture]
	public class Fixture // Not inheriting from BugTestCase on purpose
	{
		[Test]
		public void IncrementGeneratorShouldIncludeClassLevelSchemaWhenGettingNextId()
		{
			System.Type thisType = GetType();
			Assembly thisAssembly = thisType.Assembly;

			Configuration cfg = new Configuration();
			cfg.AddResource(thisType.Namespace + ".Mappings.hbm.xml", thisAssembly);

			PersistentClass persistentClass = cfg.GetClassMapping(typeof(TestNH1061));
			// We know the ID generator is an IncrementGenerator.  The dialect does
			// not play a big role here, so just use the MsSql2000Dialect.
			IncrementGenerator generator =
				(IncrementGenerator)
				persistentClass.Identifier.CreateIdentifierGenerator(new Dialect.MsSql2000Dialect(), null, null, null);

			// I could not find a good seam to crack to test this.
			// This is not ideal as we are reflecting into a private variable to test.
			// On the other hand, the IncrementGenerator is rather stable, so I don't
			// think this would be a huge problem.
			// Having said that, if someone sees this and have a better idea to test,
			// please feel free to change it.
			FieldInfo sqlFieldInfo = generator.GetType().GetField("_sql", BindingFlags.NonPublic | BindingFlags.Instance);
			string sql = sqlFieldInfo.GetValue(generator).ToString();

			Assert.AreEqual("select max(Id) from test.TestNH1061", sql);
		}
	}
}
