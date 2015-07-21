using System;
using System.Xml;
using NHibernate.Mapping;
using NHibernate.Properties;
using NUnit.Framework;

namespace NHibernate.Test.EntityModeTest.Xml.Accessors
{
	[TestFixture]
	public class XmlAccessorFixture
	{
		public static XmlElement dom = GenerateTestElement();

		private static XmlElement GenerateTestElement()
		{
			const string xml = 
@"<company id='123'>
	description...
	<name>NHForge</name>
	<account num='456'/>
</company>";

			var baseXml = new XmlDocument();
			baseXml.LoadXml(xml);
			return baseXml.DocumentElement;
		}

		private static XmlElement GenerateRootTestElement()
		{
			return (new XmlDocument()).CreateElement("company");
		}

		private static Property GenerateAccountIdProperty()
		{
			var value = new SimpleValue {TypeName = "long"};

			return new Property {Name = "number", NodeName = "account/@num", Value = value};
		}

		private static Property GenerateTextProperty()
		{
			var value = new SimpleValue {TypeName = "string"};

			return new Property {Name = "text", NodeName = ".", Value = value};
		}

		private static Property GenerateNameProperty()
		{
			var value = new SimpleValue {TypeName = "string"};

			return new Property {Name = "name", NodeName = "name", Value = value};
		}

		private static Property GenerateIdProperty()
		{
			var value = new SimpleValue {TypeName = "long"};

			return new Property {Name = "id", NodeName = "@id", Value = value};
		}

		[Test]
		public void CompanyElementGeneration()
		{
			ISetter idSetter = PropertyAccessorFactory.GetPropertyAccessor(GenerateIdProperty(), EntityMode.Xml).GetSetter(null,
			                                                                                                               null);
			ISetter nameSetter =
				PropertyAccessorFactory.GetPropertyAccessor(GenerateNameProperty(), EntityMode.Xml).GetSetter(null, null);
			ISetter textSetter =
				PropertyAccessorFactory.GetPropertyAccessor(GenerateTextProperty(), EntityMode.Xml).GetSetter(null, null);
			ISetter accountIdSetter =
				PropertyAccessorFactory.GetPropertyAccessor(GenerateAccountIdProperty(), EntityMode.Xml).GetSetter(null, null);

			XmlNode root = GenerateRootTestElement();

			idSetter.Set(root, 123L);
			textSetter.Set(root, "description...");
			nameSetter.Set(root, "NHForge");
			accountIdSetter.Set(root, 456L);

			Console.WriteLine(dom.OuterXml);
			//Assert.That(new NodeComparator().Compare(dom, root) == 0);
		}

		[Test]
		public void LongAttributeExtraction()
		{
			Property property = GenerateIdProperty();
			IGetter getter = PropertyAccessorFactory.GetPropertyAccessor(property, EntityMode.Xml).GetGetter(null, null);
			var id = (long) getter.Get(dom);
			Assert.That(id, Is.EqualTo(123L));
		}

		[Test]
		public void LongElementAttributeExtraction()
		{
			Property property = GenerateAccountIdProperty();
			IGetter getter = PropertyAccessorFactory.GetPropertyAccessor(property, EntityMode.Xml).GetGetter(null, null);
			var id = (long) getter.Get(dom);
			Assert.That(id, Is.EqualTo(456L));
		}

		[Test]
		public void StringElementExtraction()
		{
			Property property = GenerateNameProperty();
			IGetter getter = PropertyAccessorFactory.GetPropertyAccessor(property, EntityMode.Xml).GetGetter(null, null);
			var name = (string) getter.Get(dom);
			Assert.That(name, Is.EqualTo("NHForge"));
		}

		[Test, Ignore("Not supported yet.")]
		public void StringTextExtraction()
		{
			Property property = GenerateTextProperty();
			IGetter getter = PropertyAccessorFactory.GetPropertyAccessor(property, EntityMode.Xml).GetGetter(null, null);
			var name = (string) getter.Get(dom);
			Assert.That(name, Is.EqualTo("description..."));
		}
	}
}