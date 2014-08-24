using System.Data;
using System.Xml.Linq;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;
using SharpTestsEx;

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
				document.Should().Not.Be.Null();
				document.Document.Root.ToString(SaveOptions.DisableFormatting).Should().Contain("<MyNode>my Text</MyNode>");
			  var xmlElement = new XElement("Pizza", new XAttribute("temp", "calda"));
        document.Document.Root.Add(xmlElement);
				s.Save(docEntity);
				s.Flush();
			}
			using (var s = OpenSession())
			{
				var docEntity = s.Get<XDocClass>(1);
        var document = docEntity.Document;
        document.Document.Root.ToString(SaveOptions.DisableFormatting).Should().Contain("Pizza temp=\"calda\"");
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
				docEntity.Document.Should().Be.Null();
				s.Delete(docEntity);
				s.Flush();
			}
		}

		[Test]
		public void AutoDiscoverFromNetType()
		{
			// integration test to be 100% sure
			var propertyType = sessions.GetEntityPersister(typeof (XDocClass).FullName).GetPropertyType("AutoDocument");
			propertyType.Should().Be.InstanceOf<XDocType>();
		}
	}
}
