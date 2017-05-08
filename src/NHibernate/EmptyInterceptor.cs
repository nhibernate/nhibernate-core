using System;
using System.Collections;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate
{
	/// <summary>
	/// An interceptor that does nothing.  May be used as a base class for application-defined custom interceptors.
	/// </summary>
	[Serializable]
	public class EmptyInterceptor : IInterceptor
	{
		/// <summary>
		/// The singleton reference.
		/// </summary>
		public static readonly EmptyInterceptor Instance = new EmptyInterceptor();

		public virtual void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
		}

		public virtual void OnCollectionRecreate(object collection, object key)
		{
		}

		public virtual void OnCollectionRemove(object collection, object key)
		{
		}

		public virtual void OnCollectionUpdate(object collection, object key)
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

		public virtual object Instantiate(string clazz, object id)
		{
			return null;
		}

		public virtual string GetEntityName(object entity)
		{
			return null;
		}

		public virtual object GetEntity(string entityName, object id)
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

		public virtual SqlString OnPrepareStatement(SqlString sql)
		{
			return sql;
		}
	}
}