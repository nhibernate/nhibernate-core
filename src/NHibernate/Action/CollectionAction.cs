using System;
using System.Runtime.Serialization;
using NHibernate.Cache;
using NHibernate.Cache.Access;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Persister.Collection;
using NHibernate.Util;

namespace NHibernate.Action
{
	/// <summary>
	/// Any action relating to insert/update/delete of a collection
	/// </summary>
	[Serializable]
	public abstract partial class CollectionAction : IExecutable, IComparable<CollectionAction>, IDeserializationCallback
	{
		private readonly object key;
		private object finalKey;
		[NonSerialized] private ICollectionPersister persister;
		private readonly ISessionImplementor session;
		private readonly string collectionRole;
		private readonly IPersistentCollection collection;
		private ISoftLock softLock;

		/// <summary>
		/// Initializes a new instance of <see cref="CollectionAction"/>.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> that is responsible for the persisting the Collection.</param>
		/// <param name="collection">The Persistent collection.</param>
		/// <param name="key">The identifier of the Collection.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> that the Action is occurring in.</param>
		protected CollectionAction(ICollectionPersister persister, IPersistentCollection collection, object key,
		                        ISessionImplementor session)
		{
			this.persister = persister;
			this.session = session;
			this.key = key;
			collectionRole = persister.Role;
			this.collection = collection;
		}

		protected internal IPersistentCollection Collection
		{
			get { return collection; }
		}

		protected internal ICollectionPersister Persister
		{
			get { return persister; }
		}

		protected internal object Key
		{
			get
			{
				finalKey = key;
				if (key is DelayedPostInsertIdentifier)
				{
					// need to look it up from the persistence-context
					finalKey = session.PersistenceContext.GetEntry(collection.Owner).Id;
					if (finalKey == key)
					{
						// we may be screwed here since the collection action is about to execute
						// and we do not know the final owner key value
					}
				}
				return finalKey;
			}
		}

		protected internal ISessionImplementor Session
		{
			get { return session; }
		}

		#region IExecutable Members

		/// <summary>
		/// What spaces (tables) are affected by this action?
		/// </summary>
		public string[] PropertySpaces
		{
			get { return Persister.CollectionSpaces; }
		}

		/// <summary> Called before executing any actions</summary>
		public virtual void BeforeExecutions()
		{
			// we need to obtain the lock before any actions are
			// executed, since this may be an inverse="true"
			// bidirectional association and it is one of the
			// earlier entity actions which actually updates
			// the database (this action is responsible for
			// second-level cache invalidation only)
			if (persister.HasCache)
			{
				CacheKey ck = session.GenerateCacheKey(key, persister.KeyType, persister.Role);
				softLock = persister.Cache.Lock(ck, null);
			}
		}

		/// <summary>Execute this action</summary>
		public abstract void Execute();

		public virtual BeforeTransactionCompletionProcessDelegate BeforeTransactionCompletionProcess
		{
			get 
			{ 
				return null; 
			}
		}

		public virtual AfterTransactionCompletionProcessDelegate AfterTransactionCompletionProcess
		{

			get
			{
				// Only make sense to add the delegate if there is a cache.
				if (persister.HasCache)
				{
					return new AfterTransactionCompletionProcessDelegate((success) =>
					{
						CacheKey ck = new CacheKey(key, persister.KeyType, persister.Role, Session.Factory);
						persister.Cache.Release(ck, softLock);
					});
				}
				return null; 
			}
		}
		
		#endregion

		public ISoftLock Lock
		{
			get { return softLock; }
		}

		protected internal void Evict()
		{
			if (persister.HasCache)
			{
				CacheKey ck = session.GenerateCacheKey(key, persister.KeyType, persister.Role);
				persister.Cache.Evict(ck);
			}
		}

		#region IComparable<CollectionAction> Members

		///<summary>
		///Compares the current object with another object of the same type.
		///</summary>
		///<returns>
		///A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other. 
		///</returns>
		///<param name="other">An object to compare with this object.</param>
		public virtual int CompareTo(CollectionAction other)
		{
			//sort first by role name
			int roleComparison = string.Compare(collectionRole, other.collectionRole);
			if (roleComparison != 0)
			{
				return roleComparison;
			}
			//then by fk
			return persister.KeyType.Compare(key, other.key);
		}

		#endregion

		public override string ToString()
		{
			return StringHelper.Unqualify(GetType().FullName) + MessageHelper.InfoString(collectionRole, key);
		}

		#region IDeserializationCallback Members

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			persister = session.Factory.GetCollectionPersister(collectionRole);
		}

		#endregion
	}
}
