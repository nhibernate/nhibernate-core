using System;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Mapping;
using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Tool.hbm2ddl
{
	// Candidate to be exstensions of ISessionFactory and Configuration
	public static partial class SchemaMetadataUpdater
	{
		public static void Update(ISessionFactoryImplementor sessionFactory)
		{
			UpdateDialectKeywords(
				sessionFactory.Dialect,
				new SuppliedConnectionProviderConnectionHelper(sessionFactory.ConnectionProvider));
		}

		public static void Update(Configuration configuration, Dialect.Dialect dialect)
		{
			UpdateDialectKeywords(
				dialect,
				new ManagedProviderConnectionHelper(configuration.GetDerivedProperties()));
		}

		static void UpdateDialectKeywords(Dialect.Dialect dialect, IConnectionHelper connectionHelper)
		{
			dialect.RegisterKeywords(GetReservedWords(dialect, connectionHelper));
		}

		static IEnumerable<string> GetReservedWords(Dialect.Dialect dialect, IConnectionHelper connectionHelper)
		{
			connectionHelper.Prepare();
			try
			{
				var metaData = dialect.GetDataBaseSchema(connectionHelper.Connection);
				return metaData.GetReservedWords();
			}
			finally
			{
				connectionHelper.Release();
			}
		}

		// Since v5
		[Obsolete("Use the overload that passes dialect so keywords will be updated and persisted before auto-quoting")]
		public static void QuoteTableAndColumns(Configuration configuration)
		{
			// Instantiates a new instance of the dialect so doesn't benefit from the Update call.
			var dialect = Dialect.Dialect.GetDialect(configuration.GetDerivedProperties());
			Update(configuration, dialect);
			QuoteTableAndColumns(configuration, dialect);
		}

		public static void QuoteTableAndColumns(Configuration configuration, Dialect.Dialect dialect)
		{
			foreach (var cm in configuration.ClassMappings)
			{
				QuoteTable(cm.Table,cm.Key?.ColumnIterator?.OfType<Column>(), dialect);
			}

			foreach (var cm in configuration.CollectionMappings)
			{
				QuoteTable(cm.Table, cm.Key?.ColumnIterator?.OfType<Column>(), dialect);
			}
		}

		private static void QuoteColumns(IEnumerable<Column> columns, Dialect.Dialect dialect)
		{
			if (columns != null)
			{
				foreach (Column column in columns)
				{
					if (!column.IsQuoted && dialect.IsKeyword(column.Name))
					{
						column.IsQuoted = true;
					}
				}
			}
		}

		private static void QuoteTable(Table table, IEnumerable<Column> columns, Dialect.Dialect dialect)
		{
			if (!table.IsQuoted && dialect.IsKeyword(table.Name))
			{
				table.IsQuoted = true;
			}

			foreach (var column in table.ColumnIterator)
			{
				if (!column.IsQuoted && dialect.IsKeyword(column.Name))
				{
					column.IsQuoted = true;
				}
			}
			
			QuoteColumns(columns, dialect);
		}
	}
}