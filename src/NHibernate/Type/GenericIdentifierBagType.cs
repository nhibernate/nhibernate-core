using System;
using System.Collections;
using System.Collections.Generic;

using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;

namespace NHibernate.Type
{
	public class GenericIdentifierBagType<T> : IdentifierBagType
	{
		public GenericIdentifierBagType( string role, string propertyRef )
			: base( role, propertyRef )
		{
		}

		public override IPersistentCollection Instantiate( ISessionImplementor session, ICollectionPersister persister )
		{
			return new PersistentIdentifierBag<T>( session );
		}

		public override IPersistentCollection Wrap( ISessionImplementor session, object collection )
		{
			return new PersistentIdentifierBag<T>( session, ( ICollection ) collection );
		}

		public override System.Type ReturnedClass
		{
			get { return typeof( IList<T> ); }
		}
	}
}
