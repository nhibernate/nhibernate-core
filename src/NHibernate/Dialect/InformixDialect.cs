using System.Data;
using NHibernate.Cfg;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Summary description for InformixDialect.
	/// </summary>
	/// <remarks>
	/// The InformixDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.OdbcDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class InformixDialect : Dialect
	{
		/// <summary></summary>
		public InformixDialect() : base()
		{
//		registerFunction( "concat", new VarArgsSQLFunction( Hibernate.STRING, "(", "||", ")" ) );

			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 32739, "LVARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "CLOB");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.Binary, 2147483647, "BLOB");
			RegisterColumnType(DbType.Binary, "BLOB");
			RegisterColumnType(DbType.Byte, "SMALLINT");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "datetime year to fraction(5)");
			RegisterColumnType(DbType.Decimal, "DECIMAL(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "DECIMAL(19, $l)");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "SmallFloat");
			RegisterColumnType(DbType.Time, "datetime hour to second");
			RegisterColumnType(DbType.StringFixedLength, "CHAR($l)");
			RegisterColumnType(DbType.String, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.String, 32739, "LVARCHAR($l)");
			RegisterColumnType(DbType.String, 2147483647, "CLOB");
			RegisterColumnType(DbType.String, "VARCHAR(255)");


			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.OdbcDriver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary>
		/// The syntax that returns the identity value of the last insert, if native
		/// key generation is supported
		/// </summary>
		public override string IdentitySelectString
		{
//			return type==Types.BIGINT ?
//				"select dbinfo('serial8') from systables where tabid=1" :
//				"select dbinfo('sqlca.sqlerrd1') from systables where tabid=1";
			get { return "select dbinfo('sqlca.sqlerrd1') from systables where tabid=1"; }
		}


		/// <summary>
		/// The keyword used to specify an identity column, if native key generation is supported
		/// </summary>
		public override string IdentityColumnString
		{
//		return type==Types.BIGINT ?
//			"serial8 not null" :
//			"serial not null";
			get { return "serial not null"; }
		}

		/// <summary>
		/// Whether this dialect have an Identity clause added to the data type or a
		/// completely seperate identity data type
		/// </summary>
		public override bool HasDataTypeInIdentityColumn
		{
			get { return false; }
		}
	}
}