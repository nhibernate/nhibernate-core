using System.Xml;

namespace NHibernate.Test.TypesTest
{
	public class XmlDocClass
	{
		public int Id { get; set; }
		public XmlDocument Document { get; set; }
		public XmlDocument AutoDocument { get; set; }
	}
}