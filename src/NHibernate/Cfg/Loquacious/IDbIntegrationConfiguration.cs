using System;

namespace NHibernate.Cfg.Loquacious
{
	public interface IDbIntegrationConfiguration
	{
		/// <summary>
		/// Define and configure the dialect to use.
		/// </summary>
		/// <typeparam name="TDialect">The dialect implementation inherited from <see cref="Dialect.Dialect"/>.</typeparam>
		/// <returns>The fluent configuration itself.</returns>
		IDbIntegrationConfiguration Using<TDialect>() where TDialect : Dialect.Dialect;

		IDbIntegrationConfiguration DisableKeywordsAutoImport();

		IDbIntegrationConfiguration AutoQuoteKeywords();

		IDbIntegrationConfiguration LogSqlInConsole();

		IDbIntegrationConfiguration EnableLogFormattedSql();

		[Obsolete("Please use EnableLogFormattedSql. This method will be removed in a future version.")]
		IDbIntegrationConfiguration DisableLogFormatedSql();

		IConnectionConfiguration Connected { get; }

		IBatcherConfiguration BatchingQueries { get; }

		ITransactionConfiguration Transactions { get; }

		ICommandsConfiguration CreateCommands { get; }

		IDbSchemaIntegrationConfiguration Schema { get; }
	}
}