using System;
using System.Data;
using Environment = NHibernate.Cfg.Environment;

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
	///			<description><see cref="NHibernate.Driver.IngresDriver" /></description>
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
			RegisterColumnType(DbType.Decimal, 19, "decimal(18, $l)");
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

	    public override bool SupportsAlterColumn
	    {
	        get { return false; }
	    }

	    public override bool SupportsRenameColumn
	    {
	        get { return true; }
	    }

        public override string GetRenameColumnString(string tableName, string oldColumnName, string newColumnName)
        {
            return String.Format("ALTER TABLE {0} RENAME COLUMN {1} TO {2}", tableName, oldColumnName, newColumnName);
        }

	  public override string GetRenameTableString(string oldTableName, string newTableName)
	  {
      return String.Format("ALTER TABLE {0} RENAME  {0}  TO  {1}", oldTableName, newTableName);
	  }
	}
}