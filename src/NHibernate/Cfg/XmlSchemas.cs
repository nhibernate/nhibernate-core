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

		private static readonly XmlSchemaSet ConfigSchemaSet = ReadXmlSchemaFromEmbeddedResource(CfgSchemaResource);
		private static readonly XmlSchemaSet MappingSchemaSet = ReadXmlSchemaFromEmbeddedResource(MappingSchemaResource);

		public XmlReaderSettings CreateConfigReaderSettings()
		{
			XmlReaderSettings result = CreateXmlReaderSettings(ConfigSchemaSet);
			result.ValidationEventHandler += ConfigSettingsValidationEventHandler;
			result.IgnoreComments = true;
			return result;
		}

		public XmlReaderSettings CreateMappingReaderSettings()
		{
			return CreateXmlReaderSettings(MappingSchemaSet);
		}

		private static XmlSchemaSet ReadXmlSchemaFromEmbeddedResource(string resourceName)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();

			using (Stream resourceStream = executingAssembly.GetManifestResourceStream(resourceName))
			{
				var xmlSchema = XmlSchema.Read(resourceStream, null);
				var xmlSchemaSet = new XmlSchemaSet();
				xmlSchemaSet.Add(xmlSchema);
				xmlSchemaSet.Compile();
				return xmlSchemaSet;
			}
		}

		private static XmlReaderSettings CreateXmlReaderSettings(XmlSchemaSet xmlSchemaSet)
		{
			return new XmlReaderSettings {ValidationType = ValidationType.Schema, Schemas = xmlSchemaSet};
		}

		private static void ConfigSettingsValidationEventHandler(object sender, ValidationEventArgs e)
		{
			throw new HibernateConfigException("An exception occurred parsing configuration :" + e.Message, e.Exception);
		}
	}
}