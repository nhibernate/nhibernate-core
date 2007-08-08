using NHibernate.Cfg;

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
		public SybaseDialect() : base()
		{
			/* Java mapping was:
			
			Types.BIT, "TINYINT" );
			Types.BIGINT, "NUMERIC(19,0)" );
			Types.SMALLINT, "SMALLINT" );
			Types.TINYINT, "TINYINT" );
			Types.INTEGER, "INT" );
			Types.CHAR, "CHAR(1)" );
			Types.VARCHAR, "VARCHAR($1)" );
			Types.FLOAT, "FLOAT" );
			Types.DOUBLE, "DOUBLE PRECISION"
			Types.DATE, "DATETIME" );
			Types.TIME, "DATETIME" );
			Types.TIMESTAMP, "DATETIME" );
			Types.VARBINARY, "VARBINARY($1)"
			Types.NUMERIC, "NUMERIC(19,$1)" 
			Types.BLOB, "IMAGE" );
			Types.CLOB, "TEXT" );
			*/

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
		public override string GetIdentitySelectString(string identityColumn, string tableName)
		{
			return "select @@identity";
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