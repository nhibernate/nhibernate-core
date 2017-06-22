using System.Data;
using System.Xml;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

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
				Assert.That(document, Is.Not.Null);
				Assert.That(document.OuterXml, Does.Contain("<MyNode>my Text</MyNode>"));
				var xmlElement = document.CreateElement("Pizza");
				xmlElement.SetAttribute("temp", "calda");
				document.FirstChild.AppendChild(xmlElement);
				s.Save(docEntity);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var docEntity = s.Get<XmlDocClass>(1);
				Assert.That(docEntity.Document.OuterXml, Does.Contain("Pizza temp=\"calda\""));
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
				Assert.That(docEntity.Document, Is.Null);
				s.Delete(docEntity);
				s.Flush();
			}
		}

		[Test]
		public void AutoDiscoverFromNetType()
		{
			// integration test to be 100% sure
			var propertyType = Sfi.GetEntityPersister(typeof (XmlDocClass).FullName).GetPropertyType("AutoDocument");
			Assert.That(propertyType, Is.InstanceOf<XmlDocType>());
		}
	}
}
