using System.Data;
using NHibernate.Cfg;
using NHibernate.Dialect.Function;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect compatible with Sybase.
	/// </summary>
	/// <remarks>
	/// <p>
	/// This dialect probably will not work with schema-export.  If anyone out there
	/// can fill in the ctor with DbTypes to Strings that would be helpful.
	/// </p>
	/// The SybaseDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>use_outer_join</term>
	///			<description><see langword="true" /></description>
	///		</item>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description><see cref="NHibernate.Driver.SybaseClientDriver" /></description>
	///		</item>
	///		<item>
	///			<term>prepare_sql</term>
	///			<description><see langword="false" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class SybaseDialect : Dialect
	{
		/// <summary></summary>
		public SybaseDialect()
		{
			RegisterColumnType(DbType.Boolean, "tinyint"); //Sybase BIT type does not support null values
			RegisterColumnType(DbType.Int16, "smallint");
			RegisterColumnType(DbType.Int32, "int");
			RegisterColumnType(DbType.Int64, "numeric(19,0)");
			RegisterColumnType(DbType.UInt16, "smallint");
			RegisterColumnType(DbType.UInt32, "int");
			RegisterColumnType(DbType.UInt64, "numeric(19,0)");

			RegisterColumnType(DbType.Byte, "tinyint");

			RegisterColumnType(DbType.AnsiStringFixedLength, "char(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 800, "char($l)");
			RegisterColumnType(DbType.AnsiString, "varchar(255)");
			RegisterColumnType(DbType.AnsiString, 8000, "varchar($l)");

			RegisterColumnType(DbType.String, "varchar(255)");
			RegisterColumnType(DbType.String, 8000, "varchar($l)");
			RegisterColumnType(DbType.String, "text");

			RegisterColumnType(DbType.Single, "float");
			RegisterColumnType(DbType.Double, "double precision");
			RegisterColumnType(DbType.Date, "datetime");
			RegisterColumnType(DbType.Time, "datetime");
			RegisterColumnType(DbType.DateTime, "datetime");

			RegisterColumnType(DbType.Binary, 8000, "varbinary($l)");
			RegisterColumnType(DbType.Binary, "varbinary(8000)");
			RegisterColumnType(DbType.Binary, int.MaxValue, "image");

			RegisterColumnType(DbType.Decimal, "numeric(19,6)");
			RegisterColumnType(DbType.Decimal, 19, "numeric($p,$s)");

			RegisterFunction("ascii", new StandardSQLFunction("ascii", NHibernateUtil.Int32));
			RegisterFunction("char", new StandardSQLFunction("char", NHibernateUtil.Character));
			RegisterFunction("len", new StandardSQLFunction("len", NHibernateUtil.Int64));
			RegisterFunction("lower", new StandardSQLFunction("lower"));
			RegisterFunction("upper", new StandardSQLFunction("upper"));
			RegisterFunction("str", new StandardSQLFunction("str", NHibernateUtil.String));
			RegisterFunction("ltrim", new StandardSQLFunction("ltrim"));
			RegisterFunction("rtrim", new StandardSQLFunction("rtrim"));
			RegisterFunction("reverse", new StandardSQLFunction("reverse"));
			RegisterFunction("space", new StandardSQLFunction("space", NHibernateUtil.String));

			RegisterFunction("user", new NoArgSQLFunction("user", NHibernateUtil.String));

			RegisterFunction("current_timestamp", new NoArgSQLFunction("getdate", NHibernateUtil.DateTime));
			RegisterFunction("current_time", new NoArgSQLFunction("getdate", NHibernateUtil.Time));
			RegisterFunction("current_date", new NoArgSQLFunction("getdate", NHibernateUtil.Date));

			RegisterFunction("getdate", new NoArgSQLFunction("getdate", NHibernateUtil.Timestamp));
			RegisterFunction("getutcdate", new NoArgSQLFunction("getutcdate", NHibernateUtil.Timestamp));
			RegisterFunction("day", new StandardSQLFunction("day", NHibernateUtil.Int32));
			RegisterFunction("month", new StandardSQLFunction("month", NHibernateUtil.Int32));
			RegisterFunction("year", new StandardSQLFunction("year", NHibernateUtil.Int32));
			RegisterFunction("datename", new StandardSQLFunction("datename", NHibernateUtil.String));

			RegisterFunction("abs", new StandardSQLFunction("abs"));
			RegisterFunction("sign", new StandardSQLFunction("sign", NHibernateUtil.Int32));

			RegisterFunction("acos", new StandardSQLFunction("acos", NHibernateUtil.Double));
			RegisterFunction("asin", new StandardSQLFunction("asin", NHibernateUtil.Double));
			RegisterFunction("atan", new StandardSQLFunction("atan", NHibernateUtil.Double));
			RegisterFunction("cos", new StandardSQLFunction("cos", NHibernateUtil.Double));
			RegisterFunction("cot", new StandardSQLFunction("cot", NHibernateUtil.Double));
			RegisterFunction("exp", new StandardSQLFunction("exp", NHibernateUtil.Double));
			RegisterFunction("log", new StandardSQLFunction("log", NHibernateUtil.Double));
			RegisterFunction("log10", new StandardSQLFunction("log10", NHibernateUtil.Double));
			RegisterFunction("sin", new StandardSQLFunction("sin", NHibernateUtil.Double));
			RegisterFunction("sqrt", new StandardSQLFunction("sqrt", NHibernateUtil.Double));
			RegisterFunction("tan", new StandardSQLFunction("tan", NHibernateUtil.Double));
			RegisterFunction("pi", new NoArgSQLFunction("pi", NHibernateUtil.Double));
			RegisterFunction("square", new StandardSQLFunction("square"));
			RegisterFunction("rand", new StandardSQLFunction("rand", NHibernateUtil.Single));

			RegisterFunction("radians", new StandardSQLFunction("radians", NHibernateUtil.Double));
			RegisterFunction("degrees", new StandardSQLFunction("degrees", NHibernateUtil.Double));

			RegisterFunction("round", new StandardSQLFunction("round"));
			RegisterFunction("ceiling", new StandardSQLFunction("ceiling"));
			RegisterFunction("floor", new StandardSQLFunction("floor"));

			RegisterFunction("isnull", new StandardSQLFunction("isnull"));

			RegisterFunction("concat", new VarArgsSQLFunction(NHibernateUtil.String, "(", "+", ")"));

			RegisterFunction("length", new StandardSQLFunction("len", NHibernateUtil.Int32));
			RegisterFunction("trim", new SQLFunctionTemplate(NHibernateUtil.String, "ltrim(rtrim(?1))"));
			RegisterFunction("locate", new CharIndexFunction());

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SybaseClientDriver";
			DefaultProperties[Environment.PrepareSql] = "false";
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
		public override string ForUpdateString
		{
			get { return string.Empty; }
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string IdentitySelectString
		{
			get { return "select @@identity"; }
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

		/// <remarks>
		/// Sybase does not support quoted aliases, this function thus returns
		/// <c>aliasName</c> as is.
		/// </remarks>
		public override string QuoteForAliasName(string aliasName)
		{
			return aliasName;
		}
	}
}