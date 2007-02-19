using System;
using System.Data;

using NHibernate.SqlCommand;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A dialect for SQL Server Everywhere (SQL Server CE).
	/// </summary>
	public class MsSqlCeDialect : Dialect
	{
		public MsSqlCeDialect()
		{
			RegisterColumnType( DbType.AnsiStringFixedLength, "NCHAR(255)" );
			RegisterColumnType( DbType.AnsiStringFixedLength, 4000, "NCHAR" );
			RegisterColumnType( DbType.AnsiString, "NVARCHAR(255)" );
			RegisterColumnType( DbType.AnsiString, 4000, "NVARCHAR" );
			RegisterColumnType( DbType.AnsiString, 1073741823, "NTEXT" );
			RegisterColumnType( DbType.Binary, "VARBINARY(4000)" );
			RegisterColumnType( DbType.Binary, 4000, "VARBINARY($1)" );
			RegisterColumnType( DbType.Binary, 1073741823, "IMAGE" );
			RegisterColumnType( DbType.Boolean, "BIT" );
			RegisterColumnType( DbType.Byte, "TINYINT" );
			RegisterColumnType( DbType.Currency, "MONEY" );
			RegisterColumnType( DbType.DateTime, "DATETIME" );
			RegisterColumnType( DbType.Decimal, "NUMERIC(19,5)" );
			RegisterColumnType( DbType.Decimal, 19, "NUMERIC(19, $1)" );
			RegisterColumnType( DbType.Double, "FLOAT" );
			RegisterColumnType( DbType.Guid, "UNIQUEIDENTIFIER" );
			RegisterColumnType( DbType.Int16, "SMALLINT" );
			RegisterColumnType( DbType.Int32, "INT" );
			RegisterColumnType( DbType.Int64, "BIGINT" );
			RegisterColumnType( DbType.Single, "REAL" ); //synonym for FLOAT(24) 
			RegisterColumnType( DbType.StringFixedLength, "NCHAR(255)" );
			RegisterColumnType( DbType.StringFixedLength, 4000, "NCHAR($1)" );
			RegisterColumnType( DbType.String, "NVARCHAR(255)" );
			RegisterColumnType( DbType.String, 4000, "NVARCHAR($1)" );
			RegisterColumnType( DbType.String, 1073741823, "NTEXT" );
			RegisterColumnType( DbType.Time, "DATETIME" );

			DefaultProperties[ Environment.ConnectionDriver ] = "NHibernate.Driver.SqlServerCeDriver";
			DefaultProperties[ Environment.PrepareSql ] = "false";
		}

		public override string AddColumnString
		{
			get { return "add"; }
		}

		public override string NullColumnString
		{
			get { return " null"; }
		}

		public override bool QualifyIndexName
		{
			get { return false; }
		}
		
		public override string ForUpdateString
		{
			get { return string.Empty; }
		}

		public override SqlString AddIdentitySelectToInsert( SqlString insertSql, string identityColumn, string tableName )
		{
			return null;
		}

		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		public override string GetIdentitySelectString( string identityColumn, string tableName )
		{
			return "select @@IDENTITY";
		}

		public override string IdentityColumnString
		{
			get { return "IDENTITY NOT NULL"; }
		}

		public override bool SupportsLimit
		{
			get { return false; }
		}

		public override bool SupportsLimitOffset
		{
			get { return false; }
		}

		public override bool SupportsVariableLimit
		{
			get { return false; }
		}
	}
}
