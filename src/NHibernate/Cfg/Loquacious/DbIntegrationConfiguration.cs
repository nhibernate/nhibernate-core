using System;
using System.Data;
using System.Data.Common;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.AdoNet;
using NHibernate.Exceptions;
using NHibernate.Transaction;

namespace NHibernate.Cfg.Loquacious
{
	public class DbIntegrationConfiguration 
#pragma warning disable 618
		: IDbIntegrationConfiguration
#pragma warning restore 618
	{
		private readonly Configuration configuration;

		public DbIntegrationConfiguration(Configuration configuration)
		{
			this.configuration = configuration;
			Connected = new ConnectionConfiguration(this);
			BatchingQueries = new BatcherConfiguration(this);
			Transactions = new TransactionConfiguration(this);
			CreateCommands = new CommandsConfiguration(this);
			Schema = new DbSchemaIntegrationConfiguration(this);
		}

		public Configuration Configuration => configuration;

		/// <summary>
		/// Define and configure the dialect to use.
		/// </summary>
		/// <typeparam name="TDialect">The dialect implementation inherited from <see cref="Dialect.Dialect"/>.</typeparam>
		/// <returns>The fluent configuration itself.</returns>
		public DbIntegrationConfiguration Using<TDialect>() where TDialect : Dialect.Dialect
		{
			configuration.SetProperty(Environment.Dialect, typeof(TDialect).AssemblyQualifiedName);
			return this;
		}

		public DbIntegrationConfiguration DisableKeywordsAutoImport()
		{
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "none");
			return this;
		}

		public DbIntegrationConfiguration AutoQuoteKeywords()
		{
			configuration.SetProperty(Environment.Hbm2ddlKeyWords, "auto-quote");
			return this;
		}

		public DbIntegrationConfiguration LogSqlInConsole()
		{
			configuration.SetProperty(Environment.ShowSql, "true");
			return this;
		}

		public DbIntegrationConfiguration EnableLogFormattedSql()
		{
			configuration.SetProperty(Environment.FormatSql, "true");
			return this;
		}

		public ConnectionConfiguration Connected { get; }

		public BatcherConfiguration BatchingQueries { get; }

		public TransactionConfiguration Transactions { get; }

		public CommandsConfiguration CreateCommands { get; }

		public DbSchemaIntegrationConfiguration Schema { get; }

		#region Implementation of IDbIntegrationConfiguration
#pragma warning disable 618

		IDbIntegrationConfiguration IDbIntegrationConfiguration.Using<TDialect>()
		{
			return Using<TDialect>();
		}

		IDbIntegrationConfiguration IDbIntegrationConfiguration.DisableKeywordsAutoImport()
		{
			return DisableKeywordsAutoImport();
		}

		IDbIntegrationConfiguration IDbIntegrationConfiguration.AutoQuoteKeywords()
		{
			return AutoQuoteKeywords();
		}

		IDbIntegrationConfiguration IDbIntegrationConfiguration.LogSqlInConsole()
		{
			return LogSqlInConsole();
		}

		IDbIntegrationConfiguration IDbIntegrationConfiguration.EnableLogFormattedSql()
		{
			return EnableLogFormattedSql();
		}

		IConnectionConfiguration IDbIntegrationConfiguration.Connected => Connected;

		IBatcherConfiguration IDbIntegrationConfiguration.BatchingQueries => BatchingQueries;

		ITransactionConfiguration IDbIntegrationConfiguration.Transactions => Transactions;

		ICommandsConfiguration IDbIntegrationConfiguration.CreateCommands => CreateCommands;

		IDbSchemaIntegrationConfiguration IDbIntegrationConfiguration.Schema => Schema;

#pragma warning restore 618
		#endregion
	}

	public class DbSchemaIntegrationConfiguration 
#pragma warning disable 618
		: IDbSchemaIntegrationConfiguration
#pragma warning restore 618
	{
		private readonly DbIntegrationConfiguration dbc;

		public DbSchemaIntegrationConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		public DbIntegrationConfiguration Recreating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Recreate.ToString());
			return dbc;
		}

		public DbIntegrationConfiguration Creating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Create.ToString());
			return dbc;
		}

		public DbIntegrationConfiguration Updating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Update.ToString());
			return dbc;
		}

		public DbIntegrationConfiguration Validating()
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlAuto, SchemaAutoAction.Validate.ToString());
			return dbc;
		}

		// 6.0 TODO default should become true
		/// <summary>
		/// Whether to throw or not on schema auto-update failures. <see langword="false" /> by default.
		/// </summary>
		/// <param name="throw"><see langword="true" /> to throw in case any failure is reported during schema auto-update,
		/// <see langword="false" /> to ignore failures.</param>
		public DbIntegrationConfiguration ThrowOnSchemaUpdate(bool @throw)
		{
			dbc.Configuration.SetProperty(Environment.Hbm2ddlThrowOnUpdate, @throw.ToString().ToLowerInvariant());
			return dbc;
		}

		#region Implementation of IDbSchemaIntegrationConfiguration
#pragma warning disable 618

		IDbIntegrationConfiguration IDbSchemaIntegrationConfiguration.Recreating()
		{
			return Recreating();
		}

		IDbIntegrationConfiguration IDbSchemaIntegrationConfiguration.Creating()
		{
			return Creating();
		}

		IDbIntegrationConfiguration IDbSchemaIntegrationConfiguration.Updating()
		{
			return Updating();
		}

		IDbIntegrationConfiguration IDbSchemaIntegrationConfiguration.Validating()
		{
			return Validating();
		}

#pragma warning restore 618
		#endregion
	}

	public class CommandsConfiguration 
#pragma warning disable 618
		: ICommandsConfiguration
#pragma warning restore 618
	{
		private readonly DbIntegrationConfiguration dbc;

		public CommandsConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		public CommandsConfiguration Preparing()
		{
			dbc.Configuration.SetProperty(Environment.PrepareSql, "true");
			return this;
		}

		public CommandsConfiguration WithTimeout(byte seconds)
		{
			dbc.Configuration.SetProperty(Environment.CommandTimeout, seconds.ToString());
			return this;
		}

		public CommandsConfiguration ConvertingExceptionsThrough<TExceptionConverter>()
			where TExceptionConverter : ISQLExceptionConverter
		{
			dbc.Configuration.SetProperty(Environment.SqlExceptionConverter, typeof(TExceptionConverter).AssemblyQualifiedName);
			return this;
		}

		public CommandsConfiguration AutoCommentingSql()
		{
			dbc.Configuration.SetProperty(Environment.UseSqlComments, "true");
			return this;
		}

		public DbIntegrationConfiguration WithHqlToSqlSubstitutions(string csvQuerySubstitutions)
		{
			dbc.Configuration.SetProperty(Environment.QuerySubstitutions, csvQuerySubstitutions);
			return dbc;
		}

		public DbIntegrationConfiguration WithDefaultHqlToSqlSubstitutions()
		{
			return dbc;
		}

		/// <summary>
		/// Maximum depth of outer join fetching
		/// </summary>
		/// <remarks>
		/// 0 (zero) disable the usage of OuterJoinFetching
		/// </remarks>
		public CommandsConfiguration WithMaximumDepthOfOuterJoinFetching(byte maxFetchDepth)
		{
			dbc.Configuration.SetProperty(Environment.MaxFetchDepth, maxFetchDepth.ToString());
			return this;
		}

		#region Implementation of ICommandsConfiguration
#pragma warning disable 618

		ICommandsConfiguration ICommandsConfiguration.Preparing()
		{
			return Preparing();
		}

		ICommandsConfiguration ICommandsConfiguration.WithTimeout(byte seconds)
		{
			return WithTimeout(seconds);
		}

		ICommandsConfiguration ICommandsConfiguration.ConvertingExceptionsThrough<TExceptionConverter>()
		{
			return ConvertingExceptionsThrough<TExceptionConverter>();
		}

		ICommandsConfiguration ICommandsConfiguration.AutoCommentingSql()
		{
			return AutoCommentingSql();
		}

		IDbIntegrationConfiguration ICommandsConfiguration.WithHqlToSqlSubstitutions(string csvQuerySubstitutions)
		{
			return WithHqlToSqlSubstitutions(csvQuerySubstitutions);
		}

		IDbIntegrationConfiguration ICommandsConfiguration.WithDefaultHqlToSqlSubstitutions()
		{
			return WithDefaultHqlToSqlSubstitutions();
		}

		ICommandsConfiguration ICommandsConfiguration.WithMaximumDepthOfOuterJoinFetching(byte maxFetchDepth)
		{
			return WithMaximumDepthOfOuterJoinFetching(maxFetchDepth);
		}

#pragma warning restore 618
		#endregion
	}

	public class TransactionConfiguration 
#pragma warning disable 618
		: ITransactionConfiguration
#pragma warning restore 618
	{
		private readonly DbIntegrationConfiguration dbc;

		public TransactionConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		public DbIntegrationConfiguration Through<TFactory>() where TFactory : ITransactionFactory
		{
			dbc.Configuration.SetProperty(Environment.TransactionStrategy, typeof(TFactory).AssemblyQualifiedName);
			return dbc;
		}

		#region Implementation of ITransactionConfiguration
#pragma warning disable 618

		IDbIntegrationConfiguration ITransactionConfiguration.Through<TFactory>()
		{
			return Through<TFactory>();
		}

#pragma warning restore 618
		#endregion
	}

	public class BatcherConfiguration
#pragma warning disable 618
		: IBatcherConfiguration
#pragma warning restore 618
	{
		private readonly DbIntegrationConfiguration dbc;

		public BatcherConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		public BatcherConfiguration Through<TBatcher>() where TBatcher : IBatcherFactory
		{
			dbc.Configuration.SetProperty(Environment.BatchStrategy, typeof(TBatcher).AssemblyQualifiedName);
			return this;
		}

		public DbIntegrationConfiguration Each(short batchSize)
		{
			dbc.Configuration.SetProperty(Environment.BatchSize, batchSize.ToString());
			return dbc;
		}

		public BatcherConfiguration OrderingInserts()
		{
			dbc.Configuration.SetProperty(Environment.OrderInserts, true.ToString().ToLowerInvariant());
			return this;
		}

		public BatcherConfiguration DisablingInsertsOrdering()
		{
			dbc.Configuration.SetProperty(Environment.OrderInserts, false.ToString().ToLowerInvariant());
			return this;
		}

		#region Implementation of IBatcherConfiguration
#pragma warning disable 618

		IBatcherConfiguration IBatcherConfiguration.Through<TBatcher>()
		{
			return Through<TBatcher>();
		}

		IDbIntegrationConfiguration IBatcherConfiguration.Each(short batchSize)
		{
			return Each(batchSize);
		}

		IBatcherConfiguration IBatcherConfiguration.OrderingInserts()
		{
			return OrderingInserts();
		}

		IBatcherConfiguration IBatcherConfiguration.DisablingInsertsOrdering()
		{
			return DisablingInsertsOrdering();
		}

#pragma warning restore 618
		#endregion
	}

	public class ConnectionConfiguration 
#pragma warning disable 618
		: IConnectionConfiguration
#pragma warning restore 618
	{
		private readonly DbIntegrationConfiguration dbc;

		public ConnectionConfiguration(DbIntegrationConfiguration dbc)
		{
			this.dbc = dbc;
		}

		public ConnectionConfiguration Through<TProvider>() where TProvider : IConnectionProvider
		{
			dbc.Configuration.SetProperty(Environment.ConnectionProvider, typeof(TProvider).AssemblyQualifiedName);
			return this;
		}

		public ConnectionConfiguration By<TDriver>() where TDriver : IDriver
		{
			dbc.Configuration.SetProperty(Environment.ConnectionDriver, typeof(TDriver).AssemblyQualifiedName);
			return this;
		}

		public ConnectionConfiguration With(IsolationLevel level)
		{
			dbc.Configuration.SetProperty(Environment.Isolation, level.ToString());
			return this;
		}

		public ConnectionConfiguration Releasing(ConnectionReleaseMode releaseMode)
		{
			dbc.Configuration.SetProperty(Environment.ReleaseConnections, ConnectionReleaseModeParser.ToString(releaseMode));
			return this;
		}

		public DbIntegrationConfiguration Using(string connectionString)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionString, connectionString);
			return dbc;
		}

		public DbIntegrationConfiguration Using(DbConnectionStringBuilder connectionStringBuilder)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionString, connectionStringBuilder.ConnectionString);
			return dbc;
		}

		public DbIntegrationConfiguration ByAppConfing(string connectionStringName)
		{
			dbc.Configuration.SetProperty(Environment.ConnectionStringName, connectionStringName);
			return dbc;
		}

		#region Implementation of IConnectionConfiguration
#pragma warning disable 618

		IConnectionConfiguration IConnectionConfiguration.Through<TProvider>()
		{
			return Through<TProvider>();
		}

		IConnectionConfiguration IConnectionConfiguration.By<TDriver>()
		{
			return By<TDriver>();
		}

		IConnectionConfiguration IConnectionConfiguration.With(IsolationLevel level)
		{
			return With(level);
		}

		IConnectionConfiguration IConnectionConfiguration.Releasing(ConnectionReleaseMode releaseMode)
		{
			return Releasing(releaseMode);
		}

		IDbIntegrationConfiguration IConnectionConfiguration.Using(string connectionString)
		{
			return Using(connectionString);
		}

		IDbIntegrationConfiguration IConnectionConfiguration.Using(DbConnectionStringBuilder connectionStringBuilder)
		{
			return Using(connectionStringBuilder);
		}

		IDbIntegrationConfiguration IConnectionConfiguration.ByAppConfing(string connectionStringName)
		{
			return ByAppConfing(connectionStringName);
		}

#pragma warning restore 618
		#endregion
	}
}
