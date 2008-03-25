using System;
using System.IO;
using System.Xml;
using NHibernate.Cfg;
using NHibernate.Cfg.XmlHbmBinding;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Test.CfgTest
{
	[TestFixture]
	public class HbmBinderFixture
	{
		private XmlDocument LoadAndValidate(string xml, string name)
		{
			using (StringReader stringReader = new StringReader(xml))
			{
				XmlTextReader xmlReader = new XmlTextReader(stringReader);
				Configuration cfg = new Configuration();
				return cfg.LoadMappingDocument(xmlReader, name).Document;
			}
		}

		private string GetXmlForTesting(string versionTag)
		{
			string XML_TEMPLATE =
				@"<?xml version='1.0' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2'>
	<class name='class'>
		<id column='id'>
			<generator class='generator' />
		</id>
		<{0} name='{0}' />
	</class>
</hibernate-mapping>";

			return String.Format(XML_TEMPLATE, versionTag);
		}

		private void CheckDefaultUnsavedValue(string versionTag)
		{
			string XML = GetXmlForTesting(versionTag);
			XmlDocument document = LoadAndValidate(XML, versionTag);
			XmlNode node = document.GetElementsByTagName(versionTag)[0];
			SimpleValue model = new SimpleValue();
			RootClassBinder.MakeVersion(node, model);
			Assert.IsNull(model.NullValue,
			              "default unsaved-value for tag {0} should be null, but is '{1}'",
			              versionTag, model.NullValue);
		}

		[Test]
		public void DefaultUnsavedValueIsUndefined()
		{
			CheckDefaultUnsavedValue("version");
			CheckDefaultUnsavedValue("timestamp");
		}
	}
}