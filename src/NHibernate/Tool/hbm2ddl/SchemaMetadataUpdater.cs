using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using System.Collections.Generic;

namespace NHibernate.Tool.hbm2ddl
{
	// Candidate to be exstensions of ISessionFactory and Configuration
	public static partial class SchemaMetadataUpdater
	{
		public static void Update(ISessionFactory sessionFactory)
		{
			var factory = (ISessionFactoryImplementor) sessionFactory;
			var dialect = factory.Dialect;
			var connectionHelper = new SuppliedConnectionProviderConnectionHelper(factory.ConnectionProvider);
			factory.Dialect.Keywords.UnionWith(GetReservedWords(dialect, connectionHelper));
		}

		public static void QuoteTableAndColumns(Configuration configuration)
		{
			ISet<string> reservedDb = GetReservedWords(configuration.GetDerivedProperties());
			foreach (var cm in configuration.ClassMappings)
			{
				QuoteTable(cm.Table, reservedDb);
			}
			foreach (var cm in configuration.CollectionMappings)
			{
				QuoteTable(cm.Table, reservedDb);
			}
		}

		private static ISet<string> GetReservedWords(IDictionary<string, string> cfgProperties)
		{
			var dialect = Dialect.Dialect.GetDialect(cfgProperties);
			var connectionHelper = new ManagedProviderConnectionHelper(cfgProperties);
			return GetReservedWords(dialect, connectionHelper);
		}

		private static ISet<string> GetReservedWords(Dialect.Dialect dialect, IConnectionHelper connectionHelper)
		{
			ISet<string> reservedDb = new HashSet<string>();
			connectionHelper.Prepare();
			try
			{
				var metaData = dialect.GetDataBaseSchema(connectionHelper.Connection);
				foreach (var rw in metaData.GetReservedWords())
				{
					reservedDb.Add(rw.ToLowerInvariant());
				}
			}
			finally
			{
				connectionHelper.Release();
			}
			return reservedDb;
		}

		private static void QuoteTable(Table table, ICollection<string> reservedDb)
		{
			if (!table.IsQuoted && reservedDb.Contains(table.Name.ToLowerInvariant()))
			{
				table.Name = GetNhQuoted(table.Name);
			}
			foreach (var column in table.ColumnIterator)
			{
				if (!column.IsQuoted && reservedDb.Contains(column.Name.ToLowerInvariant()))
				{
					column.Name = GetNhQuoted(column.Name);
				}
			}
		}

		private static string GetNhQuoted(string name)
		{
			return "`" + name + "`";
		}
	}
}