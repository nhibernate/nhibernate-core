
using System;
using System.Collections.Generic;

using NHibernate.Impl;

namespace NHibernate.Test.Criteria.Lambda
{

	public class SessionStub : ISession
	{

		ICriteria ISession.CreateCriteria(System.Type persistentClass)
		{
			return new CriteriaImpl(persistentClass, null);
		}

		ICriteria ISession.CreateCriteria(System.Type persistentClass, string alias)
		{
			return new CriteriaImpl(persistentClass, alias, null);
		}

		#region non stubbed methods 

		EntityMode ISession.ActiveEntityMode
		{
			get { throw new NotImplementedException(); }
		}

		void ISession.Flush()
		{
			throw new NotImplementedException();
		}

		FlushMode ISession.FlushMode
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		CacheMode ISession.CacheMode
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		ISessionFactory ISession.SessionFactory
		{
			get { throw new NotImplementedException(); }
		}

		System.Data.IDbConnection ISession.Connection
		{
			get { throw new NotImplementedException(); }
		}

		System.Data.IDbConnection ISession.Disconnect()
		{
			throw new NotImplementedException();
		}

		void ISession.Reconnect()
		{
			throw new NotImplementedException();
		}

		void ISession.Reconnect(System.Data.IDbConnection connection)
		{
			throw new NotImplementedException();
		}

		System.Data.IDbConnection ISession.Close()
		{
			throw new NotImplementedException();
		}

		void ISession.CancelQuery()
		{
			throw new NotImplementedException();
		}

		bool ISession.IsOpen
		{
			get { throw new NotImplementedException(); }
		}

		bool ISession.IsConnected
		{
			get { throw new NotImplementedException(); }
		}

		bool ISession.IsDirty()
		{
			throw new NotImplementedException();
		}

		object ISession.GetIdentifier(object obj)
		{
			throw new NotImplementedException();
		}

		bool ISession.Contains(object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.Evict(object obj)
		{
			throw new NotImplementedException();
		}

		object ISession.Load(System.Type theType, object id, LockMode lockMode)
		{
			throw new NotImplementedException();
		}

		object ISession.Load(string entityName, object id, LockMode lockMode)
		{
			throw new NotImplementedException();
		}

		object ISession.Load(System.Type theType, object id)
		{
			throw new NotImplementedException();
		}

		T ISession.Load<T>(object id, LockMode lockMode)
		{
			throw new NotImplementedException();
		}

		T ISession.Load<T>(object id)
		{
			throw new NotImplementedException();
		}

		object ISession.Load(string entityName, object id)
		{
			throw new NotImplementedException();
		}

		void ISession.Load(object obj, object id)
		{
			throw new NotImplementedException();
		}

		void ISession.Replicate(object obj, ReplicationMode replicationMode)
		{
			throw new NotImplementedException();
		}

		void ISession.Replicate(string entityName, object obj, ReplicationMode replicationMode)
		{
			throw new NotImplementedException();
		}

		object ISession.Save(object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.Save(object obj, object id)
		{
			throw new NotImplementedException();
		}

		object ISession.Save(string entityName, object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.SaveOrUpdate(object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.SaveOrUpdate(string entityName, object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.Update(object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.Update(object obj, object id)
		{
			throw new NotImplementedException();
		}

		void ISession.Update(string entityName, object obj)
		{
			throw new NotImplementedException();
		}

		object ISession.Merge(object obj)
		{
			throw new NotImplementedException();
		}

		object ISession.Merge(string entityName, object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.Persist(object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.Persist(string entityName, object obj)
		{
			throw new NotImplementedException();
		}

		object ISession.SaveOrUpdateCopy(object obj)
		{
			throw new NotImplementedException();
		}

		object ISession.SaveOrUpdateCopy(object obj, object id)
		{
			throw new NotImplementedException();
		}

		void ISession.Delete(object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.Delete(string entityName, object obj)
		{
			throw new NotImplementedException();
		}

		System.Collections.IList ISession.Find(string query)
		{
			throw new NotImplementedException();
		}

		System.Collections.IList ISession.Find(string query, object value, NHibernate.Type.IType type)
		{
			throw new NotImplementedException();
		}

		System.Collections.IList ISession.Find(string query, object[] values, NHibernate.Type.IType[] types)
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerable ISession.Enumerable(string query)
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerable ISession.Enumerable(string query, object value, NHibernate.Type.IType type)
		{
			throw new NotImplementedException();
		}

		System.Collections.IEnumerable ISession.Enumerable(string query, object[] values, NHibernate.Type.IType[] types)
		{
			throw new NotImplementedException();
		}

		System.Collections.ICollection ISession.Filter(object collection, string filter)
		{
			throw new NotImplementedException();
		}

		System.Collections.ICollection ISession.Filter(object collection, string filter, object value, NHibernate.Type.IType type)
		{
			throw new NotImplementedException();
		}

		System.Collections.ICollection ISession.Filter(object collection, string filter, object[] values, NHibernate.Type.IType[] types)
		{
			throw new NotImplementedException();
		}

		int ISession.Delete(string query)
		{
			throw new NotImplementedException();
		}

		int ISession.Delete(string query, object value, NHibernate.Type.IType type)
		{
			throw new NotImplementedException();
		}

		int ISession.Delete(string query, object[] values, NHibernate.Type.IType[] types)
		{
			throw new NotImplementedException();
		}

		void ISession.Lock(object obj, LockMode lockMode)
		{
			throw new NotImplementedException();
		}

		void ISession.Lock(string entityName, object obj, LockMode lockMode)
		{
			throw new NotImplementedException();
		}

		void ISession.Refresh(object obj)
		{
			throw new NotImplementedException();
		}

		void ISession.Refresh(object obj, LockMode lockMode)
		{
			throw new NotImplementedException();
		}

		LockMode ISession.GetCurrentLockMode(object obj)
		{
			throw new NotImplementedException();
		}

		ITransaction ISession.BeginTransaction()
		{
			throw new NotImplementedException();
		}

		ITransaction ISession.BeginTransaction(System.Data.IsolationLevel isolationLevel)
		{
			throw new NotImplementedException();
		}

		ITransaction ISession.Transaction
		{
			get { throw new NotImplementedException(); }
		}

		ICriteria ISession.CreateCriteria<T>()
		{
			throw new NotImplementedException();
		}

		ICriteria ISession.CreateCriteria<T>(string alias)
		{
			throw new NotImplementedException();
		}

		ICriteria ISession.CreateCriteria(string entityName)
		{
			throw new NotImplementedException();
		}

		ICriteria ISession.CreateCriteria(string entityName, string alias)
		{
			throw new NotImplementedException();
		}

		IQuery ISession.CreateQuery(string queryString)
		{
			throw new NotImplementedException();
		}

		IQuery ISession.CreateFilter(object collection, string queryString)
		{
			throw new NotImplementedException();
		}

		IQuery ISession.GetNamedQuery(string queryName)
		{
			throw new NotImplementedException();
		}

		IQuery ISession.CreateSQLQuery(string sql, string returnAlias, System.Type returnClass)
		{
			throw new NotImplementedException();
		}

		IQuery ISession.CreateSQLQuery(string sql, string[] returnAliases, System.Type[] returnClasses)
		{
			throw new NotImplementedException();
		}

		ISQLQuery ISession.CreateSQLQuery(string queryString)
		{
			throw new NotImplementedException();
		}

		void ISession.Clear()
		{
			throw new NotImplementedException();
		}

		object ISession.Get(System.Type clazz, object id)
		{
			throw new NotImplementedException();
		}

		object ISession.Get(System.Type clazz, object id, LockMode lockMode)
		{
			throw new NotImplementedException();
		}

		object ISession.Get(string entityName, object id)
		{
			throw new NotImplementedException();
		}

		T ISession.Get<T>(object id)
		{
			throw new NotImplementedException();
		}

		T ISession.Get<T>(object id, LockMode lockMode)
		{
			throw new NotImplementedException();
		}

		string ISession.GetEntityName(object obj)
		{
			throw new NotImplementedException();
		}

		IFilter ISession.EnableFilter(string filterName)
		{
			throw new NotImplementedException();
		}

		IFilter ISession.GetEnabledFilter(string filterName)
		{
			throw new NotImplementedException();
		}

		void ISession.DisableFilter(string filterName)
		{
			throw new NotImplementedException();
		}

		IMultiQuery ISession.CreateMultiQuery()
		{
			throw new NotImplementedException();
		}

		ISession ISession.SetBatchSize(int batchSize)
		{
			throw new NotImplementedException();
		}

		NHibernate.Engine.ISessionImplementor ISession.GetSessionImplementation()
		{
			throw new NotImplementedException();
		}

		IMultiCriteria ISession.CreateMultiCriteria()
		{
			throw new NotImplementedException();
		}

		NHibernate.Stat.ISessionStatistics ISession.Statistics
		{
			get { throw new NotImplementedException(); }
		}

		ISession ISession.GetSession(EntityMode entityMode)
		{
			throw new NotImplementedException();
		}

		void IDisposable.Dispose()
		{
			throw new NotImplementedException();
		}

		#endregion

	}

}

