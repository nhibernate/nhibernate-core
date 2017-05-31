using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
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
	public class DebugSessionFactory : ISessionFactoryImplementor
	{
		public DebugConnectionProvider ConnectionProvider { get; }
		public ISessionFactoryImplementor ActualFactory { get; }

		public EventListeners EventListeners => ((SessionFactoryImpl)ActualFactory).EventListeners;

		private readonly ConcurrentBag<ISessionImplementor> _openedSessions = new ConcurrentBag<ISessionImplementor>();
		private static readonly ILog _log = LogManager.GetLogger(typeof(DebugSessionFactory).Assembly, typeof(TestCase));

		public DebugSessionFactory(ISessionFactory actualFactory)
		{
			ActualFactory = (ISessionFactoryImplementor)actualFactory;
			ConnectionProvider = ActualFactory.ConnectionProvider as DebugConnectionProvider;
		}

		#region Session tracking

		public bool CheckSessionsWereClosed()
		{
			var allClosed = true;
			foreach (var session in _openedSessions)
			{
				if (session.IsOpen)
				{
					if (session.TransactionContext?.ShouldCloseSessionOnDistributedTransactionCompleted ?? false)
					{
						// Delayed transactions not having completed and closed their sessions? Give them a chance to complete.
						Thread.Sleep(100);
						if (!session.IsOpen)
						{
							_log.Warn($"Test case had a delayed close of session {session.SessionId}.");
							continue;
						}
					}

					_log.Error($"Test case didn't close session {session.SessionId}, closing");
					allClosed = false;
					(session as ISession)?.Close();
					(session as IStatelessSession)?.Close();
				}
			}

			return allClosed;
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
			var s = ActualFactory.OpenSession(connection, flushBeforeCompletionEnabled, autoCloseSessionEnabled, connectionReleaseMode);
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

		Dialect.Dialect ISessionFactoryImplementor.Dialect => ActualFactory.Dialect;

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

		IDictionary<string, ICache> ISessionFactoryImplementor.GetAllSecondLevelCacheRegions()
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

		ICache ISessionFactoryImplementor.GetSecondLevelCacheRegion(string regionName)
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
				(ISessionCreationOptions)sessionBuilder;
		}

		public static ISessionCreationOptions GetCreationOptions(IStatelessSessionBuilder sessionBuilder)
		{
			return ((StatelessSessionBuilder)sessionBuilder).CreationOptions;
		}

		internal class SessionBuilder : ISessionBuilder
		{
			private readonly ISessionBuilder _actualBuilder;
			private readonly DebugSessionFactory _debugFactory;

			internal ISessionCreationOptions CreationOptions => (ISessionCreationOptions)_actualBuilder;

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

			ISessionBuilder ISessionBuilder<ISessionBuilder>.FlushMode(FlushMode flushMode)
			{
				_actualBuilder.FlushMode(flushMode);
				return this;
			}

			#endregion
		}

		internal class StatelessSessionBuilder : IStatelessSessionBuilder
		{
			private readonly IStatelessSessionBuilder _actualBuilder;
			private readonly DebugSessionFactory _debugFactory;

			internal ISessionCreationOptions CreationOptions => (ISessionCreationOptions)_actualBuilder;

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

			#endregion
		}
	}
}
