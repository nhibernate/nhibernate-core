using System.Data;
using NHibernate.AdoNet;
using NHibernate.Driver;
using NHibernate.Connection;
using NHibernate.Transaction;
using NHibernate.Exceptions;

namespace NHibernate.Cfg.Loquacious
{
	public interface IDbIntegrationConfiguration
	{
		/// <summary>
		/// Define the dialect to use.
		/// </summary>
		/// <typeparam name="TDialect">The dialect implementation inherited from <see cref="Dialect.Dialect"/>. </typeparam>
		/// <returns>The fluent configuration itself.</returns>
		IDbIntegrationConfiguration Using<TDialect>() where TDialect : Dialect.Dialect;
		IDbIntegrationConfiguration DisableKeywordsAutoImport();
		IDbIntegrationConfiguration AutoQuoteKeywords();
		IDbIntegrationConfiguration LogSqlInConsole();
		IDbIntegrationConfiguration DisableLogFormatedSql();
		
		IConnectionConfiguration Connected { get; }

		IBatcherConfiguration BatchingQueries { get; }

		ITransactionConfiguration Transactions { get; }

		ICommandsConfiguration CreateCommands { get; }

		IDbSchemaIntegrationConfiguration Schema { get; }
	}

	public interface IDbIntegrationConfigurationProperties
	{
		void Dialect<TDialect>() where TDialect : Dialect.Dialect;
		Hbm2DDLKeyWords KeywordsAutoImport { set; }
		bool LogSqlInConsole { set; }
		bool LogFormatedSql { set; }

		void ConnectionProvider<TProvider>() where TProvider : IConnectionProvider;
		void Driver<TDriver>() where TDriver : IDriver;
		IsolationLevel IsolationLevel { set; }
		ConnectionReleaseMode ConnectionReleaseMode { set; }
		string ConnectionString { set; }
		string ConnectionStringName { set; }

		void Batcher<TBatcher>() where TBatcher : IBatcherFactory;
		short BatchSize { set; }

		void TransactionFactory<TFactory>() where TFactory : ITransactionFactory;

		bool PrepareCommands { set; }
		byte Timeout { set; }
		void ExceptionConverter<TExceptionConverter>() where TExceptionConverter : ISQLExceptionConverter;
		bool AutoCommentSql { set; }
		string HqlToSqlSubstitutions { set; }
		byte MaximumDepthOfOuterJoinFetching { set; }

		SchemaAutoAction SchemaAction { set; }
	}
}