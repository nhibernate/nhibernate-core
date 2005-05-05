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
			RegisterColumnType( DbType.AnsiStringFixedLength, "char(255)" );
			RegisterColumnType( DbType.AnsiStringFixedLength, 8000, "char($1)" );
			RegisterColumnType( DbType.AnsiString, "varchar(255)" );
			RegisterColumnType( DbType.AnsiString, 8000, "varchar($1)" );
			RegisterColumnType( DbType.AnsiString, 2147483647, "text" );
			RegisterColumnType( DbType.Binary, 2147483647, "bytea" );
			RegisterColumnType( DbType.Boolean, "boolean" );
			RegisterColumnType( DbType.Byte, "int2" );
			RegisterColumnType( DbType.Currency, "decimal(16,4)" );
			RegisterColumnType( DbType.Date, "date" );
			RegisterColumnType( DbType.DateTime, "timestamp" );
			RegisterColumnType( DbType.Decimal, "decimal(19,5)" );
			RegisterColumnType( DbType.Decimal, 19, "decimal(18, $1)" );
			RegisterColumnType( DbType.Double, "float8" );
			RegisterColumnType( DbType.Int16, "int2" );
			RegisterColumnType( DbType.Int32, "int4" );
			RegisterColumnType( DbType.Int64, "int8" );
			RegisterColumnType( DbType.Single, "float4" );
			RegisterColumnType( DbType.StringFixedLength, "char(255)" );
			RegisterColumnType( DbType.StringFixedLength, 4000, "char($1)" );
			RegisterColumnType( DbType.String, "varchar(255)" );
			RegisterColumnType( DbType.String, 4000, "varchar($1)" );
			RegisterColumnType( DbType.String, 1073741823, "text" ); //
			RegisterColumnType( DbType.Time, "time" );

			DefaultProperties[ Environment.UseOuterJoin ] = "true";
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

		public override bool PreferLimit
		{
			get { return true; }
		}

	}
}