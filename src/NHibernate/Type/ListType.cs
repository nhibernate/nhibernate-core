using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type {
	
	public class ListType : PersistentCollectionType {
		
		public ListType(string role) : base(role) {
		}

		public override PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister) {
			return new List(session);
		}

		public override System.Type ReturnedClass {
			get { return typeof(IList); }
		}

		public override PersistentCollection Wrap(ISessionImplementor session, object collection) {
			return new List( session, (IList) collection );
		}

		public override PersistentCollection AssembleCachedCollection(
			ISessionImplementor session,
			CollectionPersister persister,
			object disassembled,
			object owner) {
			
			return new List(session, persister, disassembled, owner);
		}
	}
}
