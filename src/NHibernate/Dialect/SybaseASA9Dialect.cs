using System.Data;
using System.Data.Common;

using NHibernate.Cfg;
using NHibernate.Dialect.Function;
using NHibernate.Dialect.Schema;
using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for Sybase Adaptive Server Anywhere 9.0
	/// </summary>
	/// <remarks>
	/// <p>
	/// This dialect probably will not work with schema-export.  If anyone out there
	/// can fill in the ctor with DbTypes to Strings that would be helpful.
	/// </p>
	/// The dialect defaults the following configuration properties:
	/// <list type="table">
	///	<listheader>
	///		<term>Property</term>
	///		<description>Default Value</description>
	///	</listheader>
	///	<item>
	///		<term>connection.driver_class</term>
	///		<description><see cref="NHibernate.Driver.SybaseAsaClientDriver" /></description>
	///	</item>
	///	<item>
	///		<term>prepare_sql</term>
	///		<description><see langword="false" /></description>
	///	</item>
	/// </list>
	/// </remarks>
	public class SybaseASA9Dialect : Dialect
	{
		public SybaseASA9Dialect()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SybaseAsaClientDriver";
			DefaultProperties[Environment.PrepareSql] = "false";

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
			RegisterFunction("nullif", new StandardSafeSQLFunction("nullif", 2));
			RegisterFunction("lower", new StandardSafeSQLFunction("lower", NHibernateUtil.String, 1));
			RegisterFunction("upper", new StandardSafeSQLFunction("upper", NHibernateUtil.String, 1));
			RegisterFunction("now", new StandardSQLFunction("now"));

			RegisterKeyword("top");
		}

		public override bool SupportsLimit
		{
			get { return true; }
		}

		public override bool SupportsVariableLimit
		{
			get { return false; }
		}

		public override bool OffsetStartsAtOne
		{
			get { return true; }
		}

		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			int intSelectInsertPoint = GetAfterSelectInsertPoint(queryString);

			SqlStringBuilder limitFragment = new SqlStringBuilder();
			limitFragment.Add(" top ");
			if (limit != null)
				limitFragment.Add(limit);
			else
				limitFragment.Add(int.MaxValue.ToString());

			if (offset != null)
			{
				limitFragment.Add(" start at ");
				limitFragment.Add(offset);
			}

			return queryString.Insert(intSelectInsertPoint, limitFragment.ToSqlString());
		}

		public override IDataBaseSchema GetDataBaseSchema(DbConnection connection)
		{
			return new SybaseAnywhereDataBaseMetaData(connection);
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

		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		public override string IdentitySelectString
		{
			get { return "select @@identity"; }
		}

		public override string IdentityColumnString
		{
			get { return "identity not null"; }
		}

		public override string NoColumnsInsertString
		{
			get { return "default values"; }
		}

		/// <summary>
		/// ASA does not require to drop constraint before dropping tables, and DROP statement
		/// syntax used by Hibernate to drop constraint is not compatible with ASA, so disable it.
		/// Comments matches SybaseAnywhereDialect from Hibernate-3.1 src
		/// </summary>
		public override bool DropConstraints
		{
			get { return false; }
		}

		private static int GetAfterSelectInsertPoint(SqlString sql)
		{
			string[] arrSelectStrings = { "select distinct", "select all", "select" };
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