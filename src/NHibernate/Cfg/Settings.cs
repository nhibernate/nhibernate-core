using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using NHibernate.AdoNet;
using NHibernate.AdoNet.Util;
using NHibernate.Cache;
using NHibernate.Connection;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.MultiTenancy;
using NHibernate.Transaction;

namespace NHibernate.Cfg
{
	/// <summary>
	/// Settings that affect the behavior of NHibernate at runtime.
	/// </summary>
	public sealed class Settings
	{
		public Settings()
		{
			MaximumFetchDepth = -1;
		}

		// not ported - private TransactionManagerLookup transactionManagerLookup;
		// not ported - private bool strictJPAQLCompliance;

		#region JDBC Specific (Not Ported)

		//private int jdbcFetchSize;

		#endregion
		public SqlStatementLogger SqlStatementLogger { get; internal set; }

		public int MaximumFetchDepth { get; internal set; }

		public IDictionary<string, string> QuerySubstitutions { get; internal set; }

		public Dialect.Dialect Dialect { get; internal set; }

		public int AdoBatchSize { get; internal set; }

		public int DefaultBatchFetchSize { get; internal set; }

		public bool IsScrollableResultSetsEnabled { get; internal set; }

		public bool IsGetGeneratedKeysEnabled { get; internal set; }

		public string DefaultSchemaName { get; set; }

		public string DefaultCatalogName { get; internal set; }

		public string SessionFactoryName { get; internal set; }

		/// <summary>
		/// Should sessions check on every operation whether there is an ongoing system transaction or not, and enlist
		/// into it if any? Default is <see langword="true"/>. It can also be controlled at session opening, see
		/// <see cref="ISessionFactory.WithOptions" />. A session can also be instructed to explicitly join the current
		/// transaction by calling <see cref="ISession.JoinTransaction" />. This setting has no effect if using a
		/// transaction factory that is not system transactions aware.
		/// </summary>
		public bool AutoJoinTransaction { get; internal set; }

		public bool IsAutoCreateSchema { get; internal set; }

		public bool IsAutoDropSchema { get; internal set; }

		public bool IsAutoUpdateSchema { get; internal set; }

		public bool IsAutoValidateSchema { get; internal set; }

		public bool IsAutoQuoteEnabled { get; internal set; }

		public bool IsKeywordsImportEnabled { get; internal set; }

		public bool IsQueryCacheEnabled { get; internal set; }

		public bool IsStructuredCacheEntriesEnabled { get; internal set; }

		public bool IsSecondLevelCacheEnabled { get; internal set; }

		public string CacheRegionPrefix { get; internal set; }

		public bool IsMinimalPutsEnabled { get; internal set; }

		public bool IsCommentsEnabled { get; internal set; }

		public bool IsStatisticsEnabled { get; internal set; }

		public bool IsIdentifierRollbackEnabled { get; internal set; }

		// Since v5
		[Obsolete("Please use DefaultFlushMode instead.")]
		public bool IsFlushBeforeCompletionEnabled { get; internal set; }

		public bool IsAutoCloseSessionEnabled { get; internal set; }

		public ConnectionReleaseMode ConnectionReleaseMode { get; internal set; }

		public ICacheProvider CacheProvider { get; internal set; }

		public IQueryCacheFactory QueryCacheFactory { get; internal set; }

		public IConnectionProvider ConnectionProvider { get; internal set; }

		public ITransactionFactory TransactionFactory { get; internal set; }

		public IBatcherFactory BatcherFactory { get; internal set; }

		public IQueryTranslatorFactory QueryTranslatorFactory { get; internal set; }

		public System.Type LinqQueryProviderType { get; internal set; }

		public ISQLExceptionConverter SqlExceptionConverter { get; internal set; }

		public bool IsWrapResultSetsEnabled { get; internal set; }

		public bool IsOrderUpdatesEnabled { get; internal set; }

		public bool IsOrderInsertsEnabled { get; internal set; }

		public FlushMode DefaultFlushMode { get; internal set; }

		public bool IsDataDefinitionImplicitCommit { get; internal set; }

		public bool IsDataDefinitionInTransactionSupported { get; internal set; }

		public bool IsNamedQueryStartupCheckingEnabled { get; internal set; }

		public bool IsBatchVersionedDataEnabled { get; internal set; }
		
		// 6.0 TODO : should throw by default
		/// <summary>
		/// <see langword="true" /> to throw in case any failure is reported during schema auto-update,
		/// <see langword="false" /> to ignore failures.
		/// </summary>
		public bool ThrowOnSchemaUpdate { get; internal set; }

		#region NH specific

		public IsolationLevel IsolationLevel { get; internal set; }

		public bool IsOuterJoinFetchEnabled { get; internal set; }
		
		public bool TrackSessionId { get; internal set; }

		/// <summary>
		/// Get the registry to provide Hql-Generators for known properties/methods.
		/// </summary>
		public ILinqToHqlGeneratorsRegistry LinqToHqlGeneratorsRegistry { get; internal set; }

		/// <summary>
		/// Whether to use the legacy pre-evaluation or not in Linq queries. <c>true</c> by default.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Legacy pre-evaluation is causing special properties or functions like <c>DateTime.Now</c> or
		/// <c>Guid.NewGuid()</c> to be always evaluated with the .Net runtime and replaced in the query by
		/// parameter values.
		/// </para>
		/// <para>
		/// The new pre-evaluation allows them to be converted to HQL function calls which will be run on the db
		/// side. This allows for example to retrieve the server time instead of the client time, or to generate
		/// UUIDs for each row instead of an unique one for all rows.
		/// </para>
		/// </remarks>
		public bool LinqToHqlLegacyPreEvaluation { get; internal set; }

		/// <summary>
		/// When the new pre-evaluation is enabled, should methods which translation is not supported by the current
		/// dialect fallback to pre-evaluation? <c>false</c> by default.
		/// </summary>
		/// <remarks>
		/// <para>
		/// When this fallback option is enabled while legacy pre-evaluation is disabled, properties or functions
		/// like <c>DateTime.Now</c> or <c>Guid.NewGuid()</c> used in Linq expressions will not fail when the dialect does not
		/// support them, but will instead be pre-evaluated.
		/// </para>
		/// <para>
		/// When this fallback option is disabled while legacy pre-evaluation is disabled, properties or functions
		/// like <c>DateTime.Now</c> or <c>Guid.NewGuid()</c> used in Linq expressions will fail when the dialect does not
		/// support them.
		/// </para>
		/// <para>
		/// This option has no effect if the legacy pre-evaluation is enabled.
		/// </para>
		/// </remarks>
		public bool LinqToHqlFallbackOnPreEvaluation { get; internal set; }

		public IQueryModelRewriterFactory QueryModelRewriterFactory { get; internal set; }

		/// <summary>
		/// The pre-transformer registrar used to register custom expression transformers.
		/// </summary>
		public IExpressionTransformerRegistrar PreTransformerRegistrar { get; internal set; }

		internal Func<Expression, Expression> LinqPreTransformer { get; set; }

		#endregion

		internal string GetFullCacheRegionName(string name)
		{
			var prefix = CacheRegionPrefix;
			if (!string.IsNullOrEmpty(prefix))
				return prefix + '.' + name;
			return name;
		}

		public MultiTenancyStrategy MultiTenancyStrategy { get; internal set; }

		public IMultiTenancyConnectionProvider MultiTenancyConnectionProvider { get; internal set; }
	}
}
