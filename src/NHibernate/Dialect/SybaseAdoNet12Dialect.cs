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
	public class SybaseAdoNet12Dialect : Dialect
	{
		/// <summary></summary>
		public SybaseAdoNet12Dialect()
		{
			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.SybaseAdoNet12ClientDriver";
			DefaultProperties[Environment.PrepareSql] = "true";
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