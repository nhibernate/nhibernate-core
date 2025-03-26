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
		/// <summary>
		/// ASE 16 supports limit statements, see https://help.sap.com/docs/SAP_ASE/e0d4539d39c34f52ae9ef822c2060077/26d84b4ddae94fed89d4e7c88bc8d1e6.html?locale=en-US
		/// </summary>
		/// <returns>true</returns>
		public override bool SupportsLimit => true;

		/// <inheritdoc />
		/// <returns>true</returns>
		public override bool SupportsLimitOffset => true;

		/// <inheritdoc />
		/// <returns>false</returns>
		public override bool SupportsVariableLimit => false;

		/// <inheritdoc />
		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			if (offset == null && limit == null)
				return queryString;

			var pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add(queryString);
			pagingBuilder.Add(" rows ");

			if (limit != null)
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
	}
}
