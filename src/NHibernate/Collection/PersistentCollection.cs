using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection {
	/// <summary>
	/// Summary description for PersistentCollection.
	/// </summary>
	[Serializable]
	public abstract class PersistentCollection {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PersistentCollection));

		[NonSerialized] protected ISessionImplementor session;
		protected bool initialized;
		[NonSerialized] private ArrayList additions;
		private ICollectionSnapshot collectionSnapshot;
		[NonSerialized] protected bool directlyAccessible;

		//careful: these methods do not initialize the collection
		public abstract ICollection Elements();
		public abstract bool Empty { get; }

		public abstract void ReplaceElements(IDictionary replacements);

		public void Read() {
			Initialize(false);
		}

		private bool IsConnectedToSession { 
			get {
				return session!=null && session.IsOpen;
			}
		}

		protected void Write() {
			Initialize(true);
			if (session!=null && session.IsOpen) {
				session.Dirty(this);
			} else {
				collectionSnapshot.SetDirty();
			}
		}

		private bool MayQueueAdd {
			get {
				return 
					!initialized &&
					IsConnectedToSession &&
					session.IsInverseCollection(this);
			}
		}

		protected bool QueueAdd(object element) {
			if ( MayQueueAdd ) {
				if (additions == null) additions = new ArrayList(10);
				additions.Add(element);
				return true;
			} else {
				return false;
			}
		}

		protected bool QueueAddAll(ICollection coll) {
			if ( MayQueueAdd ) {
				if (additions==null) additions = new ArrayList(20);
				additions.AddRange(coll);
				return true;
			} else {
				return false;
			}
		}

		public virtual bool AddAll(ICollection coll) {
			throw new AssertionFailure("Collection does not support delayed initialization");
		}

		protected PersistentCollection(ISessionImplementor session) {
			this.session = session;
		}

		// As far as every client is concerned, the collection is loaded after this call!
		// (Actually is done lazily
		public virtual object GetInitialValue(bool lazy) {
			if ( !lazy ) {
				session.Initialize(this, false);
				initialized = true;
			}
			return this;
		}

		public virtual object GetCachedValue() {
			return this;
		}

		public void Initialize(bool writing) {
			if (!initialized) {
				if ( session!=null && session.IsOpen ) {
					try {
						session.Initialize(this, writing);
						initialized = true;
						// do this after setting initialized to true or it will recurse
						if (additions!=null) {
							AddAll(additions);
							additions = null;
						}
					} catch (Exception e) {
						log.Error("Failed to lazily initialize a collection", e);
						throw new LazyInitializationException("Failed to lazily initialize a collection", e);
					}
				} else {
					throw new LazyInitializationException("Failed to lazily initialize a collection - no session");
				}
			}
		}

		public bool UnsetSession(ISessionImplementor session) {
			if (session==this.session) {
				this.session=null;
				return true;
			}
			else {
				return false;
			}
		}

		public bool SetSession(ISessionImplementor session) {
			if (session == this.session) {
				return false;
			} else {
				if ( this.session!=null && this.session.IsOpen ) {
					throw new HibernateException("Illegal attempt to associate a collection with two open sessions");
				} else {
					this.session = session;
					return true;
				}
			}
		}

		public bool IsDirectlyAccessible {
			get { return directlyAccessible; }
		}

		public virtual bool IsArrayHolder {
			get { return false; }
		}

		public abstract ICollection Entries();
		public abstract void ReadEntries(ICollection entries);
		public abstract object ReadFrom(IDataReader reader, CollectionPersister role, object entry);
		public abstract void WriteTo(IDbCommand st, CollectionPersister role, object entry, int i, bool writeOrder);
		public abstract object GetIndex(object entry, int i);
		public abstract void BeforeInitialize(CollectionPersister persister);

		public abstract bool EqualsSnapshot(IType elementType);
		protected abstract object Snapshot(CollectionPersister persister);

		public abstract object Disassemble(CollectionPersister persister);

		public virtual bool NeedsRecreate(IType elemType) {
			return false;
		}

		public object GetSnapshot(CollectionPersister persister) {
			return (persister==null) ? null : Snapshot(persister);
		}

		//default behavior; will be overridden in deep lazy collections
		public void ForceLoad() {
			Read();
		}

		public abstract bool EntryExists(object entry, int i);
		public abstract bool NeedsInserting(object key, int i, IType elemType);
		public abstract bool NeedsUpdating(object key, int i, IType elemType);
		public abstract ICollection GetDeletes(IType elemType);

		protected object GetSnapshot() {
			return session.GetSnapshot(this);
		}

		public bool WasInitialized {
			get { return initialized; }
		}

		public bool HasQueuedAdds {
			get { return additions!= null; }
		}

		public ICollection QueuedAddsCollection {
			get { return additions; }
		}

		public ICollectionSnapshot CollectionSnapshot {
			get { return collectionSnapshot; }
			set { collectionSnapshot = value; }
		}

		internal sealed class ListProxy : IList {
			private PersistentCollection pc;
			private IList list;
			
			public ListProxy(PersistentCollection pc, IList list) {
				this.pc = pc;
				this.list = list;
			}

			public bool IsFixedSize {
				get { return list.IsFixedSize; }
			}
			public bool IsReadOnly {
				get { return list.IsReadOnly; }
			}
			public int Count {
				get { return list.Count; }
			}
			public bool IsSynchronized {
				get { return list.IsSynchronized; }
			}
			public object SyncRoot {
				get { return list.SyncRoot; }
			}
			public bool Contains(object obj) {
				return list.Contains(obj);
			}
			public int IndexOf(object obj) {
				return list.IndexOf(obj); 
			}
			public void CopyTo(Array array, int index) {
				list.CopyTo(array, index);
			}
			public IEnumerator GetEnumerator() {
				return new EnumeratorProxy(list.GetEnumerator());
			}
			public int Add(object obj) {
				pc.Write();
				return list.Add(obj);
			}
			public void Clear() {
				pc.Write();
				list.Clear();
			}
			public void Insert(int index, object obj) {
				pc.Write();
				list.Insert(index, obj);
			}
			public void RemoveAt(int index) {
				pc.Write();
				list.RemoveAt(index);
			}
			public object this [ int index] {
				get { return list[index]; }
				set {
					pc.Write();
					list[index] = value;
				}
			}

			public void Remove(object obj) {
				pc.Write();
				list.Remove(obj);
			}
		}

		internal sealed class EnumeratorProxy : IEnumerator {
			private IEnumerator en;
			public EnumeratorProxy(IEnumerator en) {
				this.en = en;
			}
			public object Current {
				get { return en.Current; }
			}
			public bool MoveNext() {
				return en.MoveNext();
			}
			public void Reset() {
				en.Reset();
			}
		}

	}

	

	
}
