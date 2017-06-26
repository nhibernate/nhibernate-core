using System.Data;
using System.Xml.Linq;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
  [TestFixture]
	public class XDocTypeFixture : TypeFixtureBase
	{
		protected override string TypeName
		{
			get { return "XDoc"; }
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
				var docEntity = new XDocClass {Id = 1 };
		docEntity.Document = XDocument.Parse("<MyNode>my Text</MyNode>");
				s.Save(docEntity);
				s.Flush();
			}

			using (var s = OpenSession())
			{
				var docEntity = s.Get<XDocClass>(1);
				var document = docEntity.Document;
				Assert.That(document, Is.Not.Null);
				Assert.That(document.Document.Root.ToString(SaveOptions.DisableFormatting), Does.Contain("<MyNode>my Text</MyNode>"));
			  var xmlElement = new XElement("Pizza", new XAttribute("temp", "calda"));
		document.Document.Root.Add(xmlElement);
				s.Save(docEntity);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var docEntity = s.Get<XDocClass>(1);
		var document = docEntity.Document;
		Assert.That(document.Document.Root.ToString(SaveOptions.DisableFormatting), Does.Contain("Pizza temp=\"calda\""));
				s.Delete(docEntity);
				s.Flush();
			}
		}

		[Test]
		public void InsertNullValue()
		{
			using (ISession s = OpenSession())
			{
				var docEntity = new XDocClass { Id = 1 };
				docEntity.Document = null;
				s.Save(docEntity);
				s.Flush();
			}

			using (ISession s = OpenSession())
			{
				var docEntity = s.Get<XDocClass>(1);
				Assert.That(docEntity.Document, Is.Null);
				s.Delete(docEntity);
				s.Flush();
			}
		}

		[Test]
		public void AutoDiscoverFromNetType()
		{
			// integration test to be 100% sure
			var propertyType = Sfi.GetEntityPersister(typeof (XDocClass).FullName).GetPropertyType("AutoDocument");
			Assert.That(propertyType, Is.InstanceOf<XDocType>());
		}
	}
}
