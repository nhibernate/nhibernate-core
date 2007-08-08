using System;
using System.Data;
using System.Text;
using NHibernate.SqlCommand;
using NHibernate.Util;
using Environment=NHibernate.Cfg.Environment;

namespace NHibernate.Dialect
{
	/// <summary>
	/// A SQL dialect for MySQL
	/// </summary>
	/// <remarks>
	/// The MySQLDialect defaults the following configuration properties:
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
	///			<description><see cref="NHibernate.Driver.MySqlDataDriver" /></description>
	///		</item>
	/// </list>
	/// </remarks>
	public class MySQLDialect : Dialect
	{
		/// <summary></summary>
		public MySQLDialect() : base()
		{
			RegisterColumnType(DbType.AnsiStringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 255, "CHAR($1)");
			RegisterColumnType(DbType.AnsiStringFixedLength, 65535, "TEXT");
			RegisterColumnType(DbType.AnsiStringFixedLength, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.AnsiString, "VARCHAR(255)");
			RegisterColumnType(DbType.AnsiString, 255, "VARCHAR($1)");
			RegisterColumnType(DbType.AnsiString, 65535, "TEXT");
			RegisterColumnType(DbType.AnsiString, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.Binary, "LONGBLOB");
			RegisterColumnType(DbType.Binary, 127, "TINYBLOB");
			RegisterColumnType(DbType.Binary, 65535, "BLOB");
			RegisterColumnType(DbType.Binary, 16777215, "MEDIUMBLOB");
			RegisterColumnType(DbType.Boolean, "TINYINT(1)");
			RegisterColumnType(DbType.Byte, "TINYINT UNSIGNED");
			RegisterColumnType(DbType.Currency, "MONEY");
			RegisterColumnType(DbType.Date, "DATE");
			RegisterColumnType(DbType.DateTime, "DATETIME");
			RegisterColumnType(DbType.Decimal, "NUMERIC(19,5)");
			RegisterColumnType(DbType.Decimal, 19, "NUMERIC(19, $1)");
			RegisterColumnType(DbType.Double, "DOUBLE");
			RegisterColumnType(DbType.Guid, "VARCHAR(40)");
			RegisterColumnType(DbType.Int16, "SMALLINT");
			RegisterColumnType(DbType.Int32, "INTEGER");
			RegisterColumnType(DbType.Int64, "BIGINT");
			RegisterColumnType(DbType.Single, "FLOAT");
			RegisterColumnType(DbType.StringFixedLength, "CHAR(255)");
			RegisterColumnType(DbType.StringFixedLength, 255, "CHAR($1)");
			RegisterColumnType(DbType.StringFixedLength, 65535, "TEXT");
			RegisterColumnType(DbType.StringFixedLength, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.String, "VARCHAR(255)");
			RegisterColumnType(DbType.String, 255, "VARCHAR($1)");
			RegisterColumnType(DbType.String, 65535, "TEXT");
			RegisterColumnType(DbType.String, 16777215, "MEDIUMTEXT");
			RegisterColumnType(DbType.Time, "TIME");

			DefaultProperties[Environment.ConnectionDriver] = "NHibernate.Driver.MySqlDataDriver";
		}

		/// <summary></summary>
		public override string AddColumnString
		{
			get { return "add column"; }
		}

		/// <summary></summary>
		public override bool QualifyIndexName
		{
			get { return false; }
		}

		/// <summary></summary>
		public override bool SupportsIdentityColumns
		{
			get { return true; }
		}

		/// <summary></summary>
		public override string GetIdentitySelectString(string identityColumn, string tableName)
		{
			return "SELECT LAST_INSERT_ID()";
		}

		/// <summary></summary>
		public override string IdentityColumnString
		{
			get { return "NOT NULL AUTO_INCREMENT"; }
		}

		/// <summary></summary>
		public override char CloseQuote
		{
			get { return '`'; }
		}

		/// <summary></summary>
		public override char OpenQuote
		{
			get { return '`'; }
		}

		protected override bool SupportsIfExistsBeforeTableName
		{
			get { return true; }
		}

		/// <summary></summary>
		public override bool SupportsLimit
		{
			get { return true; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="querySqlString"></param>
		/// <param name="hasOffset"></param>
		/// <returns></returns>
		public override SqlString GetLimitString(SqlString querySqlString, bool hasOffset)
		{
			SqlStringBuilder pagingBuilder = new SqlStringBuilder();
			pagingBuilder.Add(querySqlString);
			pagingBuilder.Add(" limit ");
			pagingBuilder.Add(Parameter.Placeholder);

			if (hasOffset)
			{
				pagingBuilder.Add(", ");
				pagingBuilder.Add(Parameter.Placeholder);
			}

			return pagingBuilder.ToSqlString();
		}

		/// <summary>
		/// </summary>
		/// <param name="parentTable"></param>
		/// <param name="constraintName"></param>
		/// <param name="foreignKey"></param>
		/// <param name="referencedTable"></param>
		/// <param name="primaryKey"></param>
		/// <returns></returns>
		public override string GetAddForeignKeyConstraintString(string parentTable, string constraintName, string[] foreignKey,
		                                                        string referencedTable, string[] primaryKey)
		{
			string cols = String.Join(StringHelper.CommaSpace, foreignKey);
			return new StringBuilder(30)
				.Append(" add index (")
				.Append(cols)
				.Append("), add constraint ")
				.Append(constraintName)
				.Append(" foreign key (")
				.Append(cols)
				.Append(") references ")
				.Append(referencedTable)
				.Append(" (")
				.Append(String.Join(StringHelper.CommaSpace, primaryKey))
				.Append(')')
				.ToString();
		}

		/// <summary>
		/// Create the SQL string to drop a foreign key constraint.
		/// </summary>
		/// <param name="constraintName">The name of the foreign key to drop.</param>
		/// <returns>The SQL string to drop the foreign key constraint.</returns>
		public override string GetDropForeignKeyConstraintString(string constraintName)
		{
			return " drop foreign key " + constraintName;
		}

		/// <summary>
		/// Create the SQL string to drop a primary key constraint.
		/// </summary>
		/// <param name="constraintName">The name of the primary key to drop.</param>
		/// <returns>The SQL string to drop the primary key constraint.</returns>
		public override string GetDropPrimaryKeyConstraintString(string constraintName)
		{
			return " drop primary key " + constraintName;
		}

		/// <summary>
		/// Create the SQL string to drop an index.
		/// </summary>
		/// <param name="constraintName">The name of the index to drop.</param>
		/// <returns>The SQL string to drop the index constraint.</returns>
		public override string GetDropIndexConstraintString(string constraintName)
		{
			return " drop index " + constraintName;
		}

		public override bool SupportsSubSelects
		{
			// TODO: newer MySQLs actually support subselects
			get { return false; }
		}
	}
}