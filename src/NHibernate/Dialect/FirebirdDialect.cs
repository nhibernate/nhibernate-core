using System;
using System.Data;
using NHibernate.SqlTypes;

namespace NHibernate.Dialect
{
	/// <summary>
	/// Summary description for FirebirdDialect.
	/// </summary>
	public class FirebirdDialect : Dialect
	{
		public FirebirdDialect() : base()
		{
			// ========EXACT NUMERICS==============
			//	Integers
			Register( DbType.Boolean, "SMALLINT" ); //Sybase BIT type does not support null values
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INTEGER" );
			Register( DbType.Int64, "BIGINT" );

			//	bit
			Register( DbType.Byte, "SMALLINT" );

			// decimal & numeric
			// TODO: figure out if this is the good way to fix the problem
			// with exporting a DECIMAL column
			// NUMERIC(precision, scale) has a hardcoded precision of 19, even though it can range from 1 to 38
			// and the scale has to be 0 <= scale <= precision
			//Register( DbType.Decimal, "NUMERIC(19,$1)" );
			Register( DbType.Decimal, "DECIMAL(19,0)" ); // NUMERIC(19,0) is equivalent to DECIMAL(19,0)
			Register( DbType.Decimal, 19, "DECIMAL(19, $1)");

			//	money & smallmoney
			Register (DbType.Currency, "DECIMAL(16,4)");

			// ========APPROXIMATE NUMERICS==============
			//Register( DbType.Single, "FLOAT" );
			Register( DbType.Single, "FLOAT" ); //synonym for REAL
			Register( DbType.Double, "DOUBLE PRECISION" ); //synonym for FLOAT(53)

			// ========DATETIME & SMALLDATETIME==============
			Register( DbType.Date, "DATE");
			Register( DbType.DateTime, "TIMESTAMP" );
			Register( DbType.Time, "TIME" );
			

			// ========CHARACTER STRINGS==============
			//Register( DbType.Character, "CHAR(1)" );
			Register( DbType.AnsiStringFixedLength, "CHAR(255)");
			Register( DbType.AnsiStringFixedLength, 8000, "CHAR($1)" );
			
			Register( DbType.AnsiString, "VARCHAR(255)" );
			Register( DbType.AnsiString, 8000, "VARCHAR($1)" );
			Register( DbType.AnsiString, 2147483647, "BLOB"); // should use the IType.ClobType

			// TODO: figure out how to support this - VARCHAR > 8000
			// I think that each DbType can only be registered once??
			//Register( DbType.AnsiString, "TEXT" );
		
			// ========UNICODE CHARACTER STRINGS==============
			Register( DbType.StringFixedLength, "CHAR(255)");
			Register( DbType.StringFixedLength, 4000, "CHAR($1)");
			Register( DbType.String, "VARCHAR(255)" );
			Register( DbType.String, 4000, "VARCHAR($1)" );
			Register( DbType.String, 1073741823, "BLOB SUB_TYPE 1" );// should use the IType.ClobType
			
			Register( DbType.Binary, "BLOB(8000) SUB_TYPE 0");
			Register( DbType.Binary, 8000, "BLOB($1) SUB_TYPE 0");
			Register( DbType.Binary, 2147483647, "BLOB SUB_TYPE 0" );// should use the IType.BlobType
		}

		public override string AddColumnString
		{
			get
			{
				return "add";
			}
		}

		public override string GetSequenceNextValString(string sequenceName)
		{
			return string.Format("select gen_id({0}, 1 ) from RDB$DATABASE", sequenceName);
		}

		public override string GetCreateSequenceString(string sequenceName)
		{
			return string.Format("create generator {0}", sequenceName);
		}

		public override string GetDropSequenceString(string sequenceName)
		{
			return string.Format("drop generator {0}", sequenceName);
		}

		public override bool SupportsSequences
		{
			get
			{
				return true;
			}
		}
						
			
		private string SqlTypeToString(string name, int length) 
		{
			return name + "(" + length + ")";
		}

		private string SqlTypeToString(string name, int precision, int scale) 
		{
			if (precision > 18) precision = 18;
			return name + "(" + precision + ", " + scale + ")";
		}

		protected override string SqlTypeToString(AnsiStringFixedLengthSqlType sqlType) 
		{
			
			if(sqlType.Length <= 8000) 
			{
				return SqlTypeToString("CHAR", sqlType.Length);
			}
			else 
			{
				return "BLOB SUB_TYPE 1"; // should use the IType.ClobType
			}
					
		}

		protected override  string SqlTypeToString(BinarySqlType sqlType) 
		{
			return "BLOB SUB_TYPE 0"; // should use the IType.BlobType
		}
		
		protected override string SqlTypeToString(BooleanSqlType sqlType)
		{
			return "SMALLINT";
		}

		
		protected override string SqlTypeToString(ByteSqlType sqlType)
		{
			return "SMALLINT";
		}

		protected override string SqlTypeToString(CurrencySqlType sqlType)
		{
			return "DECIMAL(16,4)";
		}

		protected override string SqlTypeToString(DateSqlType sqlType)
		{
			return "DATE";
		}

		protected override string SqlTypeToString(DateTimeSqlType sqlType)
		{
			return "TIMESTAMP";
		}

		protected override string SqlTypeToString(DecimalSqlType sqlType)
		{
			return SqlTypeToString("DECIMAL", sqlType.Precision, sqlType.Scale);
		}

		protected override string SqlTypeToString(DoubleSqlType sqlType)
		{
			return "DOUBLE PRECISION";
		}

		protected override string SqlTypeToString(Int16SqlType sqlType)
		{
			return "SMALLINT";
		}

		protected override string SqlTypeToString(Int32SqlType sqlType)
		{
			return "INTEGER";
		}

		protected override string SqlTypeToString(Int64SqlType sqlType)
		{
			return "BIGINT";
		}

		protected override string SqlTypeToString(SingleSqlType sqlType)
		{
			return "FLOAT";
		}

		protected override string SqlTypeToString(StringFixedLengthSqlType sqlType) 
		{
			
			if(sqlType.Length <= 4000) 
			{
				return SqlTypeToString("CHAR", sqlType.Length);
			}
			else 
			{
				return "BLOB SUB_TYPE 1"; // should use the IType.ClobType
			}
					
		}

		protected override string SqlTypeToString(StringSqlType sqlType) 
		{
			
			if(sqlType.Length <= 4000) 
			{
				return SqlTypeToString("VARCHAR", sqlType.Length);
			}
			else 
			{
				return "BLOB SUB_TYPE 1";
			}
					
		}


	}
}
