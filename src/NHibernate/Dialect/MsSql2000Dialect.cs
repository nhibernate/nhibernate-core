using System;
using System.Data;

using NHibernate.SqlTypes;

namespace NHibernate.Dialect {
	/// <summary>
	/// An SQL dialect compatible with Microsoft SQL Server 2000.
	/// </summary>
	public class MsSql2000Dialect: Dialect {

		public MsSql2000Dialect() : base() {

			// ========EXACT NUMERICS==============
			//	Integers
			Register( DbType.Boolean, "TINYINT" ); //Sybase BIT type does not support null values
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INT" );
			Register( DbType.Int64, "BIGINT" );
			
			//	bit
			Register( DbType.Byte, "TINYINT" );
			
			// decimal & numeric
			// TODO: figure out if this is the good way to fix the problem
			// with exporting a DECIMAL column
			// NUMERIC(precision, scale) has a hardcoded precision of 19, even though it can range from 1 to 38
			// and the scale has to be 0 <= scale <= precision
			//Register( DbType.Decimal, "NUMERIC(19,$1)" );
			Register( DbType.Decimal, "DECIMAL(19,0)" ); // NUMERIC(19,0) is equivalent to DECIMAL(19,0)
			Register( DbType.Decimal, 19, "DECIMAL(19, $1)");

			//	money & smallmoney
			Register (DbType.Currency, "MONEY");


			// ========APPROXIMATE NUMERICS==============
			//Register( DbType.Single, "FLOAT" );
			Register( DbType.Single, "FLOAT(24)" ); //synonym for REAL
			Register( DbType.Double, "DOUBLE PRECISION" ); //synonym for FLOAT(53)
			

			// ========DATETIME & SMALLDATETIME==============
			Register( DbType.Date, "DATETIME");
			Register( DbType.DateTime, "DATETIME" );
			

			// ========CHARACTER STRINGS==============
			//Register( DbType.Character, "CHAR(1)" );
			Register( DbType.AnsiStringFixedLength, "CHAR(255)");
			Register( DbType.AnsiStringFixedLength, 8000, "CHAR($1)" );
			
			Register( DbType.AnsiString, "VARCHAR(255)" );
			Register( DbType.AnsiString, 8000, "VARCHAR($1)" );
			Register( DbType.AnsiString, 2147483647, "TEXT"); // should use the IType.ClobType

			// TODO: figure out how to support this - VARCHAR > 8000
			// I think that each DbType can only be registered once??
			//Register( DbType.AnsiString, "TEXT" );
		
			// ========UNICODE CHARACTER STRINGS==============
			Register( DbType.StringFixedLength, "NCHAR(255)");
			Register( DbType.StringFixedLength, 4000, "NCHAR($1)");
			Register( DbType.String, "NVARCHAR(255)" );
			Register( DbType.String, 4000, "NVARCHAR($1)" );
			Register( DbType.String, 1073741823, "NTEXT" );// should use the IType.ClobType
			
			Register( DbType.Binary, "VARBINARY(8000)");
			Register( DbType.Binary, 8000, "VARBINARY($1)");
			Register( DbType.Binary, 2147483647, "IMAGE" );// should use the IType.BlobType
			
			//Register( DbType.Timestamp, "DATETIME" );
			//Register( DbType.VarBinary, "VARBINARY($1)" );
			
			
			
			/*
				getDefaultProperties().setProperty(Environment.OUTER_JOIN, "true");
				getDefaultProperties().setProperty(Environment.STATEMENT_BATCH_SIZE, NO_BATCH);
				*/
		}

		public override string AddColumnString {
			get { return "add"; }
		}
		public override string NullColumnString {
			get { return " null"; }
		}
		public override bool QualifyIndexName {
			get { return false; }
		}
	
		public override bool SupportsForUpdate {
			get { return false; }
		}
	
		public override bool SupportsIdentityColumns {
			get { return true; }
		}
		public override string IdentitySelectString {
			get { return "select @@identity"; }
		}
		public override string IdentityColumnString {
			get { return "IDENTITY NOT NULL"; } 
		}

		public override string NoColumnsInsertString {
			get { return "DEFAULT VALUES"; }
		}

		public override bool UseNamedParameters {
			get { return true; }
		}

		public override string NamedParametersPrefix {
			get { return "@"; }
		}

		public override string SqlTypeToString(SqlType sqlType) {
			switch(sqlType.DbType) {
				case DbType.AnsiStringFixedLength: 
					return SqlTypeToString((AnsiStringFixedLengthSqlType)sqlType);
					//break;
				case DbType.Binary :
					return SqlTypeToString((BinarySqlType)sqlType);
					//break;
				case DbType.Boolean :
					return "TINYINT";
					//break;
				case DbType.Byte:
					return "TINYINT";
					//break;
				case DbType.Currency:
					return "MONEY";
					//break;
				case DbType.Date:
					return "DATETIME";

				case DbType.DateTime:
					return "DATETIME";
					//break;
				case DbType.Decimal:
					return SqlTypeToString("DECIMAL", sqlType.Precision, sqlType.Scale);
					//break;
				case DbType.Double:
					return SqlTypeToString("FLOAT", sqlType.Length);
					//break;
				case DbType.Int16:
					return "SMALLINT";
					//break;
				case DbType.Int32:
					return "INT";
					//break;
				case DbType.Int64:
					return "BIGINT";
					//break;
				case DbType.Single:
					return SqlTypeToString("FLOAT", sqlType.Length);
					//break;
				case DbType.StringFixedLength:
					return SqlTypeToString((StringFixedLengthSqlType)sqlType);
					//break;
				case DbType.String:
					return SqlTypeToString((StringSqlType)sqlType);
					//break;
				default:
					throw new ApplicationException("Unmapped DBType");
					//break;
			}

		}
						
			
		private string SqlTypeToString(string name, int length) {
			return name + "(" + length + ")";
		}

		private string SqlTypeToString(string name, int precision, int scale) {
			return name + "(" + precision + ", " + scale + ")";
		}

		private string SqlTypeToString(AnsiStringFixedLengthSqlType sqlType) {
			
			if(sqlType.Length <= 8000) {
				return SqlTypeToString("CHAR", sqlType.Length);
			}
			else {
				return "TEXT"; // should use the IType.ClobType
			}
					
		}

		private string SqlTypeToString(BinarySqlType sqlType) {
			
			if(sqlType.Length <= 8000) {
				return SqlTypeToString("VARBINARY", sqlType.Length);
			}
			else {
				return "IMAGE"; // should use the IType.BlobType
			}
					
		}

		private string SqlTypeToString(StringFixedLengthSqlType sqlType) {
			
			if(sqlType.Length <= 4000) {
				return SqlTypeToString("NCHAR", sqlType.Length);
			}
			else {
				return "NTEXT"; // should use the IType.ClobType
			}
					
		}

		private string SqlTypeToString(StringSqlType sqlType) {
			
			if(sqlType.Length <= 4000) {
				return SqlTypeToString("NVARCHAR", sqlType.Length);
			}
			else {
				return "NTEXT";
			}
					
		}

	}
}
