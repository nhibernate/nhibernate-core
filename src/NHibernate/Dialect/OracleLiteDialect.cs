using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	/// <summary>
	/// It's a immature version, it just work.
	/// An SQL dialect for Oracle Lite
	/// </summary>
	/// <remarks>
	/// The OracleLiteDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.OracleLiteDataClientDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class OracleLiteDialect : Oracle9iDialect
	{
		/// <summary></summary>
		public OracleLiteDialect()
		{
			DefaultProperties[Environment.PrepareSql] = "false";
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.OracleLiteDataClientDriver";

			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 2000, "CHAR($l)");
			RegisterColumnType(DbType.AnsiString, "VARCHAR2(255)");
			RegisterColumnType(DbType.AnsiString, 2000, "VARCHAR2($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "CLOB"); // should use the IType.ClobType
			RegisterColumnType(DbType.Binary, "RAW(2000)");
			RegisterColumnType(DbType.Binary, 2000, "RAW($l)");
			RegisterColumnType(DbType.Binary, 2147483647, "BLOB");
			RegisterColumnType(DbType.Boolean, "NUMBER(1,0)");
			RegisterColumnType(DbType.Byte, "NUMBER(3,0)");
			RegisterColumnType(DbType.Currency, "NUMBER(19,1)");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "TIMESTAMP(4)");
			RegisterColumnType(DbType.Decimal, "NUMBER(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "NUMBER(19, $l)");
			// having problems with both ODP and OracleClient from MS not being able
			// to read values out of a field that is DOUBLE PRECISION
			RegisterColumnType(DbType.Double, "DOUBLE PRECISION"); //"FLOAT(53)" );
			//RegisterColumnType(DbType.Guid, "CHAR(38)");
			RegisterColumnType(DbType.Int16, "NUMBER(5,0)");
			RegisterColumnType(DbType.Int32, "NUMBER(10,0)");
			RegisterColumnType(DbType.Int64, "NUMBER(20,0)");
			RegisterColumnType(DbType.UInt16, "NUMBER(5,0)");
			RegisterColumnType(DbType.UInt32, "NUMBER(10,0)");
			RegisterColumnType(DbType.UInt64, "NUMBER(20,0)");
			RegisterColumnType(DbType.Single, "FLOAT(24)");
			RegisterColumnType(DbType.StringFixedLength, "NCHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 2000, "NCHAR($l)");
			RegisterColumnType(DbType.String, "VARCHAR2(255)");
			RegisterColumnType(DbType.String, 2000, "VARCHAR2($l)");
			RegisterColumnType(DbType.String, 1073741823, "CLOB");
			RegisterColumnType(DbType.Time, "DATE");

			RegisterFunction("stddev", new StandardSQLFunction("stddev", NHibernateUtil.Double));
			RegisterFunction("variance", new StandardSQLFunction("variance", NHibernateUtil.Double));

			RegisterFunction("round", new StandardSQLFunction("round"));
			RegisterFunction("trunc", new StandardSQLFunction("trunc"));
			RegisterFunction("ceil", new StandardSQLFunction("ceil"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));

			RegisterFunction("chr", new StandardSQLFunction("chr", NHibernateUtil.Character));
			RegisterFunction("initcap", new StandardSQLFunction("initcap"));
			RegisterFunction("lower", new StandardSQLFunction("lower"));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("rtrim", new StandardSQLFunction("rtrim"));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
			RegisterFunction("length", new StandardSQLFunction("length", NHibernateUtil.Int64));

			RegisterFunction("to_char", new StandardSQLFunction("to_char", NHibernateUtil.String));
			RegisterFunction("to_date", new StandardSQLFunction("to_date", NHibernateUtil.Timestamp));

			RegisterFunction("last_day", new StandardSQLFunction("last_day", NHibernateUtil.Date));
			RegisterFunction("sysdate", new NoArgSQLFunction("sysdate", NHibernateUtil.Date, false));
			RegisterFunction("user", new NoArgSQLFunction("user", NHibernateUtil.String, false));

			// Multi-param string dialect functions...
			RegisterFunction("concat", new StandardSQLFunction("concat", NHibernateUtil.String));
			RegisterFunction("instr", new StandardSQLFunction("instr", NHibernateUtil.String));
			RegisterFunction("instrb", new StandardSQLFunction("instrb", NHibernateUtil.String));
			RegisterFunction("lpad", new StandardSQLFunction("lpad", NHibernateUtil.String));
			RegisterFunction("replace", new StandardSQLFunction("replace", NHibernateUtil.String));
			RegisterFunction("rpad", new StandardSQLFunction("rpad", NHibernateUtil.String));
			RegisterFunction("substr", new StandardSQLFunction("substr", NHibernateUtil.String));
			RegisterFunction("substrb", new StandardSQLFunction("substrb", NHibernateUtil.String));
			RegisterFunction("translate", new StandardSQLFunction("translate", NHibernateUtil.String));

			// Multi-param numeric dialect functions...
			RegisterFunction("mod", new StandardSQLFunction("mod", NHibernateUtil.Int32));
			RegisterFunction("nvl", new StandardSQLFunction("nvl"));

			// Multi-param date dialect functions...
			RegisterFunction("add_months", new StandardSQLFunction("add_months", NHibernateUtil.Date));
			RegisterFunction("months_between", new StandardSQLFunction("months_between", NHibernateUtil.Single));
			RegisterFunction("next_day", new StandardSQLFunction("next_day", NHibernateUtil.Date));
		}

		/// <summary></summary>
		public override string GetCreateSequenceString(string sequenceName)
		{
			return "create sequence " + sequenceName;
		}

		protected override string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
		{
			return GetCreateSequenceString(sequenceName) + " increment by " + incrementSize + " start with " + initialValue;
		}
	}
}