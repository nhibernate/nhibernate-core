using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection {
	
	public class Map : PersistentCollection, IDictionary {
		
		protected IDictionary map;
		protected IDictionary mapIdentifiers;

		protected override object Snapshot(CollectionPersister persister) {
			Hashtable clonedMap = new Hashtable( map.Count );
			foreach(DictionaryEntry e in map) {
				clonedMap[e.Key] = persister.ElementType.DeepCopy( e.Value );
			}
			return clonedMap;
		}

		public override bool EqualsSnapshot(IType elementType) {
			IDictionary xmap = (IDictionary) GetSnapshot();
			if ( xmap.Count!=this.map.Count ) return false;
			foreach(DictionaryEntry entry in map) {
				if ( elementType.IsDirty( entry.Value, xmap[entry.Key], session)) return false;
			}
			return true;
		}

		public Map(ISessionImplementor session) : base(session) { }
		
		public Map(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) : base(session) {
			BeforeInitialize(persister);
			object[] array = (object[]) disassembled;
			for (int i=0; i<array.Length; i+=2)
				map[ persister.IndexType.Assemble( array[i], session, owner) ] =
					persister.ElementType.Assemble( array[i+1], session, owner );
			initialized = true;
		}

		public override void BeforeInitialize(CollectionPersister persister) {
			//TODO: check this
			//this.map = persister.HasOrdering ? LinkedHashCollectionHelper.CreateLinkedHashMap() : new Hashtable();
			this.map = new Hashtable();
			this.mapIdentifiers = new Hashtable();
		}

		public Map(ISessionImplementor session, IDictionary map) : base(session) {
			this.map = map;
			initialized = true;
			directlyAccessible = true;
		}

		public int Count {
			get {
				Read();
				return map.Count;
			}
		}

		public bool IsSynchronized {
			get { return false; }
		}
		public bool IsFixedSize {
			get { return false; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public object SyncRoot {
			get { return this; }
		}
		public ICollection Keys 
		{
			get 
			{ 
				//Read();
				return new CollectionProxy(this, map.Keys);
			}
		}
		public ICollection Values 
		{
			get 
			{  
				//Read();
				return new CollectionProxy(this, map.Values);
			}
			
		}
		public IEnumerator GetEnumerator() {
			Read();
			return map.GetEnumerator();
			
		}

		IEnumerator IEnumerable.GetEnumerator() {
			Read();
			return map.GetEnumerator();
			
		}

		IDictionaryEnumerator IDictionary.GetEnumerator() {
			Read();
			return map.GetEnumerator();
		}

		public void CopyTo(System.Array array, int index) {
			Read();
			map.CopyTo(array, index);
		}
		public void Add(object key, object value) {
			Write();
			map.Add(key, value);
		}
		public bool Contains(object key) {
			Read();
			return map.Contains(key);
		}

		public object this [object key] {
			get {
				Read();
				return map[key];
			}
			set {
				Write();
				map[key] = value;
			}
		}

		public void Remove(object key) {
			Write();
			map.Remove(key);
		}

		public void Clear() {
			Write();
			map.Clear();
		}

		public override void EndRead(CollectionPersister persister, object owner) {
			foreach(DictionaryEntry entry in mapIdentifiers){
				object index = entry.Key;
				object elementIdentifier = entry.Value;

				object element = persister.ElementType.ResolveIdentifier(elementIdentifier, session, owner);

				map[index] = element;
			}
		}

		public override ICollection Elements() {
			return map.Values;
		}
		public override bool Empty {
			get { return map.Count==0; }
		}
		public override string ToString() {
			Read();
			return map.ToString();
		}

		public override void WriteTo(IDbCommand st, CollectionPersister persister, object entry, int i, bool writeOrder) {
			DictionaryEntry e = (DictionaryEntry) entry;
			persister.WriteElement(st, e.Value, writeOrder, session);
			persister.WriteIndex(st, e.Key, writeOrder, session);
		}

		public override object ReadFrom(IDataReader rs, CollectionPersister persister, object owner) {
			//object element = persister.ReadElement(rs, owner, session);
			object elementIdentifier = persister.ReadElementIdentifier(rs, owner, session);

			object index = persister.ReadIndex(rs, session);
			map[index] = null;
			mapIdentifiers[index] = elementIdentifier;
			return elementIdentifier;
		}

		public override ICollection Entries() {
			ArrayList entries = new ArrayList();
			foreach(DictionaryEntry entry in map) {
				entries.Add(entry);
			}
			return entries;
		}

		public override void ReadEntries(ICollection entries) {
			foreach(DictionaryEntry entry in entries) {
				map[entry.Key] = entry.Value;
			}
		}

		public override object Disassemble(CollectionPersister persister) {
			object[] result = new object[map.Count * 2];
			int i=0;
			foreach(DictionaryEntry e in map) {
				result[i++] = persister.IndexType.Disassemble( e.Key, session );
				result[i++] = persister.ElementType.Disassemble( e.Value, session );
			}
			return result;
		}

		public override ICollection GetDeletes(IType elemType) {
			IList deletes = new ArrayList();
			foreach(DictionaryEntry e in ((IDictionary)GetSnapshot())) {
				object key = e.Key;
				if ( e.Value!=null && map[key]==null ) deletes.Add(key);
			}
			return deletes;
		}
		public override bool NeedsInserting(object entry, int i, IType elemType) {
			IDictionary sn = (IDictionary) GetSnapshot();
			DictionaryEntry e = (DictionaryEntry) entry;
			return (e.Value!=null && sn[e.Key] == null);
		}

		public override bool NeedsUpdating(object entry, int i, IType elemType) {
			IDictionary sn = (IDictionary) GetSnapshot();
			DictionaryEntry e = (DictionaryEntry) entry;
			object snValue = sn[e.Key];
			return (e.Value != null && snValue!=null && elemType.IsDirty(snValue, e.Value, session) );
		}
		public override object GetIndex(object entry, int i) {
			return ((DictionaryEntry)entry).Key;
		}
		public override bool Equals(object other) {
			Read();
			return map.Equals(other);
		}
		public override int GetHashCode() {
			Read();
			return map.GetHashCode();
		}

		public override bool EntryExists(object entry, int i) {
			return ( (DictionaryEntry)entry).Value!=null;
		}


		
	}
}
