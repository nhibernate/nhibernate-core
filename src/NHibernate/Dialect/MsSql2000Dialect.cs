using System.Data;
using NHibernate.Cfg;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect compatible with Microsoft SQL Server 2000.
	/// </summary>
	/// <remarks>
	/// The MsSql2000Dialect defaults the following configuration properties:
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
	///			<description><see cref="NHibernate.Driver.SqlClientDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class MsSql2000Dialect : Dialect
	{
		/// <summary></summary>
		public MsSql2000Dialect() : base()
		{
			Register( DbType.AnsiStringFixedLength, "CHAR(255)" );
			Register( DbType.AnsiStringFixedLength, 8000, "CHAR($1)" );
			Register( DbType.AnsiString, "VARCHAR(255)" );
			Register( DbType.AnsiString, 8000, "VARCHAR($1)" );
			Register( DbType.AnsiString, 2147483647, "TEXT" );
			Register( DbType.Binary, "VARBINARY(8000)" );
			Register( DbType.Binary, 8000, "VARBINARY($1)" );
			Register( DbType.Binary, 2147483647, "IMAGE" );
			Register( DbType.Boolean, "BIT" );
			Register( DbType.Byte, "TINYINT" );
			Register( DbType.Currency, "MONEY" );
			Register( DbType.Date, "DATETIME" );
			Register( DbType.DateTime, "DATETIME" );
			// TODO: figure out if this is the good way to fix the problem
			// with exporting a DECIMAL column
			// NUMERIC(precision, scale) has a hardcoded precision of 19, even though it can range from 1 to 38
			// and the scale has to be 0 <= scale <= precision.
			// I think how I might handle it is keep the type="Decimal(29,5)" and make them specify a 
			// sql-type="decimal(20,5)" if they need to do that.  The Decimal parameter and ddl will get generated
			// correctly with minimal work.
			Register( DbType.Decimal, "DECIMAL(19,5)" );
			Register( DbType.Decimal, 19, "DECIMAL(19, $1)" );
			Register( DbType.Double, "DOUBLE PRECISION" ); //synonym for FLOAT(53)
			Register( DbType.Guid, "UNIQUEIDENTIFIER" );
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INT" );
			Register( DbType.Int64, "BIGINT" );
			Register( DbType.Single, "REAL" ); //synonym for FLOAT(24) 
			Register( DbType.StringFixedLength, "NCHAR(255)" );
			Register( DbType.StringFixedLength, 4000, "NCHAR($1)" );
			Register( DbType.String, "NVARCHAR(255)" );
			Register( DbType.String, 4000, "NVARCHAR($1)" );
			Register( DbType.String, 1073741823, "NTEXT" );
			Register( DbType.Time, "DATETIME" );

			DefaultProperties[ Environment.OuterJoin ] = "true";
			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.SqlClientDriver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add"; }
		}

		/// <summary></summary>
		public override string NullColumnString
		{
			get { return " null"; }
		}

		/// <summary></summary>
		public override bool QualifyIndexName
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool SupportsForUpdate
		{
			get { return false; }
		}

		/// <summary>
		/// MsSql allows the use of SELECT SCOPE_IDENTITY to be in the same
		/// Command as the INSERT
		/// </summary>
		/// <value>true</value>
		public override bool SupportsIdentitySelectInInsert
		{
			get { return true; }
		}

		/// <summary>
		/// Add the Identity Select string to the Insert Sql.
		/// </summary>
		/// <param name="insertSql">The SqlString that contains the INSERT sql.</param>
		/// <returns>A new SqlString with <c>; SELECT SCOPE_IDENTITY()</c> at the end.</returns>
		public override SqlString AddIdentitySelectToInsert( SqlString insertSql )
		{
			return insertSql.Append( "; " + IdentitySelectString );
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string IdentitySelectString
		{
			get { return "select SCOPE_IDENTITY()"; }
		}

		/// <summary></summary>
		public override string IdentityColumnString
		{
			get { return "IDENTITY NOT NULL"; }
		}

		/// <summary></summary>
		public override string NoColumnsInsertString
		{
			get { return "DEFAULT VALUES"; }
		}

		/// <summary></summary>
		public override int MaxAnsiStringSize
		{
			get { return 8000; }
		}

		/// <summary></summary>
		public override int MaxBinaryBlobSize
		{
			get { return 2147483647; }
		}

		/// <summary></summary>
		public override int MaxBinarySize
		{
			get { return 8000; }
		}

		/// <summary></summary>
		public override int MaxStringClobSize
		{
			get { return 1073741823; }
		}

		/// <summary></summary>
		public override int MaxStringSize
		{
			get { return 4000; }
		}

		/// <summary></summary>
		protected override char CloseQuote
		{
			get { return ']'; }
		}

		/// <summary></summary>
		protected override char OpenQuote
		{
			get { return '['; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>
		/// MsSql does not require the OpenQuote to be escaped as long as the first char
		/// is an OpenQuote.
		/// </remarks>
		protected override string Quote( string name )
		{
			return OpenQuote + name.Replace( CloseQuote.ToString(), new string( CloseQuote, 2 ) ) + CloseQuote;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="quoted"></param>
		/// <returns></returns>
		public override string UnQuote( string quoted )
		{
			if( IsQuoted( quoted ) )
			{
				quoted = quoted.Substring( 1, quoted.Length - 2 );
			}

			return quoted.Replace( new string( CloseQuote, 2 ), CloseQuote.ToString() );
		}
	}
}