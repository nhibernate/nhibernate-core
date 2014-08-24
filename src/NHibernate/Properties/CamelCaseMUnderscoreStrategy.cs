namespace NHibernate.Properties
{
	/// <summary>
	/// Implementation of <see cref="IFieldNamingStrategy"/> for fields that are prefixed with
	/// an <c>m_</c> and the PropertyName is changed to camelCase.
	/// </summary>
	public class CamelCaseMUnderscoreStrategy : IFieldNamingStrategy
	{
		#region IFieldNamingStrategy Members

		/// <summary>
		/// Converts the Property's name into a Field name by making the first character 
		/// of the <c>propertyName</c> lowercase and prefixing it with the letter 'm'
		/// and an underscore.
		/// </summary>
		/// <param name="propertyName">The name of the mapped property.</param>
		/// <returns>The name of the Field in CamelCase format prefixed with an 'm' and an underscore.</returns>
		public string GetFieldName(string propertyName)
		{
			return "m_" + propertyName.Substring(0, 1).ToLowerInvariant() + propertyName.Substring(1);
		}

		#endregion
	}
}
