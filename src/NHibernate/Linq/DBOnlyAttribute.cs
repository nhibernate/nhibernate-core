using System;

namespace NHibernate.Linq
{
	/// <summary>
	/// Indicates to the Linq-to-NHibernate provider a method that must not be evaluated. If supported,
	/// it will always be converted to the corresponding SQL statement.
	/// </summary>
	public class DBOnlyAttribute: Attribute
	{
		public DBOnlyAttribute()
		{ }
	}
}