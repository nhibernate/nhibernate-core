using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Transaction;

namespace NHibernate.Driver
{
	public class SqlBulkProvider : TableBasedBulkProvider
	{
		public const String BulkProviderOptions = "adonet.bulk_provider_options";

		public SqlBulkCopyOptions Options { get; set; }


		public override void Initialize(IDictionary<String, String> properties)
		{
			base.Initialize(properties);

			var bulkProviderOptions = String.Empty;

			if (properties.TryGetValue(BulkProviderOptions, out bulkProviderOptions) == true)
			{
				this.Options = (SqlBulkCopyOptions)Enum.Parse(typeof(SqlBulkCopyOptions), bulkProviderOptions, true);
			}
		}

		public override void Insert<T>(ISessionImplementor session, IEnumerable<T> entities)
		{
			if (entities.Any() == true)
			{
				var con = session.Connection as SqlConnection;
				var tx = (session.ConnectionManager.Transaction as AdoTransaction).GetNativeTransaction() as SqlTransaction;

				foreach (var table in this.GetTables(session, entities))
				{
					using (var copy = new SqlBulkCopy(con, this.Options, tx) { BatchSize = this.BatchSize, BulkCopyTimeout = this.Timeout, DestinationTableName = table.TableName })
					{
						copy.WriteToServer(table);
					}
				}
			}
		}
	}
}
