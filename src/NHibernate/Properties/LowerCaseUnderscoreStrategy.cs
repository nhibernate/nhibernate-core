namespace NHibernate.Properties
{
	/// <summary>
	/// Implementation of <see cref="IFieldNamingStrategy"/> for fields that are prefixed with
	/// an underscore and the PropertyName is changed to lower case.
	/// </summary>
	public class LowerCaseUnderscoreStrategy : IFieldNamingStrategy
	{
		#region IFieldNamingStrategy Members

		/// <summary>
		/// Converts the Property's name into a Field name by making the all characters 
		/// of the <c>propertyName</c> lowercase and prefixing it with an underscore.
		/// </summary>
		/// <param name="propertyName">The name of the mapped property.</param>
		/// <returns>The name of the Field in lowercase prefixed with an underscore.</returns>
		public string GetFieldName(string propertyName)
		{
			return "_" + propertyName.ToLowerInvariant();
		}

		#endregion
	}
}