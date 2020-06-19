using System.Data;
using NHibernate.AdoNet;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Linq.Visitors;
using NHibernate.Transaction;

namespace NHibernate.Cfg.Loquacious
{
	public class DbIntegrationConfigurationProperties
#pragma warning disable 618
		: IDbIntegrationConfigurationProperties
#pragma warning restore 618
	{
		private readonly Configuration configuration;

		public DbIntegrationConfigurationProperties(Configuration configuration)
		{
			this.configuration = configuration;
		}

		#region Implementation of IDbIntegrationConfigurationProperties

		public void Dialect<TDialect>() where TDialect : Dialect.Dialect
		{
			configuration.SetProperty(Environment.Dialect, typeof(TDialect).AssemblyQualifiedName);
		}

		public Hbm2DDLKeyWords KeywordsAutoImport
		{
			set { configuration.SetProperty(Environment.Hbm2ddlKeyWords, value.ToString()); }
		}

		public bool LogSqlInConsole
		{
			set { configuration.SetProperty(Environment.ShowSql, value.ToString().ToLowerInvariant()); }
		}

		public bool LogFormattedSql
		{
			set { configuration.SetProperty(Environment.FormatSql, value.ToString().ToLowerInvariant()); }
		}

		public void ConnectionProvider<TProvider>() where TProvider : IConnectionProvider
		{
			configuration.SetProperty(Environment.ConnectionProvider, typeof(TProvider).AssemblyQualifiedName);
		}

		public void Driver<TDriver>() where TDriver : IDriver
		{
			configuration.SetProperty(Environment.ConnectionDriver, typeof(TDriver).AssemblyQualifiedName);
		}

		public IsolationLevel IsolationLevel
		{
			set { configuration.SetProperty(Environment.Isolation, value.ToString()); }
		}

		public ConnectionReleaseMode ConnectionReleaseMode
		{
			set { configuration.SetProperty(Environment.ReleaseConnections, ConnectionReleaseModeParser.ToString(value)); }
		}

		public string ConnectionString
		{
			set { configuration.SetProperty(Environment.ConnectionString, value); }
		}

		public string ConnectionStringName
		{
			set { configuration.SetProperty(Environment.ConnectionStringName, value); }
		}

		public void Batcher<TBatcher>() where TBatcher : IBatcherFactory
		{
			configuration.SetProperty(Environment.BatchStrategy, typeof(TBatcher).AssemblyQualifiedName);
		}

		public short BatchSize
		{
			set { configuration.SetProperty(Environment.BatchSize, value.ToString()); }
		}

		public bool OrderInserts
		{
			set { configuration.SetProperty(Environment.OrderInserts, value.ToString().ToLowerInvariant()); }
		}

		public void TransactionFactory<TFactory>() where TFactory : ITransactionFactory
		{
			configuration.SetProperty(Environment.TransactionStrategy, typeof(TFactory).AssemblyQualifiedName);
		}

		public bool PrepareCommands
		{
			set { configuration.SetProperty(Environment.PrepareSql, value.ToString().ToLowerInvariant()); }
		}

		/// <summary>
		/// Set the default timeout in seconds for ADO.NET queries.
		/// </summary>
		public byte Timeout
		{
			set { configuration.SetProperty(Environment.CommandTimeout, value.ToString()); }
		}

		public void ExceptionConverter<TExceptionConverter>() where TExceptionConverter : ISQLExceptionConverter
		{
			configuration.SetProperty(Environment.SqlExceptionConverter, typeof(TExceptionConverter).AssemblyQualifiedName);
		}

		public bool AutoCommentSql
		{
			set { configuration.SetProperty(Environment.UseSqlComments, value.ToString().ToLowerInvariant()); }
		}

		public string HqlToSqlSubstitutions
		{
			set { configuration.SetProperty(Environment.QuerySubstitutions, value); }
		}

		public byte MaximumDepthOfOuterJoinFetching
		{
			set { configuration.SetProperty(Environment.MaxFetchDepth, value.ToString()); }
		}

		public SchemaAutoAction SchemaAction
		{
			set { configuration.SetProperty(Environment.Hbm2ddlAuto, value.ToString()); }
		}

		// 6.0 TODO default should become true
		/// <summary>
		/// Whether to throw or not on schema auto-update failures. <see langword="false" /> by default.
		/// </summary>
		public bool ThrowOnSchemaUpdate
		{
			set { configuration.SetProperty(Environment.Hbm2ddlThrowOnUpdate, value.ToString().ToLowerInvariant()); }
		}

		public void QueryModelRewriterFactory<TFactory>() where TFactory : IQueryModelRewriterFactory
		{
			configuration.SetProperty(Environment.QueryModelRewriterFactory, typeof(TFactory).AssemblyQualifiedName);
		}

		/// <summary>
		/// Set the class of the LINQ query pre-transformer registrar.
		/// </summary>
		/// <typeparam name="TRegistrar">The class of the LINQ query pre-transformer registrar.</typeparam>
		public void PreTransformerRegistrar<TRegistrar>() where TRegistrar : IExpressionTransformerRegistrar
		{
			configuration.SetProperty(Environment.PreTransformerRegistrar, typeof(TRegistrar).AssemblyQualifiedName);
		}

		#endregion
	}
}
