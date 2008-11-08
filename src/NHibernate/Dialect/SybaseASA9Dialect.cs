using System.Data;
using System.Data.Common;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	public class SybaseASA9Dialect : SybaseAnywhereDialect
	{
		public SybaseASA9Dialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, 255, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "LONG VARCHAR"); // should use the IType.ClobType
			RegisterColumnType(DbType.Binary, "BINARY(255)");
			RegisterColumnType(DbType.Binary, 2147483647, "LONG BINARY"); // should use the IType.BlobType
			RegisterColumnType(DbType.Boolean, "BIT");
			RegisterColumnType(DbType.Byte, "SMALLINT");
			RegisterColumnType(DbType.Currency, "DECIMAL(18,4)");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP");
			RegisterColumnType(DbType.Decimal, "DECIMAL(18,5)"); // NUMERIC(18,5) is equivalent to DECIMAL(18,5)
			RegisterColumnType(DbType.Decimal, 18, "DECIMAL(18,$l)");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Guid, "CHAR(16)");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "FLOAT");
			RegisterColumnType(DbType.StringFixedLength, 255, "CHAR($l)");
			RegisterColumnType(DbType.String, 1073741823, "LONG VARCHAR");
			RegisterColumnType(DbType.String, 255, "VARCHAR($l)");
			RegisterColumnType(DbType.String, "LONG VARCHAR");
			RegisterColumnType(DbType.Time, "TIME");
			RegisterColumnType(DbType.SByte, "SMALLINT");
			RegisterColumnType(DbType.UInt16, "UNSIGNED SMALLINT");
			RegisterColumnType(DbType.UInt32, "UNSIGNED INT");
			RegisterColumnType(DbType.UInt64, "UNSIGNED BIGINT");
			RegisterColumnType(DbType.VarNumeric, "NUMERIC($l)");
			//RegisterColumnType(DbType.Xml, "TEXT");

			// Override standard HQL function
			RegisterFunction("current_timestamp", new StandardSQLFunction("current_timestamp"));
			RegisterFunction("length", new StandardSafeSQLFunction("length", NHibernateUtil.String, 1));
			RegisterFunction("substring", new AnsiSubstringFunction());
			RegisterFunction("nullif", new StandardSafeSQLFunction("nullif", 2));
			RegisterFunction("lower", new StandardSafeSQLFunction("lower", NHibernateUtil.String, 1));
			RegisterFunction("upper", new StandardSafeSQLFunction("upper", NHibernateUtil.String, 1));
			;
			RegisterFunction("now", new StandardSQLFunction("now"));
		}

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		public override SqlString GetLimitString(SqlString querySqlString, int offset, int limit)
		{
			int intSelectInsertPoint = GetAfterSelectInsertPoint(querySqlString);
			string strLimit = string.Format(" TOP {0} START AT {1}", limit, offset + 1);
			return querySqlString.Insert(intSelectInsertPoint, strLimit);
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new SybaseAnywhereDataBaseMetaData(connection);
		}

		private static int GetAfterSelectInsertPoint(SqlString sql)
		{
			string[] arrSelectStrings = {"select distinct", "select all", "select"};
			for (int i = 0; i != arrSelectStrings.Length; ++i)
			{
				string strSelect = arrSelectStrings[i];
				if (sql.StartsWithCaseInsensitive(strSelect))
				{
					return strSelect.Length;
				}
			}
			return 0;
		}
	}
}