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
			Register( DbType.AnsiStringFixedLength, "CHAR(255)");
			Register( DbType.AnsiStringFixedLength, 8000, "CHAR($1)" );
			Register( DbType.AnsiString, "VARCHAR(255)" );
			Register( DbType.AnsiString, 8000, "VARCHAR($1)" );
			Register( DbType.AnsiString, 2147483647, "BLOB"); // should use the IType.ClobType
			Register( DbType.Binary, "BLOB(8000) SUB_TYPE 0");
			Register( DbType.Binary, 8000, "BLOB($1) SUB_TYPE 0");
			Register( DbType.Binary, 2147483647, "BLOB SUB_TYPE 0" );// should use the IType.BlobType
			Register( DbType.Boolean, "SMALLINT" ); 
			Register( DbType.Byte, "SMALLINT" );
			Register( DbType.Currency, "DECIMAL(16,4)");
			Register( DbType.Date, "DATE");
			Register( DbType.DateTime, "TIMESTAMP" );
			Register( DbType.Decimal, "DECIMAL(19,0)" ); // NUMERIC(19,0) is equivalent to DECIMAL(19,0)
			Register( DbType.Decimal, 19, "DECIMAL(19, $1)");
			Register( DbType.Double, "DOUBLE PRECISION" ); 
			Register( DbType.Int16, "SMALLINT" );
			Register( DbType.Int32, "INTEGER" );
			Register( DbType.Int64, "BIGINT" );
			Register( DbType.Single, "FLOAT" ); 
			Register( DbType.StringFixedLength, "CHAR(255)");
			Register( DbType.StringFixedLength, 4000, "CHAR($1)");
			Register( DbType.String, "VARCHAR(255)" );
			Register( DbType.String, 4000, "VARCHAR($1)" );
			Register( DbType.String, 1073741823, "BLOB SUB_TYPE 1" );// should use the IType.ClobType
			Register( DbType.Time, "TIME" );
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
	}
}
