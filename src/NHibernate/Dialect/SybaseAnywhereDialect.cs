using NHibernate.Cfg;

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
	/// The SybaseAnywhereDialect defaults the following configuration properties:
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
	public class SybaseAnywhereDialect : Dialect
	{
		/// <summary></summary>
		public SybaseAnywhereDialect() : base()
		{
			/* Java mapping was:
			
			Types.BIT, "TINYINT" );
			Types.BIGINT, "NUMERIC(19,0)" );
			Types.SMALLINT, "SMALLINT" );
			Types.TINYINT, "TINYINT" );
			Types.INTEGER, "INT" );
			Types.CHAR, "CHAR(1)" );
			Types.VARCHAR, "VARCHAR($l)" );
			Types.FLOAT, "FLOAT" );
			Types.DOUBLE, "DOUBLE PRECISION"
			Types.DATE, "DATETIME" );
			Types.TIME, "DATETIME" );
			Types.TIMESTAMP, "DATETIME" );
			Types.VARBINARY, "VARBINARY($l)"
			Types.NUMERIC, "NUMERIC(19,$l)" 
			Types.BLOB, "IMAGE" );
			Types.CLOB, "TEXT" );
			*/

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.ASAClientDriver";
			DefaultProperties[Environment.PrepareSql] = "false";
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

		/// <summary>
		/// ASA does not require to drop constraint before dropping tables, and DROP statement
		/// syntax used by Hibernate to drop constraint is not compatible with ASA, so disable it.  
		/// Comments matchs SybaseAnywhereDialect from Hibernate-3.1 src
		/// </summary>
		public override bool DropConstraints
		{
			get { return false; }
		}
	}
}