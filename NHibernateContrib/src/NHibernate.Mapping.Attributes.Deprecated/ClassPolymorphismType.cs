using System;
using System.Xml.Serialization;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for ClassPolymorphismType.
	/// </summary>
	public enum ClassPolymorphismType
	{
		[XmlEnum("implicit")]
		Implicit = 0,
		
		[XmlEnum("explicit")]
		Explicit = 1
	}
}
