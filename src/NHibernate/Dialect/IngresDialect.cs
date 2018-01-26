using System.Data;
using NHibernate.Cfg;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for IngresSQL.
	/// </summary>
	/// <remarks>
	/// The IngresDialect defaults the following configuration properties:
	/// <list type="table">
	///		<listheader>
	///			<term>Property</term>
	///			<description>Default Value</description>
	///		</listheader>
	///		<item>
	///			<term>connection.driver_class</term>
	///			<description>NHibernate.Driver.IngresDriver</description>
	///		</item>
	/// </list>
	/// </remarks>
	public class IngresDialect : Dialect
	{
		public IngresDialect()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "char(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 8000, "char($l)");
			RegisterColumnType(DbType.AnsiString, "varchar(255)");
			RegisterColumnType(DbType.AnsiString, 8000, "varchar($l)");
			RegisterColumnType(DbType.AnsiString, 2147483647, "text");
			RegisterColumnType(DbType.Binary, "bytea");
			RegisterColumnType(DbType.Binary, 2147483647, "bytea");
			RegisterColumnType(DbType.Boolean, "boolean");
			RegisterColumnType(DbType.Byte, "int2");
			RegisterColumnType(DbType.Currency, "decimal(16,4)");
			RegisterColumnType(DbType.Date, "date");
			RegisterColumnType(DbType.DateTime, "timestamp");
			RegisterColumnType(DbType.Decimal, "decimal(19,5)");
			// Ingres max precision is 31, but .Net is limited to 28-29.
			RegisterColumnType(DbType.Decimal, 28, "decimal($p, $s)");
			RegisterColumnType(DbType.Double, "float8");
			RegisterColumnType(DbType.Int16, "int2");
			RegisterColumnType(DbType.Int32, "int4");
			RegisterColumnType(DbType.Int64, "int8");
			RegisterColumnType(DbType.Single, "float4");
			RegisterColumnType(DbType.StringFixedLength, "char(255)");
			RegisterColumnType(DbType.StringFixedLength, 4000, "char($l)");
			RegisterColumnType(DbType.String, "varchar(255)");
			RegisterColumnType(DbType.String, 4000, "varchar($l)");
			//RegisterColumnType(DbType.String, 1073741823, "text"); //
			//RegisterColumnType(DbType.Time, "time");

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.IngresDriver";
		}

		// Ingres 10.2 supports 256 bytes (so worst unicode case would mean 64 characters), but I am unable to find
		// the limit for older versions, excepted many various sites mention a 32 length limit.
		// https://unifaceinfo.com/docs/0906/Uniface_Library_HTML/ulibrary/INS_NAMING_RULES_8EEFC1A489331BF969D2A8AA36AF2832.html
		// There are traces of a ticket for increasing this in version 10: http://lists.ingres.com/pipermail/bugs/2010-May/000052.html
		// This dialect seems to target version below 9, since there is Ingres9Dialect deriving from it.
		// So sticking to 32.
		/// <inheritdoc />
		public override int MaxAliasLength => 32;

		#region Overridden informational metadata

		public override bool SupportsEmptyInList => false;

		public override bool SupportsSubselectAsInPredicateLHS => false;

		public override bool SupportsExpectedLobUsagePattern => false;

		public override bool DoesReadCommittedCauseWritersToBlockReaders => true;

		#endregion
	}
}
