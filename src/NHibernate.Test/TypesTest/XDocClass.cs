using System.Xml.Linq;

namespace NHibernate.Test.TypesTest
{
	public class XDocClass
	{
		public int Id { get; set; }
		public XDocument Document { get; set; }
		public XDocument AutoDocument { get; set; }
	}
}
