using System.Data;
using NHibernate.Mapping;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Dialect
{
	public class MsSql2005Dialect : MsSql2000Dialect
	{
		///<remarks>http://stackoverflow.com/a/7264795/259946</remarks>
		public const int MaxSizeForXml = 2147483647; // int.MaxValue

		public MsSql2005Dialect()
		{
			RegisterColumnType(DbType.Xml, "XML");
		}

		protected override void RegisterCharacterTypeMappings()
		{
			base.RegisterCharacterTypeMappings();
			RegisterColumnType(DbType.String, MaxSizeForClob, "NVARCHAR(MAX)");
			RegisterColumnType(DbType.AnsiString, MaxSizeForAnsiClob, "VARCHAR(MAX)");
		}

		protected override void RegisterLargeObjectTypeMappings()
		{
			base.RegisterLargeObjectTypeMappings();
			RegisterColumnType(DbType.Binary, "VARBINARY(MAX)");
			RegisterColumnType(DbType.Binary, MaxSizeForLengthLimitedBinary, "VARBINARY($l)");
			RegisterColumnType(DbType.Binary, MaxSizeForBlob, "VARBINARY(MAX)");
		}

		protected override void RegisterKeywords()
		{
			base.RegisterKeywords();
			RegisterKeyword("xml");
		}

		public override SqlString GetLimitString(SqlString queryString, SqlString offset, SqlString limit)
		{
			return new MsSql2005DialectQueryPager(queryString).PageBy(offset, limit);
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimit => true;

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>true</c></value>
		public override bool SupportsLimitOffset => true;

		public override bool SupportsVariableLimit => true;

		protected override string GetSelectExistingObject(string catalog, string schema, string table, string name)
		{
			return
				string.Format(
					"select 1 from {0} where object_id = OBJECT_ID(N'{1}') and parent_object_id = OBJECT_ID(N'{2}')",
					Qualify(catalog, "sys", "objects"),
					Qualify(catalog, schema, Quote(name)),
					Qualify(catalog, schema, table));
		}

		/// <summary>
		/// Sql Server 2005 supports a query statement that provides <c>LIMIT</c>
		/// functionality with an offset.
		/// </summary>
		/// <value><c>false</c></value>
		public override bool UseMaxForLimit => false;

		public override string AppendLockHint(LockMode lockMode, string tableName)
		{
			if (NeedsLockHint(lockMode))
			{
				if (lockMode == LockMode.UpgradeNoWait)
				{
					return tableName + " with (updlock, rowlock, nowait)";
				}

				return tableName + " with (updlock, rowlock)";
			}

			return tableName;
		}

		// SQL Server 2005 supports 128.
		/// <inheritdoc />
		public override int MaxAliasLength => 128;

		#region Overridden informational metadata

		/// <summary>
		/// We assume that applications using this dialect are using
		/// SQL Server 2005 snapshot isolation modes.
		/// </summary>
		public override bool DoesReadCommittedCauseWritersToBlockReaders => false;

		/// <summary>
		/// We assume that applications using this dialect are using
		/// SQL Server 2005 snapshot isolation modes.
		/// </summary>
		public override bool DoesRepeatableReadCauseReadersToBlockWriters => false;

		#endregion
	}
}
