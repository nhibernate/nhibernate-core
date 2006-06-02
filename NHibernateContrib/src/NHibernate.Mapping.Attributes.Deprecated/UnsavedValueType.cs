using System;
using System.Xml.Serialization;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for UnsavedValueType.
	/// </summary>
	public enum UnsavedValueType
	{
		[XmlEnum("null")]
		Null,

		[XmlEnum("any")]
		Any,

		[XmlEnum("none")]
		None,

		Specified
	}
}
