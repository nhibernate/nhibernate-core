using System.Data;
using NHibernate.Cfg;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NHibernate.Dialect
{
	/// <summary>
	///  An SQL dialect for PostgreSQL.
	/// </summary>
	/// <remarks>
	/// The PostgreSQLDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>hibernate.use_outer_join</term>
	///			<description><c>true</c></description>
	///		</item>
	///		<item>
	///			<term>hibernate.connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.NpgsqlDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class PostgreSQLDialect : Dialect
	{
		/// <summary></summary>
		public PostgreSQLDialect()
		{
			Register( DbType.AnsiStringFixedLength, "char(255)" );
			Register( DbType.AnsiStringFixedLength, 8000, "char($1)" );
			Register( DbType.AnsiString, "varchar(255)" );
			Register( DbType.AnsiString, 8000, "varchar($1)" );
			Register( DbType.AnsiString, 2147483647, "text" );
			Register( DbType.Binary, 2147483647, "bytea" );
			Register( DbType.Boolean, "boolean" );
			Register( DbType.Byte, "int2" );
			Register( DbType.Currency, "decimal(16,4)" );
			Register( DbType.Date, "date" );
			Register( DbType.DateTime, "timestamp" );
			Register( DbType.Decimal, "decimal(19,5)" );
			Register( DbType.Decimal, 19, "decimal(18, $1)" );
			Register( DbType.Double, "float8" );
			Register( DbType.Int16, "int2" );
			Register( DbType.Int32, "int4" );
			Register( DbType.Int64, "int8" );
			Register( DbType.Single, "float4" );
			Register( DbType.StringFixedLength, "char(255)" );
			Register( DbType.StringFixedLength, 4000, "char($1)" );
			Register( DbType.String, "varchar(255)" );
			Register( DbType.String, 4000, "varchar($1)" );
			Register( DbType.String, 1073741823, "text" ); //
			Register( DbType.Time, "time" );

			DefaultProperties[ Environment.OuterJoin ] = "true";
			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.NpgsqlDriver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add column"; }
		}

		/// <summary></summary>
		public override bool DropConstraints
		{
			get { return false; }
		}

		/// <summary></summary>
		public override string CascadeConstraintsString
		{
			get { return " cascade"; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceName"></param>
		/// <returns></returns>
		public override string GetSequenceNextValString( string sequenceName )
		{
			return string.Concat( "select nextval ('", sequenceName, "')" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceName"></param>
		/// <returns></returns>
		public override string GetCreateSequenceString( string sequenceName )
		{
			return "create sequence " + sequenceName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sequenceName"></param>
		/// <returns></returns>
		public override string GetDropSequenceString( string sequenceName )
		{
			return "drop sequence " + sequenceName;
		}

		/// <summary></summary>
		public override bool SupportsSequences
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool BindLimitParametersInReverseOrder
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="querySqlString"></param>
		/// <returns></returns>
		public override SqlString GetLimitString( SqlString querySqlString )
		{
			Parameter p1 = new Parameter( "p1", new Int16SqlType() );
			Parameter p2 = new Parameter( "p2", new Int16SqlType() );

			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add( querySqlString );
			pagingBuilder.Add( " limit " );
			pagingBuilder.Add( p1 );
			pagingBuilder.Add( " offset " );
			pagingBuilder.Add( p2 );

			return pagingBuilder.ToSqlString();
		}

		/// <summary></summary>
		public override bool SupportsForUpdateOf
		{
			get { return true; }
		}
	}
}