namespace NHibernate.Properties
{
	public class PascalCaseMStrategy : IFieldNamingStrategy
	{
		#region IFieldNamingStrategy Members

		/// <summary>
		/// Converts the Property's name into a Field name by making the first character 
		/// of the <c>propertyName</c> uppercase and prefixing it with the letter 'm'.
		/// </summary>
		/// <param name="propertyName">The name of the mapped property.</param>
		/// <returns>The name of the Field in PascalCase format prefixed with an 'm'.</returns>
		public string GetFieldName(string propertyName)
		{
			return "m" + propertyName.Substring(0, 1).ToUpperInvariant() + propertyName.Substring(1);
		}

		#endregion
	}
}