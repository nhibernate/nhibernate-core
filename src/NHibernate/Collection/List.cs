using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection {

	/// <summary>
	/// A persistent wrapper for an IList
	/// </summary>
	public abstract class List {//: ODMGCollection { //, IList {
		private IList list;
/*
		protected override object Snapshot(CollectionPersister persister) {
			ArrayList clonedList = new ArrayList( list.Count );
			foreach(object obj in list) {
				clonedList.Add( persister.ElementType.DeepCopy( obj ) );
			}
			return clonedList;
		}

		public override bool EqualsSnapshot(IType elementType) {
			IList sn = (IList) Snapshot;
			if (sn.Count != this.list.Count) return false;
			for(int i=0; i<list.Count; i++) {
				if ( elementType.IsDirty(list[i], sn[i], session ) ) return false;
			}
			return true;
		}

		public List(ISessionImplementor session) : base(session) {}

		public List(ISessionImplementor session, IList list) : base(session) {
			this.list = list;
			initialized = true;
		}

		public override void BeforeInitialize(CollectionPersister persister) {
			this.list = new ArrayList();
		}

		public int Count {
			get { 
				Read();
				return list.Count;
			}
		}

		public bool Contains(object obj) {
			Read();
			return list.Contains(obj);
		}
*/ 
		//TODO: finish
	}
}
