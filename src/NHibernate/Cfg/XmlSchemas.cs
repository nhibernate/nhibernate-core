using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace NHibernate.Cfg
{
	internal class XmlSchemas
	{
		private const string CfgSchemaResource = "NHibernate.nhibernate-configuration.xsd";
		private const string MappingSchemaResource = "NHibernate.nhibernate-mapping.xsd";

		private readonly XmlSchema config = ReadXmlSchemaFromEmbeddedResource(CfgSchemaResource);
		private readonly XmlSchema mapping = ReadXmlSchemaFromEmbeddedResource(MappingSchemaResource);

		public XmlReaderSettings CreateConfigReaderSettings()
		{
			return CreateXmlReaderSettings(config);
		}

		public XmlReaderSettings CreateMappingReaderSettings()
		{
			return CreateXmlReaderSettings(mapping);
		}

		private static XmlSchema ReadXmlSchemaFromEmbeddedResource(string resourceName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();

			using (Stream resourceStream = executingAssembly.GetManifestResourceStream(resourceName))
				return XmlSchema.Read(resourceStream, null);
		}

		private static XmlReaderSettings CreateXmlReaderSettings(XmlSchema xmlSchema)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ValidationType = ValidationType.Schema;
			settings.Schemas.Add(xmlSchema);
			return settings;
		}
	}
}