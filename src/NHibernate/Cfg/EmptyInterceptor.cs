using System;
using System.Collections;
using NHibernate.Type;

namespace NHibernate.Cfg
{
	[Serializable]
	public class EmptyInterceptor : IInterceptor
	{
		public virtual void OnDelete( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
		{
		}

		public virtual bool OnFlushDirty( object entity, object id, object[ ] currentState, object[ ] previousState, string[ ] propertyNames, IType[ ] types )
		{
			return false;
		}

		public virtual bool OnLoad( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
		{
			return false;
		}

		public virtual bool OnSave( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
		{
			return false;
		}

		public virtual void PostFlush( ICollection entities )
		{
		}

		public virtual void PreFlush( ICollection entitites )
		{
		}

		public virtual object IsUnsaved( object entity )
		{
			return null;
		}

		public virtual object Instantiate( System.Type clazz, object id )
		{
			return null;
		}

		public virtual int[] FindDirty( object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types )
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
	}

}
