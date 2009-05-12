using NHibernate.Cfg;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.MappingTest
{
	[TestFixture]
	public class NonReflectiveBinderFixture
	{
		// so far we are using this test to check metadata
		// TODO: fix the test to work without class implementations
		// all infos coming from XML should be solved Mapping classes and not
		// during parse.

		private Configuration cfg;

		[TestFixtureSetUp]
		public void SetUp()
		{
			cfg = new Configuration()
					.AddResource("NHibernate.Test.MappingTest.Wicked.hbm.xml", GetType().Assembly);
			cfg.BuildMappings();
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
			cfg = null;
		}

		[Test]
		public void MetaInheritance()
		{
			PersistentClass cm = cfg.GetClassMapping("NHibernate.Test.MappingTest.Wicked");
			var m = cm.MetaAttributes;
			Assert.That(m, Is.Not.Null);
			Assert.That(cm.GetMetaAttribute("global"), Is.Not.Null);
			Assert.That(cm.GetMetaAttribute("globalnoinherit"), Is.Null);
		}

		[Test]
		public void Comparator()
		{
			PersistentClass cm = cfg.GetClassMapping("NHibernate.Test.MappingTest.Wicked");

			Property property = cm.GetProperty("SortedEmployee");
			var col = (Mapping.Collection)property.Value;
			Assert.That(col.ComparerClassName, Text.StartsWith("NHibernate.Test.MappingTest.NonExistingComparator"));
		}
	}
}