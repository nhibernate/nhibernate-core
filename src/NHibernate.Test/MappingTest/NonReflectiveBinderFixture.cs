using System.IO;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
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

			Property xp = ((NHibernate.Mapping.Component)prop.Value).GetProperty("X");
			MetaAttribute propximplements = xp.GetMetaAttribute("implements");
			Assert.That(propximplements, Is.Not.Null);
			Assert.That(propximplements.Value, Is.EqualTo("AnotherInterface"));
		}

		[Test]
		public void NonMutatedInheritance()
		{
			PersistentClass cm = cfg.GetClassMapping("NHibernate.Test.MappingTest.Wicked");
			MetaAttribute metaAttribute = cm.GetMetaAttribute("globalmutated");

			Assert.That(metaAttribute, Is.Not.Null);
			/*assertEquals( metaAttribute.getValues().size(), 2 );		
			assertEquals( "top level", metaAttribute.getValues().get(0) );*/
			Assert.That(metaAttribute.Value, Is.EqualTo("wicked level"));

			Property property = cm.GetProperty("Component");
			MetaAttribute propertyAttribute = property.GetMetaAttribute("globalmutated");

			Assert.That(propertyAttribute, Is.Not.Null);
			/*assertEquals( propertyAttribute.getValues().size(), 3 );
			assertEquals( "top level", propertyAttribute.getValues().get(0) );
			assertEquals( "wicked level", propertyAttribute.getValues().get(1) );*/
			Assert.That(propertyAttribute.Value, Is.EqualTo("monetaryamount level"));

			var component = (NHibernate.Mapping.Component)property.Value;
			property = component.GetProperty("X");
			propertyAttribute = property.GetMetaAttribute("globalmutated");

			Assert.That(propertyAttribute, Is.Not.Null);
			/*assertEquals( propertyAttribute.getValues().size(), 4 );
			assertEquals( "top level", propertyAttribute.getValues().get(0) );
			assertEquals( "wicked level", propertyAttribute.getValues().get(1) );
			assertEquals( "monetaryamount level", propertyAttribute.getValues().get(2) );*/
			Assert.That(propertyAttribute.Value, Is.EqualTo("monetaryamount x level"));

			property = cm.GetProperty("SortedEmployee");
			propertyAttribute = property.GetMetaAttribute("globalmutated");

			Assert.That(propertyAttribute, Is.Not.Null);
			/*assertEquals( propertyAttribute.getValues().size(), 3 );
			assertEquals( "top level", propertyAttribute.getValues().get(0) );
			assertEquals( "wicked level", propertyAttribute.getValues().get(1) );*/
			Assert.That(propertyAttribute.Value, Is.EqualTo("sortedemployee level"));

			property = cm.GetProperty("AnotherSet");
			propertyAttribute = property.GetMetaAttribute("globalmutated");

			Assert.That(propertyAttribute, Is.Not.Null);
			/*assertEquals( propertyAttribute.getValues().size(), 2 );
			assertEquals( "top level", propertyAttribute.getValues().get(0) );*/
			Assert.That(propertyAttribute.Value, Is.EqualTo("wicked level"));

			var bag = (Bag)property.Value;
			component = (NHibernate.Mapping.Component)bag.Element;

			Assert.That(component.MetaAttributes.Count, Is.EqualTo(4));

			metaAttribute = component.GetMetaAttribute("globalmutated");
			/*assertEquals( metaAttribute.getValues().size(), 3 );
			assertEquals( "top level", metaAttribute.getValues().get(0) );
			assertEquals( "wicked level", metaAttribute.getValues().get(1) );*/
			Assert.That(metaAttribute.Value, Is.EqualTo("monetaryamount anotherSet composite level"));

			property = component.GetProperty("Emp");
			propertyAttribute = property.GetMetaAttribute("globalmutated");

			Assert.That(propertyAttribute, Is.Not.Null);
			/*assertEquals( propertyAttribute.getValues().size(), 4 );
			assertEquals( "top level", propertyAttribute.getValues().get(0) );
			assertEquals( "wicked level", propertyAttribute.getValues().get(1) );
			assertEquals( "monetaryamount anotherSet composite level", propertyAttribute.getValues().get(2) );*/
			Assert.That(propertyAttribute.Value, Is.EqualTo("monetaryamount anotherSet composite property emp level"));

			property = component.GetProperty("Empinone");
			propertyAttribute = property.GetMetaAttribute("globalmutated");

			Assert.That(propertyAttribute, Is.Not.Null);
			/*assertEquals( propertyAttribute.getValues().size(), 4 );
			assertEquals( "top level", propertyAttribute.getValues().get(0) );
			assertEquals( "wicked level", propertyAttribute.getValues().get(1) );
			assertEquals( "monetaryamount anotherSet composite level", propertyAttribute.getValues().get(2) );*/
			Assert.That(propertyAttribute.Value, Is.EqualTo("monetaryamount anotherSet composite property empinone level"));
		}

		[Test, Ignore("Not fixed, see the TODO of this test.")]
		public void Comparator()
		{
			PersistentClass cm = cfg.GetClassMapping("NHibernate.Test.MappingTest.Wicked");

			Property property = cm.GetProperty("SortedEmployee");
			var col = (Mapping.Collection)property.Value;
			Assert.That(col.ComparerClassName, Is.StringStarting("NHibernate.Test.MappingTest.NonExistingComparator"));
		}

		[Test]
		public void ReadSubClasses()
		{
			PersistentClass cm = cfg.GetClassMapping("NHibernate.Test.MappingTest.DomesticAnimal");
			MetaAttribute metaAttribute = cm.GetMetaAttribute("Auditable");
			Assert.That(metaAttribute, Is.Not.Null);
			
			cm = cfg.GetClassMapping("NHibernate.Test.MappingTest.Cat");
			metaAttribute = cm.GetMetaAttribute("Auditable");
			Assert.That(metaAttribute, Is.Not.Null);
			
			cm = cfg.GetClassMapping("NHibernate.Test.MappingTest.Dog");
			metaAttribute = cm.GetMetaAttribute("Auditable");
			Assert.That(metaAttribute, Is.Not.Null);
		}

		[Test]
		public void XmlSerialization()
		{
			// NH-1865 (have a look to comments in JIRA)
			var mdp = new MappingDocumentParser();
			using (Stream stream = GetType().Assembly.GetManifestResourceStream("NHibernate.Test.MappingTest.Wicked.hbm.xml"))
			{
				HbmMapping mapping = mdp.Parse(stream);
				Assert.That(mapping, Is.XmlSerializable);
			}
		}
	}
}