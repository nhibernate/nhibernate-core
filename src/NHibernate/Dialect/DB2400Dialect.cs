using System;
using System.Data.Common;
using NHibernate.Dialect.Schema;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for DB2 on IBM i (formerly iSeries, formerly OS/400).
	/// </summary>
	/// <remarks>
	/// <para>
	/// The DB2400Dialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.DB2400Driver" /></description>
	///		</item>
	/// </list>
	/// </para>
	/// <para>
	/// Two DB2 engines exist. One for Linux, UNIX and Windows, see <see cref="DB2Dialect" />,
	/// and another one for the IBM i system.
	/// </para>
	/// </remarks>
	public class DB2400Dialect : DB2Dialect
	{
		public DB2400Dialect()
		{
			DefaultProperties[Cfg.Environment.ConnectionDriver] = "NHibernate.Driver.DB2400Driver";
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			// The DB2 implementation is not valid for DB2400.
			throw new NotSupportedException();
		}

		public override bool SupportsSequences
		{
			get { return false; }
		}

		public override string IdentitySelectString
		{
			get { return "select identity_val_local() from sysibm.sysdummy1"; }
		}

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override bool SupportsLimitOffset
		{
			get { return false; }
		}

        public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
            return new SqlString(queryString, " fetch first ", limit, " rows only ");
		}

		public override bool UseMaxForLimit
		{
			get { return true; }
		}

		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		/// <remarks>
		/// <see langword="true" /> by default for DB2400, <see cref="DB2Dialect.ToStringLiteral" /> implementation and
		/// <see href="https://www.ibm.com/docs/en/i/7.6.0?topic=constants-graphic-string#rbafzgraphicconst__title__3" />.
		/// </remarks>
		/// <inheritdoc />
		protected override bool UseNPrefixForUnicodeStrings => true;
	}
}
