namespace NHibernate.Properties
{
	/// <summary>
	/// Implementation of <see cref="IFieldNamingStrategy"/> for fields that are  
	/// the PropertyName in all LowerCase characters.
	/// </summary>
	public class LowerCaseStrategy : IFieldNamingStrategy
	{
		#region IFieldNamingStrategy Members

		/// <summary>
		/// Converts the Property's name into a Field name by making the all characters 
		/// of the <c>propertyName</c> lowercase.
		/// </summary>
		/// <param name="propertyName">The name of the mapped property.</param>
		/// <returns>The name of the Field in lowercase.</returns>
		public string GetFieldName(string propertyName)
		{
			return propertyName.ToLowerInvariant();
		}

		#endregion
	}
}