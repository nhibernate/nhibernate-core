using System;
using System.Data;
using System.Text;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Util;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A SQL dialect for MySQL
	/// </summary>
	/// <remarks>
	/// The MySQLDialect defaults the following configuration properties:
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
	///			<description><see cref="NHibernate.Driver.MySqlDataDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class MySQLDialect : Dialect
	{
		/// <summary></summary>
		public MySQLDialect() : base()
		{
			Register( DbType.AnsiStringFixedLength, "CHAR(255)" );
			Register( DbType.AnsiStringFixedLength, 255, "CHAR($1)" );
			Register( DbType.AnsiStringFixedLength, 65535, "TEXT" );
			Register( DbType.AnsiStringFixedLength, 16777215, "MEDIUMTEXT" );
			Register( DbType.AnsiString, "VARCHAR(255)" );
			Register( DbType.AnsiString, 255, "VARCHAR($1)" );
			Register( DbType.AnsiString, 65535, "TEXT" );
			Register( DbType.AnsiString, 16777215, "MEDIUMTEXT" );
			Register( DbType.Binary, "LONGBLOB" );
			Register( DbType.Binary, 127, "TINYBLOB" );
			Register( DbType.Binary, 65535, "BLOB" );
			Register( DbType.Binary, 16777215, "MEDIUMBLOB" );
			Register( DbType.Boolean, "TINYINT(1)" );
			Register( DbType.Byte, "TINYINT UNSIGNED" );
			Register( DbType.Currency, "MONEY" );
			Register( DbType.Date, "DATE" );
			Register( DbType.DateTime, "DATETIME" );
			Register( DbType.Decimal, "NUMERIC(19,5)" );
			Register( DbType.Decimal, 19, "NUMERIC(19, $1)" );
			Register( DbType.Double, "DOUBLE" );
			Register( DbType.Guid, "VARCHAR(40)" );
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INTEGER" );
			Register( DbType.Int64, "BIGINT" );
			Register( DbType.Single, "FLOAT" );
			Register( DbType.StringFixedLength, "CHAR(255)" );
			Register( DbType.StringFixedLength, 255, "CHAR($1)" );
			Register( DbType.StringFixedLength, 65535, "TEXT" );
			Register( DbType.StringFixedLength, 16777215, "MEDIUMTEXT" );
			Register( DbType.String, "VARCHAR(255)" );
			Register( DbType.String, 255, "VARCHAR($1)" );
			Register( DbType.String, 65535, "TEXT" );
			Register( DbType.String, 16777215, "MEDIUMTEXT" );
			Register( DbType.Time, "TIME" );

			DefaultProperties[ Environment.OuterJoin ] = "true";
			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.MySqlDataDriver";
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
		public override bool QualifyIndexName
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string IdentitySelectString
		{
			get { return "SELECT LAST_INSERT_ID()"; }
		}

		/// <summary></summary>
		public override string IdentityColumnString
		{
			get { return "NOT NULL AUTO_INCREMENT"; }
		}

		/// <summary></summary>
		protected override char CloseQuote
		{
			get { return '`'; }
		}

		/// <summary></summary>
		protected override char OpenQuote
		{
			get { return '`'; }
		}

		/// <summary></summary>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool PreferLimit
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
			Parameter p1 = new Parameter( "p1", new Int32SqlType() );
			Parameter p2 = new Parameter( "p2", new Int32SqlType() );

			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add( querySqlString );
			pagingBuilder.Add( " limit " );
			pagingBuilder.Add( p1 );
			pagingBuilder.Add( ", " );
			pagingBuilder.Add( p2 );

			return pagingBuilder.ToSqlString();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="constraintName"></param>
		/// <param name="foreignKey"></param>
		/// <param name="referencedTable"></param>
		/// <param name="primaryKey"></param>
		/// <returns></returns>
		public override string GetAddForeignKeyConstraintString( string constraintName, string[ ] foreignKey, string referencedTable, string[ ] primaryKey )
		{
			string cols = String.Join( StringHelper.CommaSpace, foreignKey );
			return new StringBuilder( 30 )
				.Append( " add index (" )
				.Append( cols )
				.Append( "), add constraint " )
				.Append( constraintName )
				.Append( " foreign key (" )
				.Append( cols )
				.Append( ") references " )
				.Append( referencedTable )
				.Append( " (" )
				.Append( String.Join( StringHelper.CommaSpace, primaryKey ) )
				.Append( ')' )
				.ToString();
		}
	}
}