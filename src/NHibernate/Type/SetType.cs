using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type {
	
	/// <summary>
	/// Summary description for SetType.
	/// </summary>
	public class SetType : PersistentCollectionType {

		public SetType(string role) : base(role){
		}

		/// <summary>
		/// <see cref="PersistentCollectionType.Instantiate"/>
		/// </summary>
		public override PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister) {
			return new Set(session);
		}

		/// <summary>
		/// <see cref="AbstractType.ReturnedClass"/>
		/// </summary>
		public override System.Type ReturnedClass {
			get {return typeof(IDictionary);}
		}

		/// <summary>
		/// <see cref="PersistentCollectionType.Wrap"/>
		/// </summary>
		public override PersistentCollection Wrap(ISessionImplementor session, object collection) {
			return new Set(session, (IDictionary)collection);
		}

		/// <summary>
		/// <see cref="PersistentCollectionType.AssembleCachedCollection"/>
		/// </summary>
		public override PersistentCollection AssembleCachedCollection(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) {
			return new Set(session, persister, disassembled, owner);
		}


	}
}
