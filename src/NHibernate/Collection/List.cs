using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;


namespace NHibernate.Collection 
{
	/// <summary>
	/// A persistent wrapper for an IList
	/// </summary>
	[Serializable]
	public class List : ODMGCollection , IList 
	{
		private IList list;

		// used to hold the Identifiers of the Elements that will later
		// be moved to the list field.
		private IList listIdentifiers;

		protected override object Snapshot(CollectionPersister persister) {
			ArrayList clonedList = new ArrayList( list.Count );
			foreach(object obj in list) {
				clonedList.Add( persister.ElementType.DeepCopy( obj ) );
			}
			return clonedList;
		}

		public override ICollection GetOrphans(object snapshot)
		{
			IList sn = (IList)snapshot;
			ArrayList result = new ArrayList(sn.Count);
			result.AddRange(sn);
			PersistentCollection.IdentityRemoveAll(result, list, session);
			return result;
		}

		public override bool EqualsSnapshot(IType elementType) 
		{
			IList sn = (IList) GetSnapshot();
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
			directlyAccessible = true;
		}

		public override void BeforeInitialize(CollectionPersister persister) {
			this.list = new ArrayList();
			this.listIdentifiers = new ArrayList();
		}

		public override int Count {
			get { 
				Read();
				return list.Count;
			}
		}

		public bool IsEmpty {
			get { 
				Read();
				return list.Count == 0;
			}
		}

		public override void CopyTo(System.Array array, int index) {
			Read();
			list.CopyTo(array, index);
		}
		public override object SyncRoot {
			get { return this; }
		}
		public override bool IsSynchronized {
			get { return false; }
		}
		public bool IsFixedSize {
			get { return false; }
		}
		public bool IsReadOnly {
			get { return false; }
		}

		public bool Contains(object obj) {
			Read();
			return list.Contains(obj);
		}

		public override IEnumerator GetEnumerator() {
			Read();
			return list.GetEnumerator();
		}

		public override void DelayedAddAll(ICollection coll)
		{
			foreach(object obj in coll) 
			{
				list.Add(obj);
			}
		}


		public int Add(object obj) 
		{
			if ( !QueueAdd(obj) ) {
				Write();
				return list.Add(obj);
			} else {
				return -1;
			}
		}
		public void Insert(int index, object obj) {
			Write();
			list.Insert(index, obj);
		}	

		public void Remove(object obj) {
			Write();
			list.Remove(obj);
		}

		public void Clear() {
			Write();
			list.Clear();
		}

		public object this [ int index ] {
			get {
				Read();
				return list[index];
			}
			set {
				Write();
				list[index] = value;
			}
		}

		public void RemoveAt(int index) {
			Write();
			list.RemoveAt(index);
		}

		public int IndexOf(object obj) {
			Read();
			return list.IndexOf(obj);
		}

		public override void EndRead(CollectionPersister persister, object owner) 
		{
			for(int i = 0 ;i < listIdentifiers.Count; i++) 
			{
				object element = persister.ElementType.ResolveIdentifier(listIdentifiers[i], session, owner);
				list[i] = element;
			}
			
			if( Additions!=null ) 
			{
				DelayedAddAll( Additions );
				Additions = null;
			}
		}

		public override ICollection Elements() {
			return list;
		}

		public override bool Empty {
			get { return list.Count == 0; }
		}

		public override string ToString() {
			Read();
			return list.ToString();
		}

		public override void WriteTo(IDbCommand st, CollectionPersister persister, object entry, int i, bool writeOrder) {
			persister.WriteElement(st, entry, writeOrder, session);
			persister.WriteIndex(st, i, writeOrder, session);
		}

		public override object ReadFrom(IDataReader rs, CollectionPersister persister, object owner) {
			//object element = persister.ReadElement(rs, owner, session);
			object elementIdentifier = persister.ReadElementIdentifier(rs, owner, session);
			int index = (int) persister.ReadIndex(rs, session);
			for (int i=list.Count; i<=index; i++) {
				list.Insert(i, null);
				listIdentifiers.Insert(i , null);
			}
			listIdentifiers[index] = elementIdentifier;
			//list[index] = element;
			return elementIdentifier;
		}

		public override ICollection Entries() {
			return list;
		}

		public List(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) : base(session) {
			BeforeInitialize(persister);
			object[] array = (object[]) disassembled;
			for (int i=0; i<array.Length; i++) {
				list.Add( persister.ElementType.Assemble( array[i], session, owner) );
			}
			initialized = true;
		}

		public override object Disassemble(CollectionPersister persister) {
			int length = list.Count;
			object[] result = new object[length];
			for (int i=0; i<length; i++ ) {
				result[i] = persister.ElementType.Disassemble(list[i], session);
			}
			return result;
		}

		public override ICollection GetDeletes(IType elemType) {
			IList deletes = new ArrayList();
			IList sn = (IList) GetSnapshot();
			int end;
			if ( sn.Count > list.Count ) {
				for (int i=list.Count;i<sn.Count; i++) deletes.Add( i );
				end = list.Count;
			} else {
				end = sn.Count;
			}
			for (int i=0;i<end; i++) {
				if ( list[i]==null && sn[i] != null ) deletes.Add( i );
			}
			return deletes;
		}

		public override bool NeedsInserting(object entry, int i, IType elemType) {
			IList sn = (IList) GetSnapshot();
			return list[i]!=null && ( i>=sn.Count || sn[i]==null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType) {
			IList sn = (IList) GetSnapshot();
			return i<sn.Count && sn[i]!=null && list[i]!=null && elemType.IsDirty( list[i], sn[i], session );
		}

		public override object GetIndex(object entry, int i) {
			return i;
		}


		public override bool EntryExists(object entry, int i) {
			return entry!=null;
		}
																					   

		
	}
}
