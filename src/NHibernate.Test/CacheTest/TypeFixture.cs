using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using NHibernate.Engine;
using NHibernate.Type;
using NSubstitute;
using NUnit.Framework;

namespace NHibernate.Test.CacheTest
{
	[TestFixture]
	public class TypeFixture
	{
		[Test]
		public void CultureInfo()
		{
			DoTestType(NHibernateUtil.CultureInfo, new CultureInfo("en-US"));
		}

		[Test]
		public void Type()
		{
			DoTestType(TypeFactory.GetTypeType(100), typeof(TypeFixture));
		}

		[Test]
		public void Uri()
		{
			DoTestType(NHibernateUtil.Uri, new Uri("http://nhibernate.info/", UriKind.RelativeOrAbsolute));
		}

		[Test]
		public void XDoc()
		{
			DoTestType(NHibernateUtil.XDoc, XDocument.Parse(@"<?xml version=""1.0"" encoding=""utf-8""?>
<hibernate-configuration>
</hibernate-configuration>"));
		}

		[Test]
		public void XmlDoc()
		{
			var doc = new XmlDocument();
			doc.LoadXml(
				@"<?xml version=""1.0"" encoding=""utf-8""?>
<hibernate-configuration>
</hibernate-configuration>");
			DoTestType(NHibernateUtil.XmlDoc, doc);
		}

		private void DoTestType(IType type, object value)
		{
			var session = Substitute.For<ISessionImplementor>();
			var cached = type.Disassemble(value, session, null);
			// All NHibernate types should yield a cacheable representation that are at least binary serializable
			NHAssert.IsSerializable(cached);
			var reassembled = type.Assemble(cached, session, null);
			Assert.That(type.IsEqual(reassembled, value), Is.True);
		}
	}
}
