using System;
using System.Data;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Collection 
{
	/// <summary>
	/// Persistent collections are treated as value objects by Hibernate.
	/// ie. they have no independent existence beyond the object holding
	/// a reference to them. Unlike instances of entity classes, they are
	/// automatically deleted when unreferenced and automatically become
	/// persistent when held by a persistent object. Collections can be
	/// passed between different objects (change "roles") and this might
	/// cause their elements to move from one database table to another.<br>
	/// <br>
	/// Hibernate "wraps" a java collection in an instance of
	/// PersistentCollection. This mechanism is designed to support
	/// tracking of changes to the collection's persistent state and
	/// lazy instantiation of collection elements. The downside is that
	/// only certain abstract collection types are supported and any
	/// extra semantics are lost<br>
	/// <br>
	/// Applications should <em>never</em> use classes in this package 
	/// directly, unless extending the "framework" here.<br>
	/// <br>
	/// Changes to <em>structure</em> of the collection are recorded by the
	/// collection calling back to the session. Changes to mutable
	/// elements (ie. composite elements) are discovered by cloning their
	/// state when the collection is initialized and comparing at flush
	/// time.
	/// 
	/// @author Gavin King
	/// </summary>
	[Serializable]
	public abstract class PersistentCollection 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PersistentCollection));

		[NonSerialized] protected ISessionImplementor session;
		protected bool initialized;
		[NonSerialized] private ArrayList additions;
		private ICollectionSnapshot collectionSnapshot;
		[NonSerialized] protected bool directlyAccessible;

		//careful: these methods do not initialize the collection
		public abstract ICollection Elements();
		public abstract bool Empty { get; }

		public void Read() 
		{
			Initialize(false);
		}

		private bool IsConnectedToSession 
		{ 
			get {return session!=null && session.IsOpen;}
		}

		protected void Write() 
		{
			Initialize(true);
			if (IsConnectedToSession) 
			{
				session.Dirty(this);
			} 
			else 
			{
				collectionSnapshot.SetDirty();
			}
		}

		private bool MayQueueAdd 
		{
			get 
			{
				return 
					!initialized &&
					IsConnectedToSession &&
					session.IsInverseCollection(this);
			}
		}

		protected bool QueueAdd(object element) 
		{
			if ( MayQueueAdd ) 
			{
				if (additions == null) additions = new ArrayList(10);
				additions.Add(element);
				session.Dirty(this); //needed so that we remove this collection from the JCS cache
				return true;
			} 
			else 
			{
				return false;
			}
		}

		protected bool QueueAddAll(ICollection coll) 
		{
			if ( MayQueueAdd ) 
			{
				if (additions==null) additions = new ArrayList(20);
				additions.AddRange(coll);
				return true;
			} 
			else 
			{
				return false;
			}
		}

		//TODO: H2.0.3 new method
		public virtual void DelayedAddAll(ICollection coll) 
		{
			throw new AssertionFailure("Collection does not support delayed initialization.");
		}

		// TODO: h2.0.3 synhc - I don't see AddAll in the H code...
		public virtual bool AddAll(ICollection coll) 
		{
			throw new AssertionFailure("Collection does not support delayed initialization");
		}

		protected PersistentCollection(ISessionImplementor session) 
		{
			this.session = session;
		}

		/// <summary>
		/// As far as every client is concerned, the collection is loaded after this call!
		/// (Actually is done lazily
		/// </summary>
		/// <param name="lazy"></param>
		/// <returns></returns>
		public virtual object GetInitialValue(bool lazy) 
		{
			if ( !lazy ) 
			{
				session.Initialize(this, false);
				initialized = true;
			}
			return this;
		}

		public virtual object GetCachedValue() 
		{
			initialized = true; //TODO: only needed for query FETCH so should move out of here
			return this;
		}

		/// <summary>
		/// Override on some subclasses
		/// </summary>
		public virtual void BeginRead() 
		{
			// override on some subclasses
		}
	
		/// <summary>
		/// It is my thoughts to have this be the portion that takes care of 
		/// converting the Identifier to the element...
		/// </summary>
		[Obsolete("Should be replaced with EndRead(CollectionPersister, object) - need to verify")]
		public virtual void EndRead() 
		{
			// override on some subclasses
		}

		/// <summary>
		/// Called when there are no other open IDataReaders so this PersistentCollection
		/// is free to resolve all of the Identifiers to their Entities (which potentially
		/// involves issuing a new query and opening a new IDataReader.
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="owner"></param>
		public abstract void EndRead(CollectionPersister persister, object owner) ;

		public void Initialize(bool writing) 
		{
			if (!initialized) 
			{
				if ( IsConnectedToSession ) 
				{
					try 
					{
						session.Initialize(this, writing);
						initialized = true;
						// do this after setting initialized to true or it will recurse
						if (additions!=null) 
						{
							DelayedAddAll(additions);
							additions = null;
						}
					} 
					catch (Exception e) 
					{
						log.Error("Failed to lazily initialize a collection", e);
						throw new LazyInitializationException("Failed to lazily initialize a collection", e);
					}
				} 
				else 
				{
					throw new LazyInitializationException("Failed to lazily initialize a collection - no session");
				}
			}
		}

		public bool UnsetSession(ISessionImplementor session) 
		{
			if (session==this.session) 
			{
				this.session=null;
				return true;
			}
			else 
			{
				return false;
			}
		}

		public bool SetSession(ISessionImplementor session) 
		{
			if (session == this.session) 
			{
				return false;
			} 
			else 
			{
				if ( IsConnectedToSession ) 
				{
					throw new HibernateException("Illegal attempt to associate a collection with two open sessions");
				} 
				else 
				{
					this.session = session;
					return true;
				}
			}
		}

		public virtual bool IsDirectlyAccessible 
		{
			get { return directlyAccessible; }
		}

		public virtual bool IsArrayHolder {
			get { return false; }
		}

		public abstract ICollection Entries();
		
		//TODO: determine where this is used - not in H2.0.3
		[Obsolete("Not in H2.0.3 - can't find any usage in NH")]
		public abstract void ReadEntries(ICollection entries);
		
		/// <summary>
		/// Reads the elements Identifier from the reader.
		/// </summary>
		/// <param name="reader">The IDataReader that contains the value of the Identifier</param>
		/// <param name="role">The persister for this Collection.</param>
		/// <param name="owner">The owner of this Collection.</param>
		/// <returns>The value of the Identifier.</returns>
		public abstract object ReadFrom(IDataReader reader, CollectionPersister role, object owner);
		
		public abstract void WriteTo(IDbCommand st, CollectionPersister role, object entry, int i, bool writeOrder);
		public abstract object GetIndex(object entry, int i);
		public abstract void BeforeInitialize(CollectionPersister persister);

		public abstract bool EqualsSnapshot(IType elementType);
		protected abstract object Snapshot(CollectionPersister persister);

		public abstract object Disassemble(CollectionPersister persister);

		public virtual bool NeedsRecreate(CollectionPersister persister) 
		{
			return false;
		}

		public object GetSnapshot(CollectionPersister persister) 
		{
			return (persister==null) ? null : Snapshot(persister);
		}

		/// <summary>
		/// Default behavior is to call Read; will be overridden in deep lazy collections
		/// </summary>
		/// TODO: H2.0.3 declares this as final
		public void ForceLoad() 
		{
			Read();
		}

		public abstract bool EntryExists(object entry, int i);
		public abstract bool NeedsInserting(object entry, int i, IType elemType);
		public abstract bool NeedsUpdating(object entry, int i, IType elemType);
		public abstract ICollection GetDeletes(IType elemType);

		protected object GetSnapshot() 
		{
			return session.GetSnapshot(this);
		}

		public bool WasInitialized 
		{
			get { return initialized; }
		}

		public bool HasQueuedAdds 
		{
			get { return additions!= null; }
		}

		public ICollection QueuedAddsCollection 
		{
			get { return additions; }
		}

		public virtual ICollectionSnapshot CollectionSnapshot 
		{
			get { return collectionSnapshot; }
			set { collectionSnapshot = value; }
		}

		// looks like it is used by IdentifierBag
		public virtual void PreInsert(CollectionPersister persister, object entry, int i) {}
		public abstract ICollection GetOrphans(object snapshot);
		public static void IdentityRemoveAll(IList list, ICollection collection, ISessionImplementor session) 
		{
			IEnumerator enumer = collection.GetEnumerator();
			while(enumer.MoveNext()) PersistentCollection.IdentityRemove(list, enumer.Current, session);
		}

		public static void IdentityRemove(IList list, object obj, ISessionImplementor session) 
		{
			int indexOfEntityToRemove = -1;

			if(session.IsSaved(obj)) 
			{
				object idOfCurrent = session.GetEntityIdentifierIfNotUnsaved(obj);
				for(int i = 0; i < list.Count; i++) 
				{
					object idOfOld = session.GetEntityIdentifierIfNotUnsaved(list[i]);
					if(idOfCurrent.Equals(idOfOld) )
					{
						// in hibernate this used the Iterator to remove the item - since in .NET
						// the Enumerator is read only we have to change the implementation slightly. 
						indexOfEntityToRemove = i;
						break;
					}
				}

				if(indexOfEntityToRemove != -1) list.RemoveAt(indexOfEntityToRemove);
			}
		}

		
		#region - Hibernate Collection Proxy Classes

		/*
		 * These were needed by Hibernate because Java's collections provide methods
		 * to get sublists, modify a collection with an iterator - all things that 
		 * Hibernate needs to be made aware of.  If .net changes their collection interfaces
		 * then we can readd these back in.
		 */ 
		

		#endregion

	}

	

	
}
