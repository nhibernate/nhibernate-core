namespace NHibernate.Properties
{
	/// <summary>
	/// A Strategy for converting a mapped property name to a Field name.
	/// </summary>
	public interface IFieldNamingStrategy
	{
		/// <summary>
		/// When implemented by a class, converts the Property's name into a Field name
		/// </summary>
		/// <param name="propertyName">The name of the mapped property.</param>
		/// <returns>The name of the Field.</returns>
		string GetFieldName(string propertyName);
	}
}