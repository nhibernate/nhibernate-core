using System;
using System.Collections;
using NHibernate.Type;

namespace NHibernate.Cfg
{
	[Serializable]
	internal class EmptyInterceptor : IInterceptor
	{

		public EmptyInterceptor()
		{	
		}

		public void OnDelete( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
		{
		}

		public bool OnFlushDirty( object entity, object id, object[ ] currentState, object[ ] previousState, string[ ] propertyNames, IType[ ] types )
		{
			return false;
		}

		public bool OnLoad( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
		{
			return false;
		}

		public bool OnSave( object entity, object id, object[ ] state, string[ ] propertyNames, IType[ ] types )
		{
			return false;
		}

		public void OnPostFlush( object entity, object id, object[ ] currentState, string[ ] propertyNames, IType[ ] types )
		{
		}

		public void PostFlush( ICollection entities )
		{
		}

		public void PreFlush( ICollection entitites )
		{
		}

		public object IsUnsaved( object entity )
		{
			return null;
		}

		public object Instantiate( System.Type clazz, object id )
		{
			return null;
		}

		public int[ ] FindDirty( object entity, object id, object[ ] currentState, object[ ] previousState, string[ ] propertyNames, IType[ ] types )
		{
			return null;
		}
	}

}
