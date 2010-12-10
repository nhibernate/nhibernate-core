using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg
{
	public class NamedXmlDocument
	{
		private readonly string name;
		private readonly HbmMapping document;

		public NamedXmlDocument(string name, XmlDocument document, XmlSerializer mappingDocumentSerializer)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			this.name = name;
			if (document.DocumentElement == null)
			{
				throw new MappingException("Empty XML document:" + name);
			}
			using (var reader = new StringReader(document.DocumentElement.OuterXml))
			{
				this.document = (HbmMapping)mappingDocumentSerializer.Deserialize(reader);
			}
		}

		public string Name
		{
			get { return name; }
		}

		public HbmMapping Document
		{
			get { return document; }
		}
	}
}
