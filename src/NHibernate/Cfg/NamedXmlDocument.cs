using System.Xml;

namespace NHibernate.Cfg
{
	public class NamedXmlDocument
	{
		private readonly string name;
		private readonly XmlDocument document;

		public NamedXmlDocument(string name, XmlDocument document)
		{
			this.name = name;
			this.document = document;
		}

		public string Name
		{
			get { return name; }
		}

		public XmlDocument Document
		{
			get { return document; }
		}
	}
}
