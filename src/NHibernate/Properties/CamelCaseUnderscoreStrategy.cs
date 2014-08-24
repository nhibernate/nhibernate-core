namespace NHibernate.Properties
{
	/// <summary>
	/// Implementation of <see cref="IFieldNamingStrategy"/> for fields that are prefixed with
	/// an underscore and the PropertyName is changed to camelCase.
	/// </summary>
	public class CamelCaseUnderscoreStrategy : IFieldNamingStrategy
	{
		#region IFieldNamingStrategy Members

		/// <summary>
		/// Converts the Property's name into a Field name by making the first character 
		/// of the <c>propertyName</c> lowercase and prefixing it with an underscore.
		/// </summary>
		/// <param name="propertyName">The name of the mapped property.</param>
		/// <returns>The name of the Field in CamelCase format prefixed with an underscore.</returns>
		public string GetFieldName(string propertyName)
		{
			return "_" + propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
		}

		#endregion
	}
}