using System;
using System.Data;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type {
	
	public class ArrayType : PersistentCollectionType {
		private System.Type elementClass;
		private System.Type arrayClass;

		public ArrayType(string role, System.Type elementClass) : base(role) {
			this.elementClass = elementClass;
			arrayClass = Array.CreateInstance(elementClass,0).GetType();
		}

		public override System.Type ReturnedClass {
			get { return arrayClass; }
		}

		protected override PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister) {
			return new ArrayHolder(session, persister);
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session) {
			base.NullSafeSet(st, session.GetArrayHolder(value), index, session);
		}

		public ICollection ElementsCollection(object collection) {
			return ((object[])collection);
		}

		public override object Disassemble(object value, ISessionImplementor session) {
			if (value==null) return null;
			return session.GetLoadedCollectionKey( session.GetArrayHolder(value) );
		}

		public override PersistentCollection Wrap(ISessionImplementor session, object array) {
			return new ArrayHolder(session, array);
		}

		public override bool IsArrayType {
			get { return true; }
		}

		public override PersistentCollection AssembleCachedCollection(	ISessionImplementor session,
			CollectionPersister persister,
			object disassembled,
			object owner ) {
			return new ArrayHolder(session, persister, disassembled, owner);
		}
	}
}
