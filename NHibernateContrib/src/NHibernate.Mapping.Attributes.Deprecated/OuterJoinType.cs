using System;
using System.Xml.Serialization;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for OuterJoinType.
	/// </summary>
	public enum OuterJoinType
	{
		[XmlEnum("auto")]
		Auto,
		
		[XmlEnum("true")]
		True,
		
		[XmlEnum("false")]
		False
	}
}
