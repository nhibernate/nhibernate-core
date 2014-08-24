using System.Data;
using NHibernate.AdoNet;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Transaction;

namespace NHibernate.Cfg.Loquacious
{
	public interface IDbIntegrationConfigurationProperties
	{
		void Dialect<TDialect>() where TDialect : Dialect.Dialect;
		Hbm2DDLKeyWords KeywordsAutoImport { set; }
		bool LogSqlInConsole { set; }
		bool LogFormattedSql { set; }

		void ConnectionProvider<TProvider>() where TProvider : IConnectionProvider;
		void Driver<TDriver>() where TDriver : IDriver;
		IsolationLevel IsolationLevel { set; }
		ConnectionReleaseMode ConnectionReleaseMode { set; }
		string ConnectionString { set; }
		string ConnectionStringName { set; }

		void Batcher<TBatcher>() where TBatcher : IBatcherFactory;
		short BatchSize { set; }
		bool OrderInserts { set; }

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