using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using NHibernate.Engine;

namespace NHibernate.Driver
{
	public class OracleDataClientBulkProvider : TableBasedBulkProvider
	{
		public const String BulkProviderOptions = "adonet.bulk_provider_options";

		private static readonly System.Type bulkCopyOptionsType = System.Type.GetType("Oracle.DataAccess.Client.OracleBulkCopyOptions, Oracle.DataAccess");
		private static readonly System.Type bulkCopyType = System.Type.GetType("Oracle.DataAccess.Client.OracleBulkCopy, Oracle.DataAccess");
		private static readonly PropertyInfo batchSizeProperty = bulkCopyType.GetProperty("BatchSize");
		private static readonly PropertyInfo bulkCopyTimeoutProperty = bulkCopyType.GetProperty("BulkCopyTimeout");
		private static readonly PropertyInfo destinationTableNameProperty = bulkCopyType.GetProperty("DestinationTableName");
		private static readonly MethodInfo writeToServerMethod = bulkCopyType.GetMethod("WriteToServer", new System.Type[] { typeof(DataTable) });

		public Int32 Options { get; set; }

		public Int32 NotifyAfter { get; set; }

		public override void Initialize(IDictionary<String, String> properties)
		{
			base.Initialize(properties);

			var bulkProviderOptions = String.Empty;

			if (properties.TryGetValue(BulkProviderOptions, out bulkProviderOptions))
			{
				this.Options = Convert.ToInt32(bulkProviderOptions);
			}
		}

		public override void Insert<T>(ISessionImplementor session, IEnumerable<T> entities)
		{
			if (entities.Any() == true)
			{
				foreach (var table in this.GetTables(session, entities))
				{
					using (var copy = Activator.CreateInstance(bulkCopyType, session.Connection, Enum.ToObject(bulkCopyOptionsType,  this.Options)) as IDisposable)
					{
						batchSizeProperty.SetValue(copy, this.BatchSize, null);
						bulkCopyTimeoutProperty.SetValue(copy, this.Timeout, null);
						destinationTableNameProperty.SetValue(copy, table.TableName, null);

						writeToServerMethod.Invoke(copy, new Object[] { table });
					}
				}
			}
		}
	}
}
