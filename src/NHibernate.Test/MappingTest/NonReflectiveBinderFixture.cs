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

			MetaAttribute metaAttribute = cm.GetMetaAttribute("implements");
			Assert.That(metaAttribute, Is.Not.Null);
			Assert.That(metaAttribute.Name, Is.EqualTo("implements"));
			Assert.That(metaAttribute.IsMultiValued);
			var values = metaAttribute.Values;
			Assert.That(values.Count, Is.EqualTo(3));
			Assert.That(values[0], Is.EqualTo("IObserver"));
			Assert.That(values[1], Is.EqualTo("IObserver"));
			Assert.That(values[2], Is.EqualTo("Foo.BogusVisitor"));

			foreach (var element in cm.PropertyIterator)
			{
				var ma = element.MetaAttributes;
				Assert.That(ma, Is.Not.Null);
				Assert.That(element.GetMetaAttribute("global"), Is.Not.Null,"inherited attribute missing for prop {0}",element.Name);
				MetaAttribute metaAttribute2 = element.GetMetaAttribute("implements");
				Assert.That(metaAttribute2, Is.Not.Null);
				Assert.That(element.GetMetaAttribute("globalnoinherit"), Is.Null);
			}

			Property prop = cm.GetProperty("Component");
			var map = prop.MetaAttributes;
			Assert.That(map, Is.Not.Null);
			Assert.That(prop.GetMetaAttribute("global"), Is.Not.Null);
			Assert.That(prop.GetMetaAttribute("componentonly"), Is.Not.Null);
			Assert.That(prop.GetMetaAttribute("allcomponent"), Is.Not.Null);
			Assert.That(prop.GetMetaAttribute("globalnoinherit"), Is.Null);

			MetaAttribute compimplements = prop.GetMetaAttribute("implements");
			Assert.That(compimplements, Is.Not.Null);
			Assert.That(compimplements.Value, Is.EqualTo("AnotherInterface"));

			Property xp = ((Component)prop.Value).GetProperty("X");
			MetaAttribute propximplements = xp.GetMetaAttribute("implements");
			Assert.That(propximplements, Is.Not.Null);
			Assert.That(propximplements.Value, Is.EqualTo("AnotherInterface"));
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