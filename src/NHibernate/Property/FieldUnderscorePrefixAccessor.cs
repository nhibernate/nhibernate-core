using System;

namespace NHibernate.Property
{
	/// <summary>
	/// Access fields directly.
	/// </summary>
	/// <remarks>
	/// This accesses fields with the following naming convention:
	/// Property Name = "Id"
	/// Field Name = "_id"
	/// </remarks>
	public class FieldUnderscorePrefixAccessor : FieldAccessor
	{
		/// <summary>
		/// Converts the Property's name into a Field name with the 
		/// "_" prefixing the Property's name converted to camel style 
		/// casing.
		/// </summary>
		/// <param name="propertyName">The name of the Property.</param>
		/// <returns>The name of the Field.</returns>
		/// <remarks>
		/// This uses the convention that a Property <c>Id</c> will have a field </c>_id</c>
		/// </remarks>
		protected override string GetFieldName(string propertyName)
		{
			return "_" + base.GetFieldName(propertyName) ;
		}

	}
}
