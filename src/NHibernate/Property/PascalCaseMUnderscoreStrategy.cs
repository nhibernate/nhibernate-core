namespace NHibernate.Property
{
	/// <summary>
	/// Implementation of FieldNamingStrategy for fields that are prefixed with
	/// an "m_" and the first character in PropertyName capitalized.
	/// </summary>
	public class PascalCaseMUnderscoreStrategy : IFieldNamingStrategy
	{
		#region IFieldNamingStrategy Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string GetFieldName( string propertyName )
		{
			return "m_" + propertyName.Substring( 0, 1 ).ToUpper() + propertyName.Substring( 1 );
		}

		#endregion
	}
}