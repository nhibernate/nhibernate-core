using System;
using System.Data;

namespace NHibernate.Dialect
{
	/// <summary>
	/// An SQL dialect for PostgreSQL 8.2 and above.
	/// </summary>
	/// <remarks>
	/// PostgreSQL 8.2 supports <c>DROP TABLE IF EXISTS tablename</c>
	/// and <c>DROP SEQUENCE IF EXISTS sequencename</c> syntax.
	/// See <see cref="PostgreSQLDialect" /> for more information.
	/// </remarks>
	public class PostgreSQL82Dialect : PostgreSQL81Dialect
	{
		public PostgreSQL82Dialect()
		{
			RegisterColumnType(DbType.Guid, "uuid");
		}

		public override bool SupportsIfExistsBeforeTableName
		{
			get { return true; }
		}

		public override string GetDropSequenceString(string sequenceName)
		{
			return string.Concat("drop sequence if exists ", sequenceName);
		}
	}
}