using System;
using System.Xml;
using NHibernate.Mapping;
using NHibernate.Properties;
using NUnit.Framework;
using System.Dynamic;

namespace NHibernate.Test.EntityModeTest.Dynamic.Accessors
{
    [TestFixture]
    public class DynamicAccessorFixture
    {
        public static object dyn = GenerateTestElement();

        private static dynamic GenerateTestElement()
        {
            dynamic t = new ExpandoObject();
            t.id = 123;
            t.description = "description...";
            t.name = "NHForge";
            //t.account.num = 456;
            return t;
        }

        private static XmlElement GenerateRootTestElement()
        {
            return (new XmlDocument()).CreateElement("company");
        }

        private static Property GenerateAccountIdProperty()
        {
            var value = new SimpleValue { TypeName = "long" };

            return new Property { Name = "number", Value = value };
        }

        private static Property GenerateTextProperty()
        {
            var value = new SimpleValue { TypeName = "string" };

            return new Property { Name = "text",  Value = value };
        }

        private static Property GenerateNameProperty()
        {
            var value = new SimpleValue { TypeName = "string" };

            return new Property { Name = "name", Value = value };
        }

        private static Property GenerateIdProperty()
        {
            var value = new SimpleValue { TypeName = "long" };

            return new Property { Name = "id", Value = value };
        }
             
        [Test]
        public void LongExtraction()
        {
            Property property = GenerateIdProperty();
            IGetter getter = PropertyAccessorFactory.GetPropertyAccessor(property, EntityMode.Dynamic).GetGetter(null, "id");
            var id = getter.Get(dyn);
            Assert.That(id, Is.EqualTo(123L));
        }

        [Test]
        public void StringExtraction()
        {
            Property property = GenerateNameProperty();
            IGetter getter = PropertyAccessorFactory.GetPropertyAccessor(property, EntityMode.Dynamic).GetGetter(null, "name");
            var name = (string)getter.Get(dyn);
            Assert.That(name, Is.EqualTo("NHForge"));
        }

        [Test]
        public void StringSet()
        {
            Property property = GenerateNameProperty();
            ISetter setter = PropertyAccessorFactory.GetPropertyAccessor(property, EntityMode.Dynamic).GetSetter(null, "name");
            setter.Set(dyn, "New Value");
            dynamic t = dyn;
            Assert.That(t.name, Is.EqualTo("New Value"));
        }

     
    }
}