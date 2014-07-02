using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NHibernate.Cfg.MappingSchema;

namespace NHibernate.Cfg
{
	public class NamedXmlDocument
	{
		private static readonly XmlSerializer mappingDocumentSerializer = new XmlSerializer(typeof (HbmMapping));
		private readonly string name;
		private readonly HbmMapping document;

		static NamedXmlDocument() { }

		public NamedXmlDocument(string name, XmlDocument document)
			: this(name, document, mappingDocumentSerializer)
		{
		}

		public NamedXmlDocument(string name, XmlDocument document, XmlSerializer serializer)
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
				this.document = (HbmMapping)serializer.Deserialize(reader);
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
