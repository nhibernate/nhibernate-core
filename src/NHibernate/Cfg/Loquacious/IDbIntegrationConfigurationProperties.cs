using System;
using System.Data;
using NHibernate.AdoNet;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Linq.Visitors;
using NHibernate.Transaction;

namespace NHibernate.Cfg.Loquacious
{
	//Since 5.3
	[Obsolete("Replaced by direct class usage")]
	public interface IDbIntegrationConfigurationProperties
	{
		void Dialect<TDialect>() where TDialect : Dialect.Dialect;
		Hbm2DDLKeyWords KeywordsAutoImport { set; }
		bool LogSqlInConsole { set; }
		bool LogFormattedSql { set; }

		//NH-3724
		void WithNotificationHandler(Delegate handler);

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
		/// <summary>
		/// Set the default timeout in seconds for ADO.NET queries.
		/// </summary>
		byte Timeout { set; }
		void ExceptionConverter<TExceptionConverter>() where TExceptionConverter : ISQLExceptionConverter;
		bool AutoCommentSql { set; }
		string HqlToSqlSubstitutions { set; }
		byte MaximumDepthOfOuterJoinFetching { set; }

		SchemaAutoAction SchemaAction { set; }

		void QueryModelRewriterFactory<TFactory>() where TFactory : IQueryModelRewriterFactory;
	}
}
