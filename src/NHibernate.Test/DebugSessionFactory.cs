using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using log4net;
using NHibernate.Cache;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Context;
using NHibernate.Dialect.Function;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Id;
using NHibernate.Impl;
using NHibernate.Metadata;
using NHibernate.MultiTenancy;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Stat;
using NHibernate.Transaction;
using NHibernate.Type;

namespace NHibernate.Test
{
	/// <summary>
	/// This session factory keeps a list of all opened sessions,
	/// it is used when testing to check that tests clean up after themselves.
	/// </summary>
	/// <remarks>Sessions opened from other sessions are not tracked.</remarks>
	[Serializable]
	public partial class DebugSessionFactory : ISessionFactoryImplementor
	{
		[NonSerialized]
		private DebugConnectionProvider _debugConnectionProvider;

		/// <summary>
		/// The debug connection provider if configured for using it, <see langword="null"/> otherwise.
		/// Use <c>ActualFactory.ConnectionProvider</c> if needing unconditionally the connection provider, be
		/// it debug or not.
		/// </summary>
		public DebugConnectionProvider DebugConnectionProvider
			=> _debugConnectionProvider ??
				(_debugConnectionProvider = ActualFactory.ConnectionProvider as DebugConnectionProvider);
		public ISessionFactoryImplementor ActualFactory { get; }

		public EventListeners EventListeners => ((SessionFactoryImpl) ActualFactory).EventListeners;

		[NonSerialized]
		private readonly ConcurrentBag<ISessionImplementor> _openedSessions = new ConcurrentBag<ISessionImplementor>();
		private static readonly ILog _log = LogManager.GetLogger(typeof(DebugSessionFactory).Assembly, typeof(TestCase));

		public DebugSessionFactory(ISessionFactory actualFactory)
		{
			ActualFactory = (ISessionFactoryImplementor) actualFactory;
		}

		#region Session tracking

		public bool CheckSessionsWereClosed()
		{
			var allClosed = true;
			foreach (var session in _openedSessions)
			{
				// Do not inverse, we want to close all of them.
				allClosed = CheckSessionWasClosed(session) && allClosed;
				// Catches only session opened from another one while sharing the connection. Those
				// opened without sharing the connection stay un-monitored.
				foreach (var dependentSession in session.ConnectionManager.DependentSessions.ToList())
				{
					allClosed = CheckSessionWasClosed(dependentSession) && allClosed;
				}
			}

			return allClosed;
		}

		private bool CheckSessionWasClosed(ISessionImplementor session)
		{
			session.TransactionContext?.Wait();

			if (!session.IsOpen)
				return true;

			_log.Error($"Test case didn't close session {session.SessionId}, closing");
			(session as ISession)?.Close();
			(session as IStatelessSession)?.Close();
			return false;
		}

		ISessionBuilder ISessionFactory.WithOptions()
		{
			return new SessionBuilder(ActualFactory.WithOptions(), this);
		}

		ISession ISessionFactory.OpenSession(DbConnection connection)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			var s = ActualFactory.OpenSession(connection);
#pragma warning restore CS0618 // Type or member is obsolete
			_openedSessions.Add(s.GetSessionImplementation());
			return s;
		}

		ISession ISessionFactory.OpenSession(IInterceptor sessionLocalInterceptor)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			var s = ActualFactory.OpenSession(sessionLocalInterceptor);
#pragma warning restore CS0618 // Type or member is obsolete
			_openedSessions.Add(s.GetSessionImplementation());
			return s;
		}

		ISession ISessionFactory.OpenSession(DbConnection conn, IInterceptor sessionLocalInterceptor)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			var s = ActualFactory.OpenSession(conn, sessionLocalInterceptor);
#pragma warning restore CS0618 // Type or member is obsolete
			_openedSessions.Add(s.GetSessionImplementation());
			return s;
		}

		ISession ISessionFactory.OpenSession()
		{
			var s = ActualFactory.OpenSession();
			_openedSessions.Add(s.GetSessionImplementation());
			return s;
		}

		IStatelessSessionBuilder ISessionFactory.WithStatelessOptions()
		{
			return new StatelessSessionBuilder(ActualFactory.WithStatelessOptions(), this);
		}

		IStatelessSession ISessionFactory.OpenStatelessSession()
		{
			var s = ActualFactory.OpenStatelessSession();
			_openedSessions.Add(s.GetSessionImplementation());
			return s;
		}

		IStatelessSession ISessionFactory.OpenStatelessSession(DbConnection connection)
		{
			var s = ActualFactory.OpenStatelessSession(connection);
			_openedSessions.Add(s.GetSessionImplementation());
			return s;
		}

		ISession ISessionFactoryImplementor.OpenSession(
			DbConnection connection,
			bool flushBeforeCompletionEnabled,
			bool autoCloseSessionEnabled,
			ConnectionReleaseMode connectionReleaseMode)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			var s = ActualFactory.OpenSession(connection, flushBeforeCompletionEnabled, autoCloseSessionEnabled, connectionReleaseMode);
#pragma warning restore CS0618 // Type or member is obsolete
			_openedSessions.Add(s.GetSessionImplementation());
			return s;
		}

		#endregion

		#region Delegated calls without any changes

		IType IMapping.GetIdentifierType(string className)
		{
			return ActualFactory.GetIdentifierType(className);
		}

		string IMapping.GetIdentifierPropertyName(string className)
		{
			return ActualFactory.GetIdentifierPropertyName(className);
		}

		IType IMapping.GetReferencedPropertyType(string className, string propertyName)
		{
			return ActualFactory.GetReferencedPropertyType(className, propertyName);
		}

		bool IMapping.HasNonIdentifierPropertyNamedId(string className)
		{
			return ActualFactory.HasNonIdentifierPropertyNamedId(className);
		}

		Dialect.Dialect IMapping.Dialect => ActualFactory.Dialect;

		void IDisposable.Dispose()
		{
			ActualFactory.Dispose();
		}

		IClassMetadata ISessionFactory.GetClassMetadata(System.Type persistentClass)
		{
			return ActualFactory.GetClassMetadata(persistentClass);
		}

		IClassMetadata ISessionFactory.GetClassMetadata(string entityName)
		{
			return ActualFactory.GetClassMetadata(entityName);
		}

		ICollectionMetadata ISessionFactory.GetCollectionMetadata(string roleName)
		{
			return ActualFactory.GetCollectionMetadata(roleName);
		}

		IDictionary<string, IClassMetadata> ISessionFactory.GetAllClassMetadata()
		{
			return ActualFactory.GetAllClassMetadata();
		}

		IDictionary<string, ICollectionMetadata> ISessionFactory.GetAllCollectionMetadata()
		{
			return ActualFactory.GetAllCollectionMetadata();
		}

		void ISessionFactory.Close()
		{
			ActualFactory.Close();
		}

		void ISessionFactory.Evict(System.Type persistentClass)
		{
			ActualFactory.Evict(persistentClass);
		}

		void ISessionFactory.Evict(System.Type persistentClass, object id)
		{
			ActualFactory.Evict(persistentClass, id);
		}

		void ISessionFactory.EvictEntity(string entityName)
		{
			ActualFactory.EvictEntity(entityName);
		}

		void ISessionFactory.EvictEntity(string entityName, object id)
		{
			ActualFactory.EvictEntity(entityName, id);
		}

		void ISessionFactory.EvictCollection(string roleName)
		{
			ActualFactory.EvictCollection(roleName);
		}

		void ISessionFactory.EvictCollection(string roleName, object id)
		{
			ActualFactory.EvictCollection(roleName, id);
		}

		void ISessionFactory.EvictQueries()
		{
			ActualFactory.EvictQueries();
		}

		void ISessionFactory.EvictQueries(string cacheRegion)
		{
			ActualFactory.EvictQueries(cacheRegion);
		}

		FilterDefinition ISessionFactory.GetFilterDefinition(string filterName)
		{
			return ActualFactory.GetFilterDefinition(filterName);
		}

		ISession ISessionFactory.GetCurrentSession()
		{
			return ActualFactory.GetCurrentSession();
		}

		IStatistics ISessionFactory.Statistics => ActualFactory.Statistics;

		bool ISessionFactory.IsClosed => ActualFactory.IsClosed;

		ICollection<string> ISessionFactory.DefinedFilterNames => ActualFactory.DefinedFilterNames;

		IInterceptor ISessionFactoryImplementor.Interceptor => ActualFactory.Interceptor;

		QueryPlanCache ISessionFactoryImplementor.QueryPlanCache => ActualFactory.QueryPlanCache;

		IConnectionProvider ISessionFactoryImplementor.ConnectionProvider => ActualFactory.ConnectionProvider;

		ITransactionFactory ISessionFactoryImplementor.TransactionFactory => ActualFactory.TransactionFactory;

		UpdateTimestampsCache ISessionFactoryImplementor.UpdateTimestampsCache => ActualFactory.UpdateTimestampsCache;

		IStatisticsImplementor ISessionFactoryImplementor.StatisticsImplementor => ActualFactory.StatisticsImplementor;

		ISQLExceptionConverter ISessionFactoryImplementor.SQLExceptionConverter => ActualFactory.SQLExceptionConverter;

		Settings ISessionFactoryImplementor.Settings => ActualFactory.Settings;

		IEntityNotFoundDelegate ISessionFactoryImplementor.EntityNotFoundDelegate => ActualFactory.EntityNotFoundDelegate;

		SQLFunctionRegistry ISessionFactoryImplementor.SQLFunctionRegistry => ActualFactory.SQLFunctionRegistry;

#pragma warning disable 618
		IDictionary<string, ICache> ISessionFactoryImplementor.GetAllSecondLevelCacheRegions()
#pragma warning restore 618
		{
			return ActualFactory.GetAllSecondLevelCacheRegions();
		}

		IEntityPersister ISessionFactoryImplementor.GetEntityPersister(string entityName)
		{
			return ActualFactory.GetEntityPersister(entityName);
		}

		ICollectionPersister ISessionFactoryImplementor.GetCollectionPersister(string role)
		{
			return ActualFactory.GetCollectionPersister(role);
		}

		public ISet<IEntityPersister> GetEntityPersisters(ISet<string> spaces)
		{
			return ActualFactory.GetEntityPersisters(spaces);
		}

		public ISet<ICollectionPersister> GetCollectionPersisters(ISet<string> spaces)
		{
			return ActualFactory.GetCollectionPersisters(spaces);
		}

		IType[] ISessionFactoryImplementor.GetReturnTypes(string queryString)
		{
			return ActualFactory.GetReturnTypes(queryString);
		}

		string[] ISessionFactoryImplementor.GetReturnAliases(string queryString)
		{
			return ActualFactory.GetReturnAliases(queryString);
		}

		string[] ISessionFactoryImplementor.GetImplementors(string entityOrClassName)
		{
			return ActualFactory.GetImplementors(entityOrClassName);
		}

		string ISessionFactoryImplementor.GetImportedClassName(string name)
		{
			return ActualFactory.GetImportedClassName(name);
		}

		IQueryCache ISessionFactoryImplementor.QueryCache => ActualFactory.QueryCache;

		IQueryCache ISessionFactoryImplementor.GetQueryCache(string regionName)
		{
			return ActualFactory.GetQueryCache(regionName);
		}

		NamedQueryDefinition ISessionFactoryImplementor.GetNamedQuery(string queryName)
		{
			return ActualFactory.GetNamedQuery(queryName);
		}

		NamedSQLQueryDefinition ISessionFactoryImplementor.GetNamedSQLQuery(string queryName)
		{
			return ActualFactory.GetNamedSQLQuery(queryName);
		}

		ResultSetMappingDefinition ISessionFactoryImplementor.GetResultSetMapping(string resultSetRef)
		{
			return ActualFactory.GetResultSetMapping(resultSetRef);
		}

		IIdentifierGenerator ISessionFactoryImplementor.GetIdentifierGenerator(string rootEntityName)
		{
			return ActualFactory.GetIdentifierGenerator(rootEntityName);
		}

#pragma warning disable 618
		ICache ISessionFactoryImplementor.GetSecondLevelCacheRegion(string regionName)
#pragma warning restore 618
		{
			return ActualFactory.GetSecondLevelCacheRegion(regionName);
		}

		ISet<string> ISessionFactoryImplementor.GetCollectionRolesByEntityParticipant(string entityName)
		{
			return ActualFactory.GetCollectionRolesByEntityParticipant(entityName);
		}

		ICurrentSessionContext ISessionFactoryImplementor.CurrentSessionContext => ActualFactory.CurrentSessionContext;

		IEntityPersister ISessionFactoryImplementor.TryGetEntityPersister(string entityName)
		{
			return ActualFactory.TryGetEntityPersister(entityName);
		}

		string ISessionFactoryImplementor.TryGetGuessEntityName(System.Type implementor)
		{
			return ActualFactory.TryGetGuessEntityName(implementor);
		}

		#endregion

		public static ISessionCreationOptions GetCreationOptions<T>(ISessionBuilder<T> sessionBuilder) where T : ISessionBuilder<T>
		{
			return (sessionBuilder as SessionBuilder)?.CreationOptions ??
				(ISessionCreationOptions) sessionBuilder;
		}

		public static ISessionCreationOptions GetCreationOptions(IStatelessSessionBuilder sessionBuilder)
		{
			return (sessionBuilder as StatelessSessionBuilder)?.CreationOptions ??
				(ISessionCreationOptions) sessionBuilder;
		}

		internal class SessionBuilder : ISessionBuilder,
			//TODO 6.0: Remove interface with implementation (will be replaced TenantConfiguration ISessionBuilder method)
			ISessionCreationOptionsWithMultiTenancy
		{
			private readonly ISessionBuilder _actualBuilder;
			private readonly DebugSessionFactory _debugFactory;

			internal ISessionCreationOptions CreationOptions => (ISessionCreationOptions) _actualBuilder;

			public SessionBuilder(ISessionBuilder actualBuilder, DebugSessionFactory debugFactory)
			{
				_actualBuilder = actualBuilder;
				_debugFactory = debugFactory;
			}

			ISession ISessionBuilder<ISessionBuilder>.OpenSession()
			{
				var s = _actualBuilder.OpenSession();
				_debugFactory._openedSessions.Add(s.GetSessionImplementation());
				return s;
			}

			#region Delegated calls without any changes

			ISessionBuilder ISessionBuilder<ISessionBuilder>.Interceptor(IInterceptor interceptor)
			{
				_actualBuilder.Interceptor(interceptor);
				return this;
			}

			ISessionBuilder ISessionBuilder<ISessionBuilder>.NoInterceptor()
			{
				_actualBuilder.NoInterceptor();
				return this;
			}

			ISessionBuilder ISessionBuilder<ISessionBuilder>.Connection(DbConnection connection)
			{
				_actualBuilder.Connection(connection);
				return this;
			}

			ISessionBuilder ISessionBuilder<ISessionBuilder>.ConnectionReleaseMode(ConnectionReleaseMode connectionReleaseMode)
			{
				_actualBuilder.ConnectionReleaseMode(connectionReleaseMode);
				return this;
			}

			ISessionBuilder ISessionBuilder<ISessionBuilder>.AutoClose(bool autoClose)
			{
				_actualBuilder.AutoClose(autoClose);
				return this;
			}

			ISessionBuilder ISessionBuilder<ISessionBuilder>.AutoJoinTransaction(bool autoJoinTransaction)
			{
				_actualBuilder.AutoJoinTransaction(autoJoinTransaction);
				return this;
			}

			ISessionBuilder ISessionBuilder<ISessionBuilder>.FlushMode(FlushMode flushMode)
			{
				_actualBuilder.FlushMode(flushMode);
				return this;
			}

			#endregion

			TenantConfiguration ISessionCreationOptionsWithMultiTenancy.TenantConfiguration
			{
				get => (_actualBuilder as ISessionCreationOptionsWithMultiTenancy)?.TenantConfiguration;
				set => _actualBuilder.Tenant(value);
			}
		}

		internal class StatelessSessionBuilder : IStatelessSessionBuilder,
			//TODO 6.0: Remove interface with implementation (will be replaced TenantConfiguration IStatelessSessionBuilder method)
			ISessionCreationOptionsWithMultiTenancy
		{
			private readonly IStatelessSessionBuilder _actualBuilder;
			private readonly DebugSessionFactory _debugFactory;

			internal ISessionCreationOptions CreationOptions => (ISessionCreationOptions) _actualBuilder;

			public StatelessSessionBuilder(IStatelessSessionBuilder actualBuilder, DebugSessionFactory debugFactory)
			{
				_actualBuilder = actualBuilder;
				_debugFactory = debugFactory;
			}

			IStatelessSession IStatelessSessionBuilder.OpenStatelessSession()
			{
				var s = _actualBuilder.OpenStatelessSession();
				_debugFactory._openedSessions.Add(s.GetSessionImplementation());
				return s;
			}

			#region Delegated calls without any changes

			IStatelessSessionBuilder IStatelessSessionBuilder.Connection(DbConnection connection)
			{
				_actualBuilder.Connection(connection);
				return this;
			}

			IStatelessSessionBuilder IStatelessSessionBuilder.AutoJoinTransaction(bool autoJoinTransaction)
			{
				_actualBuilder.AutoJoinTransaction(autoJoinTransaction);
				return this;
			}

			TenantConfiguration ISessionCreationOptionsWithMultiTenancy.TenantConfiguration
			{
				get => (_actualBuilder as ISessionCreationOptionsWithMultiTenancy)?.TenantConfiguration;
				set => _actualBuilder.Tenant(value);
			}

			#endregion
		}
	}
}
