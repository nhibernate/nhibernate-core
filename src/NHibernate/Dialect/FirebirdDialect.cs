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
			get	{ return true; }
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

		protected override string SqlTypeToString(TimeSqlType sqlType)
		{
			return "TIME";
		}

		protected override string SqlTypeToString(AnsiStringSqlType sqlType)
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
	}
}
