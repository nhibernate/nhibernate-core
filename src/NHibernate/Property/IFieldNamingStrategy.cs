using System;

namespace NHibernate.Property
{
	/// <summary>
	/// Summary description for IFieldNamingStrategy.
	/// </summary>
	public interface IFieldNamingStrategy
	{
		/// <summary>
		/// Converts the Property's name into a Field name
		/// </summary>
		/// <param name="propertyName">The name of the Property.</param>
		/// <returns>The name of the Field.</returns>
		string GetFieldName(string propertyName);
	}
}
