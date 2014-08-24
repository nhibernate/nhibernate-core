using System.Data;
using System.Xml;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.TypesTest
{
	public class XmlDocTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "XmlDoc"; }
		}

        protected override bool AppliesTo(Dialect.Dialect dialect)
        {
            return TestDialect.SupportsSqlType(new SqlType(DbType.Xml));
        }

		[Test]
		public void ReadWrite()
		{
			using (var s = OpenSession())
			{
				var docEntity = new XmlDocClass {Id = 1 };
				docEntity.Document = new XmlDocument();
				docEntity.Document.LoadXml("<MyNode>my Text</MyNode>");
				s.Save(docEntity);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var docEntity = s.Get<XmlDocClass>(1);
				var document = docEntity.Document;
				document.Should().Not.Be.Null();
				document.OuterXml.Should().Contain("<MyNode>my Text</MyNode>");
				var xmlElement = document.CreateElement("Pizza");
				xmlElement.SetAttribute("temp", "calda");
				document.FirstChild.AppendChild(xmlElement);
				s.Save(docEntity);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var docEntity = s.Get<XmlDocClass>(1);
				docEntity.Document.OuterXml.Should().Contain("Pizza temp=\"calda\"");
				s.Delete(docEntity);
				s.Flush();
			}
		}

		[Test]
		public void InsertNullValue()
		{
			using (ISession s = OpenSession())
			{
				var docEntity = new XmlDocClass { Id = 1 };
				docEntity.Document = null;
				s.Save(docEntity);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				var docEntity = s.Get<XmlDocClass>(1);
				docEntity.Document.Should().Be.Null();
				s.Delete(docEntity);
				s.Flush();
			}
		}

		[Test]
		public void AutoDiscoverFromNetType()
		{
			// integration test to be 100% sure
			var propertyType = sessions.GetEntityPersister(typeof (XmlDocClass).FullName).GetPropertyType("AutoDocument");
			propertyType.Should().Be.InstanceOf<XmlDocType>();
		}
	}
}