using System;
using System.Collections;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate
{
	[Serializable]
	public class EmptyInterceptor : IInterceptor
	{
		public virtual void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
		}

		public void OnCollectionRecreate(object collection, object key)
		{
		}

		public void OnCollectionRemove(object collection, object key)
		{
		}

		public void OnCollectionUpdate(object collection, object key)
		{
		}

		public virtual bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
		                                 string[] propertyNames, IType[] types)
		{
			return false;
		}

		public virtual bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			return false;
		}

		public virtual bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			return false;
		}

		public virtual void PostFlush(ICollection entities)
		{
		}

		public virtual void PreFlush(ICollection entitites)
		{
		}

		public virtual bool? IsTransient(object entity)
		{
			return null;
		}

		public virtual object Instantiate(string clazz, EntityMode entityMode, object id)
		{
			return null;
		}

		public string GetEntityName(object entity)
		{
			return null;
		}

		public object GetEntity(string entityName, object id)
		{
			return null;
		}

		public virtual int[] FindDirty(object entity, object id, object[] currentState, object[] previousState,
		                               string[] propertyNames, IType[] types)
		{
			return null;
		}

		public virtual void AfterTransactionBegin(ITransaction tx)
		{
		}

		public virtual void BeforeTransactionCompletion(ITransaction tx)
		{
		}

		public virtual void AfterTransactionCompletion(ITransaction tx)
		{
		}

		public virtual void SetSession(ISession session)
		{
		}

		public SqlString OnPrepareStatement(SqlString sql)
		{
			return sql;
		}
	}
}