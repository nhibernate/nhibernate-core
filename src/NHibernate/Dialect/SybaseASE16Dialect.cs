using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect targeting Sybase Adaptive Server Enterprise (ASE) 16 and higher.
	/// </summary>
	/// <remarks>
	/// The dialect defaults the following configuration properties:
	/// <list type="table">
	///	<listheader>
	///		<term>Property</term>
	///		<description>Default Value</description>
	///	</listheader>
	///	<item>
	///		<term>connection.driver_class</term>
	///		<description><see cref="NHibernate.Driver.SybaseAseClientDriver" /></description>
	///	</item>
	/// </list>
	/// </remarks>
	public class SybaseASE16Dialect : SybaseASE15Dialect
	{
		#region Limit/offset support

		/// <summary>
		/// Does this Dialect have some kind of <c>LIMIT</c> syntax?
		/// </summary>
		/// <value>False, unless overridden.</value>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary>
		/// Does this Dialect support an offset?
		/// </summary>
		public override bool SupportsLimitOffset
		{
			get { return true; }
		}

		/// <summary>
		/// Can parameters be used for a statement containing a LIMIT?
		/// </summary>
		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		/// <summary>
		/// Attempts to add a <c>LIMIT</c> clause to the given SQL <c>SELECT</c>.
		/// Expects any database-specific offset and limit adjustments to have already been performed (ex. UseMaxForLimit, OffsetStartsAtOne).
		/// </summary>
		/// <param name="queryString">The <see cref="SqlString"/> to base the limit query off.</param>
		/// <param name="offset">Offset of the first row to be returned by the query.  This may be represented as a parameter, a string literal, or a null value if no limit is requested.  This should have already been adjusted to account for OffsetStartsAtOne.</param>
		/// <param name="limit">Maximum number of rows to be returned by the query.  This may be represented as a parameter, a string literal, or a null value if no offset is requested.  This should have already been adjusted to account for UseMaxForLimit.</param>
		/// <returns>A new <see cref="SqlString"/> that contains the <c>LIMIT</c> clause. Returns <c>null</c> 
		/// if <paramref name="queryString"/> represents a SQL statement to which a limit clause cannot be added, 
		/// for example when the query string is custom SQL invoking a stored procedure.</returns>
		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			if (offset == null && limit == null)
				return queryString;

			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add(queryString);
			pagingBuilder.Add(" rows ");

			if(limit !=null)
			{
				pagingBuilder.Add(" limit ");
				pagingBuilder.Add(limit);
			}

			if (offset != null)
			{
				pagingBuilder.Add(" offset ");
				pagingBuilder.Add(offset);
			}

			return pagingBuilder.ToSqlString();
		}

		#endregion
	}
}
