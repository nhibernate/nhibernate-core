using System.Data;
using System.Text;

using NHibernate.Cfg;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Summary description for FirebirdDialect.
	/// </summary>
	/// <remarks>
	/// The FirebirdDialect defaults the following configuration properties:
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
			RegisterColumnType( DbType.AnsiStringFixedLength, "CHAR(255)" );
			RegisterColumnType( DbType.AnsiStringFixedLength, 8000, "CHAR($1)" );
			RegisterColumnType( DbType.AnsiString, "VARCHAR(255)" );
			RegisterColumnType( DbType.AnsiString, 8000, "VARCHAR($1)" );
			RegisterColumnType( DbType.AnsiString, 2147483647, "BLOB" ); // should use the IType.ClobType
			RegisterColumnType( DbType.Binary, "BLOB SUB_TYPE 0" );
			RegisterColumnType( DbType.Binary, 2147483647, "BLOB SUB_TYPE 0" ); // should use the IType.BlobType
			RegisterColumnType( DbType.Boolean, "SMALLINT" );
			RegisterColumnType( DbType.Byte, "SMALLINT" );
			RegisterColumnType( DbType.Currency, "DECIMAL(16,4)" );
			RegisterColumnType( DbType.Date, "DATE" );
			RegisterColumnType( DbType.DateTime, "TIMESTAMP" );
			RegisterColumnType( DbType.Decimal, "DECIMAL(18,5)" ); // NUMERIC(18,5) is equivalent to DECIMAL(18,5)
			RegisterColumnType( DbType.Decimal, 18, "DECIMAL(18, $1)" );
			RegisterColumnType( DbType.Double, "DOUBLE PRECISION" );
			RegisterColumnType( DbType.Int16, "SMALLINT" );
			RegisterColumnType( DbType.Int32, "INTEGER" );
			RegisterColumnType( DbType.Int64, "BIGINT" );
			RegisterColumnType( DbType.Single, "FLOAT" );
			RegisterColumnType( DbType.StringFixedLength, "CHAR(255)" );
			RegisterColumnType( DbType.StringFixedLength, 4000, "CHAR($1)" );
			RegisterColumnType( DbType.String, "VARCHAR(255)" );
			RegisterColumnType( DbType.String, 4000, "VARCHAR($1)" );
			RegisterColumnType( DbType.String, 1073741823, "BLOB SUB_TYPE 1" ); // should use the IType.ClobType
			RegisterColumnType( DbType.Time, "TIME" );

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