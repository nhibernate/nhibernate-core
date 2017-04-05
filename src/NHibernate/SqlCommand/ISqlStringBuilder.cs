namespace NHibernate.SqlCommand
{
	/// <summary></summary>
	public interface ISqlStringBuilder
	{
		/// <summary>
		/// Builds a SqlString from the internal data.
		/// </summary>
		/// <returns>A valid SqlString that can be converted into an DbCommand</returns>
		SqlString ToSqlString();
	}
}