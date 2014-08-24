namespace NHibernate.Properties
{
	/// <summary>
	/// Implementation of <see cref="IFieldNamingStrategy"/> for fields that are the 
	/// camelCase version of the PropertyName
	/// </summary>
	public class CamelCaseStrategy : IFieldNamingStrategy
	{
		#region IFieldNamingStrategy Members

		/// <summary>
		/// Converts the Property's name into a Field name by making the first character
		/// lower case.
		/// </summary>
		/// <param name="propertyName">The name of the mapped property.</param>
		/// <returns>The name of the Field in CamelCase format.</returns>
		public string GetFieldName(string propertyName)
		{
			return propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
		}

		#endregion
	}
}