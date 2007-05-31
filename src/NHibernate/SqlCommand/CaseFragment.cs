namespace NHibernate.SqlCommand
{
	/// <summary>
	/// Represents an SQL <c>case when ... then ... end as ...</c>
	/// </summary>
	public abstract class CaseFragment
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="returnColumnName"></param>
		/// <returns></returns>
		public abstract CaseFragment SetReturnColumnName(string returnColumnName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="returnColumnName"></param>
		/// <param name="suffix"></param>
		/// <returns></returns>
		public abstract CaseFragment SetReturnColumnName(string returnColumnName, string suffix);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="columnName"></param>
		/// <param name="columnValue"></param>
		/// <returns></returns>
		public abstract CaseFragment AddWhenColumnNotNull(string alias, string columnName, string columnValue);

		/// <summary></summary>
		public abstract string ToSqlStringFragment();
	}
}