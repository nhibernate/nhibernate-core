using System;
using System.Data;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type {
	
	public class MapType : PersistentCollectionType {
		
		public MapType(string role) : base(role) {
		}

		protected override PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister) {
			return new Map(session);
		}

		public override System.Type ReturnedClass {
			get { return typeof(IDictionary); }
		}

		public ICollection ElementsCollection(object collection) {
			return ((IDictionary) collection).Values;
		}

		public override PersistentCollection Wrap(ISessionImplementor session, object collection) {
			return new Map( session, (IDictionary) collection );
		}

		public override PersistentCollection AssembleCachedCollection(
			ISessionImplementor session,
			CollectionPersister persister,
			object disassembled,
			object owner) {

			return new Map(session, persister, disassembled, owner);
		}
	}
}
