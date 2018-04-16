using System.Xml;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3518
{
	[TestFixture]
	public class XmlColumnTest : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var session = Sfi.OpenSession())
			using (var t = session.BeginTransaction())
			{
				session.Delete("from ClassWithXmlMember");
				t.Commit();
			}
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2005Dialect;
		}

		protected override bool AppliesTo(ISessionFactoryImplementor factory)
		{
			return factory.ConnectionProvider.Driver is SqlClientDriver;
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Environment.PrepareSql, "true");
		}

		[Test]
		public void FilteredQuery()
		{
			var xmlDocument = new XmlDocument();
			var xmlElement = xmlDocument.CreateElement("testXml");
			xmlDocument.AppendChild(xmlElement);
			var parentA = new ClassWithXmlMember("A", xmlDocument);

			using (var s = Sfi.OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(parentA);
				t.Commit();
			}
		}
	}
}
