using System.Data;
using NHibernate.Cfg;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Summary description for FirebirdDialect.
	/// </summary>
	/// <remarks>
	/// The DB2Dialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>hibernate.connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.FirebirdDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class FirebirdDialect : Dialect
	{
		/// <summary></summary>
		public FirebirdDialect() : base()
		{
			Register( DbType.AnsiStringFixedLength, "CHAR(255)" );
			Register( DbType.AnsiStringFixedLength, 8000, "CHAR($1)" );
			Register( DbType.AnsiString, "VARCHAR(255)" );
			Register( DbType.AnsiString, 8000, "VARCHAR($1)" );
			Register( DbType.AnsiString, 2147483647, "BLOB" ); // should use the IType.ClobType
			Register( DbType.Binary, 2147483647, "BLOB SUB_TYPE 0" ); // should use the IType.BlobType
			Register( DbType.Boolean, "SMALLINT" );
			Register( DbType.Byte, "SMALLINT" );
			Register( DbType.Currency, "DECIMAL(16,4)" );
			Register( DbType.Date, "DATE" );
			Register( DbType.DateTime, "TIMESTAMP" );
			Register( DbType.Decimal, "DECIMAL(18,0)" ); // NUMERIC(18,0) is equivalent to DECIMAL(18,0)
			Register( DbType.Decimal, 18, "DECIMAL(18, $1)" );
			Register( DbType.Double, "DOUBLE PRECISION" );
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INTEGER" );
			Register( DbType.Int64, "BIGINT" );
			Register( DbType.Single, "FLOAT" );
			Register( DbType.StringFixedLength, "CHAR(255)" );
			Register( DbType.StringFixedLength, 4000, "CHAR($1)" );
			Register( DbType.String, "VARCHAR(255)" );
			Register( DbType.String, 4000, "VARCHAR($1)" );
			Register( DbType.String, 1073741823, "BLOB SUB_TYPE 1" ); // should use the IType.ClobType
			Register( DbType.Time, "TIME" );

			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.FirebirdDriver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceName"></param>
		/// <returns></returns>
		public override string GetSequenceNextValString( string sequenceName )
		{
			return string.Format( "select gen_id({0}, 1 ) from RDB$DATABASE", sequenceName );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceName"></param>
		/// <returns></returns>
		public override string GetCreateSequenceString( string sequenceName )
		{
			return string.Format( "create generator {0}", sequenceName );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceName"></param>
		/// <returns></returns>
		public override string GetDropSequenceString( string sequenceName )
		{
			return string.Format( "drop generator {0}", sequenceName );
		}

		/// <summary></summary>
		public override bool SupportsSequences
		{
			get { return true; }
		}
	}
}