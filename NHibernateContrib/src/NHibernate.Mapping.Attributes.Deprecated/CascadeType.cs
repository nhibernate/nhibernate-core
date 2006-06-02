using System;
using System.Xml.Serialization;

namespace NHibernate.Mapping.Attributes
{
	/// <summary>
	/// Summary description for Cascade.
	/// </summary>
	public enum CascadeType
	{
		[XmlEnum("none")]
		None,

		[XmlEnum("all")]
		All,

		[XmlEnum("save-update")]
		SaveUpdate,

		[XmlEnum("delete")]
		Delete
	}
}
