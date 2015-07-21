using System.Xml;

namespace NHibernate.Cfg
{
	/// <summary> 
	/// Represents a mapping queued for delayed processing to await
	/// processing of an extends entity upon which it depends. 
	/// </summary>
	public class ExtendsQueueEntry
	{
		private readonly string explicitName;
		private readonly string mappingPackage;
		private readonly XmlDocument document;

		public ExtendsQueueEntry(string explicitName, string mappingPackage, XmlDocument document)
		{
			this.explicitName = explicitName;
			this.mappingPackage = mappingPackage;
			this.document = document;
		}

		public string ExplicitName
		{
			get { return explicitName; }
		}

		public string MappingPackage
		{
			get { return mappingPackage; }
		}

		public XmlDocument Document
		{
			get { return document; }
		}
	}
}
