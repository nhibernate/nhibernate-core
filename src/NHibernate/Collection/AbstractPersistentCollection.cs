using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Collection.Generic;
using NHibernate.Collection.Trackers;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Loader;
using NHibernate.Persister.Collection;
using NHibernate.Proxy;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection
{
	/// <summary>
	/// Base class for implementing <see cref="IPersistentCollection"/>.
	/// </summary>
	// 6.0 TODO: remove ILazyInitializedCollection once IPersistentCollection derives from it
	[Serializable]
	public abstract partial class AbstractPersistentCollection : IPersistentCollection, ILazyInitializedCollection
	{
		// Since v5.3
		[Obsolete("This field has no more usages in NHibernate and will be removed in a future version.")]
		protected internal static readonly object Unknown = new object(); //place holder
		// Since v5.3
		[Obsolete("This field has no more usages in NHibernate and will be removed in a future version.")]
		protected internal static readonly object NotFound = new object(); //place holder

		// Since v5.3
		[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
		protected interface IDelayedOperation
		{
			object AddedInstance { get; }
			object Orphan { get; }
			void Operate();
		}

		class TypeComparer : IEqualityComparer<object>
		{
			private readonly IType _type;
			private readonly ISessionFactoryImplementor _sessionFactory;

			public TypeComparer(IType type, ISessionFactoryImplementor sessionFactory)
			{
				_type = type;
				_sessionFactory = sessionFactory;
			}

			public new bool Equals(object x, object y)
			{
				return _type.IsEqual(x, y, _sessionFactory);
			}

			public int GetHashCode(object obj)
			{
				return _type.GetHashCode(obj, _sessionFactory);
			}
		}

		// 6.0 TODO: Remove
		private class AdditionEnumerable : IEnumerable
		{
			private readonly AbstractPersistentCollection enclosingInstance;

			public AdditionEnumerable(AbstractPersistentCollection enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public IEnumerator GetEnumerator()
			{
				return new AdditionEnumerator(enclosingInstance);
			}

			private class AdditionEnumerator : IEnumerator
			{
				private readonly AbstractPersistentCollection enclosingInstance;
				private int position = -1;

				public AdditionEnumerator(AbstractPersistentCollection enclosingInstance)
				{
					this.enclosingInstance = enclosingInstance;
				}

				public object Current
				{
					get
					{
						try
						{
							return enclosingInstance.operationQueue[position].AddedInstance;
						}
						catch (IndexOutOfRangeException ex)
						{
							throw new InvalidOperationException(
								"MoveNext as not been called or its last call has yielded false (meaning the enumerator is beyond the end of the enumeration).",
								ex);
						}
					}
				}

				public bool MoveNext()
				{
					position++;
					return (position < enclosingInstance.operationQueue.Count);
				}

				public void Reset()
				{
					position = -1;
				}
			}
		}

		[NonSerialized] private ISessionImplementor session;
		private bool initialized;
#pragma warning disable 618
		[NonSerialized] private List<IDelayedOperation> operationQueue; // 6.0 TODO: Remove
#pragma warning restore 618
		[NonSerialized] private bool directlyAccessible;
		[NonSerialized] private bool initializing;
		[NonSerialized] private AbstractQueueOperationTracker _queueOperationTracker;

		private object owner;
		private int cachedSize = -1;

		private string role;
		private object key;

		// collections detect changes made via their public interface and mark
		// themselves as dirty as a performance optimization
		private bool dirty;
		private object storedSnapshot;

		/// <summary>
		/// Not called by Hibernate, but used by non-NET serialization, eg. SOAP libraries.
		/// </summary>
		protected AbstractPersistentCollection() {}

		protected AbstractPersistentCollection(ISessionImplementor session)
		{
			this.session = session;
		}

		public string Role
		{
			get { return role; }
		}

		public object Key
		{
			get { return key; }
		}

		public bool IsUnreferenced
		{
			get { return role == null; }
		}

		public bool IsDirty
		{
			get { return dirty; }
		}

		public object StoredSnapshot
		{
			get { return storedSnapshot; }
		}

		protected int CachedSize
		{
			get { return cachedSize; }
			set { cachedSize = value; }
		}

		/// <summary>
		/// Is the collection currently connected to an open session?
		/// </summary>
		protected bool IsConnectedToSession
		{
			get { return session != null && session.IsOpen && session.PersistenceContext.ContainsCollection(this); }
		}

		/// <summary>
		/// Is this collection in a state that would allow us to "queue" additions?
		/// </summary>
		protected bool IsOperationQueueEnabled
		{
			get { return !initialized && IsConnectedToSession && IsInverseCollection; }
		}

		/// <summary> Is this collection in a state that would allow us to
		/// "queue" puts? This is a special case, because of orphan
		/// delete.
		/// </summary>
		protected bool PutQueueEnabled
		{
			get { return !initialized && IsConnectedToSession && InverseOneToManyOrNoOrphanDelete; }
		}

		/// <summary> Is this collection in a state that would allow us to
		/// "queue" clear? This is a special case, because of orphan
		/// delete.
		/// </summary>
		protected bool ClearQueueEnabled
		{
			get { return !initialized && IsConnectedToSession && InverseCollectionNoOrphanDelete; }
		}

		/// <summary> Is this the "inverse" end of a bidirectional association?</summary>
		protected bool IsInverseCollection
		{
			get
			{
				CollectionEntry ce = session.PersistenceContext.GetCollectionEntry(this);
				return ce != null && ce.LoadedPersister.IsInverse;
			}
		}

		/// <summary> 
		/// Is this the "inverse" end of a bidirectional association with
		/// no orphan delete enabled?
		/// </summary>
		protected bool InverseCollectionNoOrphanDelete
		{
			get
			{
				CollectionEntry ce = session.PersistenceContext.GetCollectionEntry(this);
				return ce != null && ce.LoadedPersister.IsInverse && !ce.LoadedPersister.HasOrphanDelete;
			}
		}

		/// <summary> 
		/// Is this the "inverse" end of a bidirectional one-to-many, or 
		/// of a collection with no orphan delete?
		/// </summary>
		protected bool InverseOneToManyOrNoOrphanDelete
		{
			get
			{
				CollectionEntry ce = session.PersistenceContext.GetCollectionEntry(this);
				return
					ce != null && ce.LoadedPersister.IsInverse
					&& (ce.LoadedPersister.IsOneToMany || !ce.LoadedPersister.HasOrphanDelete);
			}
		}

		/// <summary>
		/// Return the user-visible collection (or array) instance
		/// </summary>
		/// <returns>
		/// By default, the NHibernate wrapper is an acceptable collection for
		/// the end user code to work with because it is interface compatible.
		/// An NHibernate PersistentList is an IList, an NHibernate PersistentMap is an IDictionary
		/// and those are the types user code is expecting.
		/// </returns>
		public virtual object GetValue()
		{
			return this;
		}

		public virtual bool RowUpdatePossible
		{
			get { return true; }
		}

		/// <summary></summary>
		protected virtual ISessionImplementor Session
		{
			get { return session; }
		}

		public virtual object Owner
		{
			get { return owner; }
			set { owner = value; }
		}

		public void ClearDirty()
		{
			dirty = false;
		}

		public void Dirty()
		{
			dirty = true;
		}

		/// <summary>
		/// Is the initialized collection empty?
		/// </summary>
		public abstract bool Empty { get; } //Careful: these methods do not initialize the collection.

		/// <summary>
		/// Called by any read-only method of the collection interface
		/// </summary>
		public virtual void Read()
		{
			Initialize(false);
		}

		/// <summary> Called by the <tt>Count</tt> property</summary>
		protected virtual bool ReadSize()
		{
			if (!initialized)
			{
				if (cachedSize != -1 && !HasQueuedOperations)
				{
					return true;
				}

				ThrowLazyInitializationExceptionIfNotConnected();
				var entry = session.PersistenceContext.GetCollectionEntry(this);
				var persister = entry.LoadedPersister;
				if (persister.IsExtraLazy)
				{
					var queueOperationTracker = GetOrCreateQueueOperationTracker();
					if (queueOperationTracker == null)
					{
						if (HasQueuedOperations)
						{
							session.Flush();
						}

						cachedSize = persister.GetSize(entry.LoadedKey, session);
						return true;
					}

					if (!queueOperationTracker.Cleared && !queueOperationTracker.DatabaseCollectionSize.HasValue)
					{
						queueOperationTracker.DatabaseCollectionSize = persister.GetSize(entry.LoadedKey, session);
					}

					cachedSize = queueOperationTracker.Cleared
						? queueOperationTracker.GetQueueSize()
						: queueOperationTracker.GetCollectionSize();
					return true;
				}
				Read();
			}
			return false;
		}

		// Since v5.3
		[Obsolete("This method has no more usages in NHibernate and will be removed in a future version.")]
		protected virtual bool? ReadIndexExistence(object index)
		{
			if (!initialized)
			{
				ThrowLazyInitializationExceptionIfNotConnected();
				CollectionEntry entry = session.PersistenceContext.GetCollectionEntry(this);
				ICollectionPersister persister = entry.LoadedPersister;
				if (persister.IsExtraLazy)
				{
					if (HasQueuedOperations)
					{
						session.Flush();
					}
					return persister.IndexExists(entry.LoadedKey, index, session);
				}
			}
			Read();
			return null;
		}

		// Since v5.3
		[Obsolete("This method has no more usages in NHibernate and will be removed in a future version.")]
		protected virtual bool? ReadElementExistence(object element)
		{
			if (!initialized)
			{
				ThrowLazyInitializationExceptionIfNotConnected();
				CollectionEntry entry = session.PersistenceContext.GetCollectionEntry(this);
				ICollectionPersister persister = entry.LoadedPersister;
				if (persister.IsExtraLazy)
				{
					if (HasQueuedOperations)
					{
						session.Flush();
					}
					return persister.ElementExists(entry.LoadedKey, element, session);
				}
			}
			Read();
			return null;
		}

		// Since v5.3
		[Obsolete("This method has no more usages in NHibernate and will be removed in a future version.")]
		protected virtual object ReadElementByIndex(object index)
		{
			if (!initialized)
			{
				ThrowLazyInitializationExceptionIfNotConnected();
				CollectionEntry entry = session.PersistenceContext.GetCollectionEntry(this);
				ICollectionPersister persister = entry.LoadedPersister;
				if (persister.IsExtraLazy)
				{
					if (HasQueuedOperations)
					{
						session.Flush();
					}
					var elementByIndex = persister.GetElementByIndex(entry.LoadedKey, index, session, owner);
					return persister.NotFoundObject == elementByIndex ? NotFound : elementByIndex;
				}
			}
			Read();
			return Unknown;
		}

		protected virtual bool? ReadKeyExistence<TKey, TValue>(TKey elementKey)
		{
			if (!initialized)
			{
				ThrowLazyInitializationExceptionIfNotConnected();
				CollectionEntry entry = session.PersistenceContext.GetCollectionEntry(this);
				ICollectionPersister persister = entry.LoadedPersister;
				if (persister.IsExtraLazy)
				{
					var queueOperationTracker = (AbstractMapQueueOperationTracker<TKey, TValue>) GetOrCreateQueueOperationTracker();
					if (queueOperationTracker == null)
					{
						if (HasQueuedOperations)
						{
							session.Flush();
						}

						return persister.IndexExists(entry.LoadedKey, elementKey, session);
					}

					if (queueOperationTracker.ContainsKey(elementKey))
					{
						return true;
					}

					if (queueOperationTracker.Cleared)
					{
						return false;
					}

					if (queueOperationTracker.IsElementKeyQueuedForDelete(elementKey))
					{
						return false;
					}

					// As keys are unordered we don't have to calculate the current order of the key
					return persister.IndexExists(entry.LoadedKey, elementKey, session);
				}
				Read();
			}
			return null;
		}

		protected virtual bool? ReadElementExistence<T>(T element, out bool? existsInDb)
		{
			if (!initialized)
			{
				ThrowLazyInitializationExceptionIfNotConnected();
				CollectionEntry entry = session.PersistenceContext.GetCollectionEntry(this);
				ICollectionPersister persister = entry.LoadedPersister;
				if (persister.IsExtraLazy)
				{
					var queueOperationTracker = (AbstractCollectionQueueOperationTracker<T>) GetOrCreateQueueOperationTracker();
					if (queueOperationTracker == null)
					{
						if (HasQueuedOperations)
						{
							session.Flush();
						}

						existsInDb = persister.ElementExists(entry.LoadedKey, element, session);
						return existsInDb;
					}

					if (queueOperationTracker.ContainsElement(element))
					{
						existsInDb = null;
						return true;
					}

					if (queueOperationTracker.Cleared)
					{
						existsInDb = null;
						return false;
					}

					if (queueOperationTracker.IsElementQueuedForDelete(element))
					{
						existsInDb = null;
						return false;
					}

					existsInDb = persister.ElementExists(entry.LoadedKey, element, session);
					return existsInDb;
				}
				Read();
			}

			existsInDb = null;
			return null;
		}

		protected virtual bool? TryReadElementByKey<TKey, TValue>(TKey elementKey, out TValue element, out bool? existsInDb)
		{
			if (!initialized)
			{
				ThrowLazyInitializationExceptionIfNotConnected();
				CollectionEntry entry = session.PersistenceContext.GetCollectionEntry(this);
				ICollectionPersister persister = entry.LoadedPersister;
				if (persister.IsExtraLazy)
				{
					var queueOperationTracker = (AbstractMapQueueOperationTracker<TKey, TValue>) GetOrCreateQueueOperationTracker();
					if (queueOperationTracker == null)
					{
						if (HasQueuedOperations)
						{
							session.Flush();
						}
					}
					else
					{
						if (queueOperationTracker.TryGetElementByKey(elementKey, out element))
						{
							existsInDb = null;
							return true;
						}

						if (queueOperationTracker.Cleared)
						{
							existsInDb = null;
							return false;
						}

						if (queueOperationTracker.IsElementKeyQueuedForDelete(elementKey))
						{
							existsInDb = null;
							return false;
						}
					}

					var elementByKey = persister.GetElementByIndex(entry.LoadedKey, elementKey, session, owner);
					if (persister.NotFoundObject == elementByKey)
					{
						element = default(TValue);
						existsInDb = false;
						return false;
					}

					element = (TValue) elementByKey;
					existsInDb = true;
					return true;
				}
				Read();
			}

			element = default(TValue);
			existsInDb = null;
			return null;
		}

		protected virtual bool? TryReadElementAtIndex<T>(int index, out T element)
		{
			if (!initialized)
			{
				ThrowLazyInitializationExceptionIfNotConnected();
				CollectionEntry entry = session.PersistenceContext.GetCollectionEntry(this);
				ICollectionPersister persister = entry.LoadedPersister;
				if (persister.IsExtraLazy)
				{
					var queueOperationTracker = (AbstractCollectionQueueOperationTracker<T>) GetOrCreateQueueOperationTracker();
					if (queueOperationTracker == null)
					{
						if (HasQueuedOperations)
						{
							session.Flush();
						}
					}
					else if (HasQueuedOperations)
					{
						if (queueOperationTracker.RequiresFlushing(nameof(queueOperationTracker.TryGetElementAtIndex)))
						{
							session.Flush();
						}
						else
						{
							if (!queueOperationTracker.DatabaseCollectionSize.HasValue && 
								queueOperationTracker.RequiresDatabaseCollectionSize(nameof(queueOperationTracker.TryGetElementAtIndex)) &&
								!ReadSize())
							{
								element = default(T);
								return null; // The collection was loaded
							}

							if (queueOperationTracker.TryGetElementAtIndex(index, out element))
							{
								return true;
							}

							if (queueOperationTracker.Cleared)
							{
								return false;
							}

							// We have to calculate the database index based on the changes in the queue
							var dbIndex = queueOperationTracker.GetDatabaseElementIndex(index);
							if (!dbIndex.HasValue)
							{
								element = default(T);
								return false;
							}

							index = dbIndex.Value;
						}
					}

					var elementByIndex = persister.GetElementByIndex(entry.LoadedKey, index, session, owner);
					if (persister.NotFoundObject == elementByIndex)
					{
						element = default(T);
						return false;
					}

					element = (T) elementByIndex;
					return true;
				}
				Read();
			}

			element = default(T);
			return null;
		}

		/// <summary>
		/// Called by any writer method of the collection interface
		/// </summary>
		protected virtual void Write()
		{
			Initialize(true);
			Dirty();
		}

		internal virtual AbstractQueueOperationTracker CreateQueueOperationTracker() => null;

		internal AbstractQueueOperationTracker QueueOperationTracker
		{
			get => _queueOperationTracker;
			set => _queueOperationTracker = value;
		}

		internal AbstractQueueOperationTracker GetOrCreateQueueOperationTracker()
		{
			if (_queueOperationTracker != null)
			{
				return _queueOperationTracker;
			}

			_queueOperationTracker = CreateQueueOperationTracker();
			return _queueOperationTracker;
		}

		/// <summary>
		/// Queue an addition, delete etc. if the persistent collection supports it
		/// </summary>
		// Since v5.3
		[Obsolete("This method has no more usages in NHibernate and will be removed in a future version.")]
		protected virtual void QueueOperation(IDelayedOperation element)
		{
			if (operationQueue == null)
			{
				operationQueue = new List<IDelayedOperation>(10);
			}
			operationQueue.Add(element);
			dirty = true; //needed so that we remove this collection from the second-level cache
		}

		protected bool QueueAddElement<T>(T element)
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker<T>(nameof(AbstractCollectionQueueOperationTracker<T>.AddElement));
			return queueOperationTracker.AddElement(element);
		}

		protected void QueueRemoveExistingElement<T>(T element, bool? existsInDb)
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker<T>(nameof(AbstractCollectionQueueOperationTracker<T>.RemoveExistingElement), out var wasFlushed);
			queueOperationTracker.RemoveExistingElement(element, wasFlushed ? true : existsInDb);
		}

		protected void QueueRemoveElementAtIndex<T>(int index, T element)
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker<T>(nameof(AbstractCollectionQueueOperationTracker<T>.RemoveElementAtIndex));
			queueOperationTracker.RemoveElementAtIndex(index, element);
		}

		protected void QueueAddElementAtIndex<T>(int index, T element)
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker<T>(nameof(AbstractCollectionQueueOperationTracker<T>.AddElementAtIndex));
			queueOperationTracker.AddElementAtIndex(index, element);
		}

		protected void QueueSetElementAtIndex<T>(int index, T element, T oldElement)
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker<T>(nameof(AbstractCollectionQueueOperationTracker<T>.SetElementAtIndex));
			queueOperationTracker.SetElementAtIndex(index, element, oldElement);
		}

		protected void QueueClearCollection()
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker(nameof(AbstractQueueOperationTracker.ClearCollection), out _);
			queueOperationTracker.ClearCollection();
		}

		protected void QueueAddElementByKey<TKey, TValue>(TKey elementKey, TValue element)
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker<TKey, TValue>(nameof(AbstractMapQueueOperationTracker<TKey, TValue>.AddElementByKey));
			queueOperationTracker.AddElementByKey(elementKey, element);
		}

		protected void QueueSetElementByKey<TKey, TValue>(TKey elementKey, TValue element, TValue oldElement, bool? existsInDb)
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker<TKey, TValue>(nameof(AbstractMapQueueOperationTracker<TKey, TValue>.SetElementByKey));
			queueOperationTracker.SetElementByKey(elementKey, element, oldElement, existsInDb);
		}

		protected bool QueueRemoveElementByKey<TKey, TValue>(TKey elementKey, TValue oldElement, bool? existsInDb)
		{
			var queueOperationTracker = TryFlushAndGetQueueOperationTracker<TKey, TValue>(nameof(AbstractMapQueueOperationTracker<TKey, TValue>.RemoveElementByKey));
			return queueOperationTracker.RemoveElementByKey(elementKey, oldElement, existsInDb);
		}

		private AbstractMapQueueOperationTracker<TKey, TValue> TryFlushAndGetQueueOperationTracker<TKey, TValue>(string operationName)
		{
			return (AbstractMapQueueOperationTracker<TKey, TValue>) TryFlushAndGetQueueOperationTracker(operationName, out _);
		}

		private AbstractCollectionQueueOperationTracker<T> TryFlushAndGetQueueOperationTracker<T>(string operationName)
		{
			return (AbstractCollectionQueueOperationTracker<T>) TryFlushAndGetQueueOperationTracker(operationName, out _);
		}

		private AbstractCollectionQueueOperationTracker<T> TryFlushAndGetQueueOperationTracker<T>(string operationName, out bool wasFlushed)
		{
			return (AbstractCollectionQueueOperationTracker<T>) TryFlushAndGetQueueOperationTracker(operationName, out wasFlushed);
		}

		private AbstractQueueOperationTracker TryFlushAndGetQueueOperationTracker(string operationName, out bool wasFlushed)
		{
			var queueOperationTracker = GetOrCreateQueueOperationTracker();
			if (queueOperationTracker == null)
			{
				throw new InvalidOperationException($"{nameof(CreateQueueOperationTracker)} must return a not null value.");
			}

			if (queueOperationTracker.RequiresFlushing(operationName))
			{
				session.Flush();
				wasFlushed = true;
			}
			else
			{
				wasFlushed = false;
			}

			queueOperationTracker.BeforeOperation(operationName);
			if (!queueOperationTracker.DatabaseCollectionSize.HasValue && queueOperationTracker.RequiresDatabaseCollectionSize(operationName) && !ReadSize())
			{
				throw new InvalidOperationException($"The collection role {Role} must be mapped as extra lazy.");
			}

			dirty = true;  // Needed so that we remove this collection from the second-level cache
			return queueOperationTracker;
		}

		internal bool CanSkipElementExistanceCheck(object element)
		{
			var queryableCollection = (IQueryableCollection) Session.Factory.GetCollectionPersister(Role);
			return
				queryableCollection != null &&
				queryableCollection.ElementType.IsEntityType &&
				!queryableCollection.ElementPersister.EntityMetamodel.OverridesEquals &&
				!element.IsProxy() &&
				!Session.PersistenceContext.IsEntryFor(element) &&
				ForeignKeys.IsTransientFast(queryableCollection.ElementPersister.EntityName, element, Session) == true;
		}

		// Obsolete since v5.2
		/// <summary>
		/// After reading all existing elements from the database,
		/// add the queued elements to the underlying collection.
		/// </summary>
		[Obsolete("Use or override ApplyQueuedOperations instead")]
		protected virtual void PerformQueuedOperations()
		{
			for (int i = 0; i < operationQueue.Count; i++)
			{
				operationQueue[i].Operate();
			}
		}

		/// <summary>
		/// After reading all existing elements from the database, do the queued operations
		/// (adds or removes) on the underlying collection.
		/// </summary>
		public virtual void ApplyQueuedOperations()
		{
			if (operationQueue == null)
				throw new InvalidOperationException("There are no operation queue.");

#pragma warning disable 618
			PerformQueuedOperations();
#pragma warning restore 618

			operationQueue = null;
		}

		public void SetSnapshot(object key, string role, object snapshot)
		{
			this.key = key;
			this.role = role;
			storedSnapshot = snapshot;
		}

		/// <summary>
		/// Clears out any Queued operation.
		/// </summary>
		/// <remarks>
		/// After flushing, clear any "queued" additions, since the
		/// database state is now synchronized with the memory state.
		/// </remarks>
		public virtual void PostAction()
		{
			operationQueue = null;
			_queueOperationTracker?.AfterFlushing();
			cachedSize = -1;
			ClearDirty();
		}

		/// <summary>
		/// Called just before reading any rows from the <see cref="DbDataReader" />
		/// </summary>
		public virtual void BeginRead()
		{
			// override on some subclasses
			initializing = true;
		}

		/// <summary>
		/// Called after reading all rows from the <see cref="DbDataReader" />
		/// </summary>
		/// <remarks>
		/// This should be overridden by sub collections that use temporary collections
		/// to store values read from the db.
		/// </remarks>
		public virtual bool EndRead(ICollectionPersister persister)
		{
			// override on some subclasses
			return AfterInitialize(persister);
		}

		public virtual bool AfterInitialize(ICollectionPersister persister)
		{
			SetInitialized();
			cachedSize = -1;
			return operationQueue == null && _queueOperationTracker == null;
		}

		/// <summary>
		/// Initialize the collection, if possible, wrapping any exceptions
		/// in a runtime exception
		/// </summary>
		/// <param name="writing">currently obsolete</param>
		/// <exception cref="LazyInitializationException">if we cannot initialize</exception>
		protected virtual void Initialize(bool writing)
		{
			if (!initialized)
			{
				if (initializing)
				{
					throw new LazyInitializationException("illegal access to loading collection");
				}
				ThrowLazyInitializationExceptionIfNotConnected();
				session.InitializeCollection(this, writing);
			}
		}

		protected void ThrowLazyInitializationExceptionIfNotConnected()
		{
			if (!IsConnectedToSession)
			{
				ThrowLazyInitializationException("no session or session was closed");
			}
			if (!session.IsConnected)
			{
				ThrowLazyInitializationException("session is disconnected");
			}
		}

		protected void ThrowLazyInitializationException(string message)
		{
			var ownerEntityName = role == null ? "Unavailable" : StringHelper.Qualifier(role);
			throw new LazyInitializationException(ownerEntityName, key, "failed to lazily initialize a collection"
												  + (role == null ? "" : " of role: " + role) + ", " + message);
		}

		/// <summary>
		/// Mark the collection as initialized.
		/// </summary>
		protected virtual void SetInitialized()
		{
			initializing = false;
			initialized = true;
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if the underlying collection is directly
		/// accessible through code.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if we are not guaranteed that the NHibernate collection wrapper
		/// is being used.
		/// </value>
		/// <remarks>
		/// This is typically <see langword="false" /> whenever a transient object that contains a collection is being
		/// associated with an <see cref="ISession" /> through <see cref="ISession.Save(object)" /> or <see cref="ISession.SaveOrUpdate(object)" />.
		/// NHibernate can't guarantee that it will know about all operations that would cause NHibernate's collections
		/// to call <see cref="Read()" /> or <see cref="Write" />.
		/// </remarks>
		public virtual bool IsDirectlyAccessible
		{
			get { return directlyAccessible; }
			protected set { directlyAccessible = value; }
		}

		/// <summary>
		/// Disassociate this collection from the given session.
		/// </summary>
		/// <param name="currentSession"></param>
		/// <returns>true if this was currently associated with the given session</returns>
		public bool UnsetSession(ISessionImplementor currentSession)
		{
			if (currentSession == session)
			{
				session = null;
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Associate the collection with the given session.
		/// </summary>
		/// <param name="session"></param>
		/// <returns>false if the collection was already associated with the session</returns>
		public virtual bool SetCurrentSession(ISessionImplementor session)
		{
			if (session == this.session // NH: added to fix NH-704
				&& session.PersistenceContext.ContainsCollection(this))
			{
				return false;
			}
			else
			{
				if (IsConnectedToSession)
				{
					CollectionEntry ce = session.PersistenceContext.GetCollectionEntry(this);
					if (ce == null)
					{
						throw new HibernateException("Illegal attempt to associate a collection with two open sessions");
					}
					else
					{
						throw new HibernateException("Illegal attempt to associate a collection with two open sessions: "
													 + MessageHelper.CollectionInfoString(ce.LoadedPersister, this, ce.LoadedKey, session));
					}
				}
				else
				{
					this.session = session;
					return true;
				}
			}
		}

		/// <summary>
		/// Gets a <see cref="Boolean"/> indicating if the rows for this collection
		/// need to be recreated in the table.
		/// </summary>
		/// <param name="persister">The <see cref="ICollectionPersister"/> for this Collection.</param>
		/// <returns>
		/// <see langword="false" /> by default since most collections can determine which rows need to be
		/// individually updated/inserted/deleted.  Currently only <see cref="PersistentGenericBag{T}"/>'s for <c>many-to-many</c>
		/// need to be recreated.
		/// </returns>
		public virtual bool NeedsRecreate(ICollectionPersister persister)
		{
			return false;
		}

		/// <summary>
		/// To be called internally by the session, forcing
		/// immediate initialization.
		/// </summary>
		/// <remarks>
		/// This method is similar to <see cref="Initialize" />, except that different exceptions are thrown.
		/// </remarks>
		public virtual void ForceInitialization()
		{
			if (!initialized)
			{
				if (initializing)
				{
					throw new AssertionFailure("force initialize loading collection");
				}
				if (session == null)
				{
					throw new HibernateException("collection is not associated with any session");
				}
				if (!session.IsConnected)
				{
					throw new HibernateException("disconnected session");
				}
				session.InitializeCollection(this, false);
			}
		}

		/// <summary>
		/// Gets the Snapshot from the current session the collection is in.
		/// </summary>
		protected virtual object GetSnapshot()
		{
			return session.PersistenceContext.GetSnapshot(this);
		}

		/// <summary> Is this instance initialized?</summary>
		public bool WasInitialized
		{
			get { return initialized; }
		}

		/// <summary> Does this instance have any "queued" additions?</summary>
		public bool HasQueuedOperations => (operationQueue != null && operationQueue.Count > 0) || _queueOperationTracker?.HasChanges() == true;

		/// <summary></summary>
		public IEnumerable QueuedAdditionIterator
		{
			get
			{
				if (HasQueuedOperations)
				{
					// Use the queue operation tracker when available to get the added elements as AdditionEnumerable
					// does not work correctly when the item is added and then removed
					if (_queueOperationTracker != null)
					{
						return _queueOperationTracker.GetAddedElements();
					}

					return new AdditionEnumerable(this);
				}
				else
				{
					return CollectionHelper.EmptyEnumerable;
				}
			}
		}

		public ICollection GetQueuedOrphans(string entityName)
		{
			if (HasQueuedOperations)
			{
				List<object> additions;
				List<object> removals;

				// Use the queue operation tracker when available to get the orphans as the default logic
				// does not work correctly when readding a transient entity. Removals list should
				// contain only entities that are already in the database.
				if (_queueOperationTracker != null)
				{
					removals = new List<object>(_queueOperationTracker.GetOrphans().Cast<object>());
					additions = new List<object>(_queueOperationTracker.GetAddedElements().Cast<object>());
				}
				else // 6.0 TODO: Remove whole else block
				{
					additions = new List<object>(operationQueue.Count);
					removals = new List<object>(operationQueue.Count);
					for (int i = 0; i < operationQueue.Count; i++)
					{
						var op = operationQueue[i];
						if (op.AddedInstance != null)
						{
							additions.Add(op.AddedInstance);
						}
						if (op.Orphan != null)
						{
							removals.Add(op.Orphan);
						}
					}
				}

				return GetOrphans(removals, additions, entityName, session);
			}

			return CollectionHelper.EmptyCollection;
		}

		/// <summary>
		/// Called before inserting rows, to ensure that any surrogate keys are fully generated
		/// </summary>
		/// <param name="persister"></param>
		public virtual void PreInsert(ICollectionPersister persister) {}

		/// <summary>
		/// Called after inserting a row, to fetch the natively generated id
		/// </summary>
		public virtual void AfterRowInsert(ICollectionPersister persister, object entry, int i, object id) { }

		/// <summary>
		/// Get all "orphaned" elements
		/// </summary>
		public abstract ICollection GetOrphans(object snapshot, string entityName);

		/// <summary> 
		/// Given a collection of entity instances that used to
		/// belong to the collection, and a collection of instances
		/// that currently belong, return a collection of orphans
		/// </summary>
		protected virtual ICollection GetOrphans(ICollection oldElements, ICollection currentElements, string entityName, ISessionImplementor session)
		{
			// short-circuit(s)
			if (currentElements.Count == 0)
			{
				// no new elements, the old list contains only Orphans
				return oldElements;
			}
			if (oldElements.Count == 0)
			{
				// no old elements, so no Orphans neither
				return oldElements;
			}

			if (currentElements.Count == oldElements.Count && currentElements.Cast<object>().SequenceEqual(oldElements.Cast<object>(), ReferenceComparer<object>.Instance))
				return Array.Empty<object>();

			var persister = session.Factory.GetEntityPersister(entityName);
			IType idType = persister.IdentifierType;

			var currentObjects = new HashSet<object>(new TypeComparer(idType, session.Factory));
			foreach (object current in currentElements)
			{
				if (current != null)
				{
					var id = ForeignKeys.GetIdentifier(persister, current);
					if (id != null)
						currentObjects.Add(id);
				}
			}

			List<object> res = new List<object>();
			// oldElements may contain new elements (one case when session.Save is called on new object with collection filled with new objects)
			foreach (object old in oldElements)
			{
				if (old != null)
				{
					var id = ForeignKeys.GetIdentifier(persister, old);
					if (id != null)
					{
						if (!currentObjects.Contains(id))
						{
							res.Add(old);
						}
					}
				}
			}

			return res;
		}

		// Since 5.3
		/// <summary> 
		/// Given a collection of entity instances that used to
		/// belong to the collection, and a collection of instances
		/// that currently belong, return a collection of orphans
		/// </summary>
		[Obsolete("This method has no more usages and will be removed in a future version")]
		protected virtual Task<ICollection> GetOrphansAsync(ICollection oldElements, ICollection currentElements, string entityName, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<ICollection>(cancellationToken);
			}

			return Task.FromResult(GetOrphans(oldElements, currentElements, entityName, session));
		}

		// Since 5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public void IdentityRemove(IList list, object obj, string entityName, ISessionImplementor session)
		{
			if (obj != null && ForeignKeys.IsNotTransientSlow(entityName, obj, session))
			{
				IType idType = session.Factory.GetEntityPersister(entityName).IdentifierType;

				object idOfCurrent = ForeignKeys.GetEntityIdentifierIfNotUnsaved(entityName, obj, session);
				List<object> toRemove = new List<object>(list.Count);
				foreach (object current in list)
				{
					if (current == null)
					{
						continue;
					}
					object idOfOld = ForeignKeys.GetEntityIdentifierIfNotUnsaved(entityName, current, session);
					if (idType.IsEqual(idOfCurrent, idOfOld, session.Factory))
					{
						toRemove.Add(current);
					}
				}
				foreach (object ro in toRemove)
				{
					list.Remove(ro);
				}
			}
		}

		public virtual object GetIdentifier(object entry, int i)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Disassemble the collection, ready for the cache
		/// </summary>
		/// <param name="persister"></param>
		/// <returns></returns>
		public abstract object Disassemble(ICollectionPersister persister);

		/// <summary>
		/// Is this the wrapper for the given underlying collection instance?
		/// </summary>
		/// <param name="collection"></param>
		/// <returns></returns>
		public abstract bool IsWrapper(object collection);

		/// <summary>
		/// Does an element exist at this entry in the collection?
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <returns></returns>
		public abstract bool EntryExists(object entry, int i);

		/// <summary>
		/// Get all the elements that need deleting
		/// </summary>
		public abstract IEnumerable GetDeletes(ICollectionPersister persister, bool indexIsFormula);

		public abstract bool IsSnapshotEmpty(object snapshot);

		public abstract IEnumerable Entries(ICollectionPersister persister);

		public abstract object GetSnapshot(ICollectionPersister persister);

		public abstract bool EqualsSnapshot(ICollectionPersister persister);

		public abstract object GetElement(object entry);

		/// <summary>
		/// Read the state of the collection from a disassembled cached value.
		/// </summary>
		/// <param name="persister"></param>
		/// <param name="disassembled"></param>
		/// <param name="owner"></param>
		public abstract void InitializeFromCache(ICollectionPersister persister, object disassembled, object owner);

		/// <summary>
		/// Do we need to update this element?
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public abstract bool NeedsUpdating(object entry, int i, IType elemType);

		/// <summary>
		/// Reads the row from the <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="reader">The DbDataReader that contains the value of the Identifier</param>
		/// <param name="role">The persister for this Collection.</param>
		/// <param name="descriptor">The descriptor providing result set column names</param>
		/// <param name="owner">The owner of this Collection.</param>
		/// <returns>The object that was contained in the row.</returns>
		public abstract object ReadFrom(DbDataReader reader, ICollectionPersister role, ICollectionAliases descriptor,
										object owner);

		public abstract object GetSnapshotElement(object entry, int i);

		/// <summary>
		/// Do we need to insert this element?
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="i"></param>
		/// <param name="elemType"></param>
		/// <returns></returns>
		public abstract bool NeedsInserting(object entry, int i, IType elemType);

		/// <summary>
		/// Get the index of the given collection entry
		/// </summary>
		public abstract object GetIndex(object entry, int i, ICollectionPersister persister);

		/// <summary>
		/// Called before any elements are read into the collection,
		/// allowing appropriate initializations to occur.
		/// </summary>
		/// <param name="persister">The underlying collection persister. </param>
		/// <param name="anticipatedSize">The anticipated size of the collection after initialization is complete. </param>
		public abstract void BeforeInitialize(ICollectionPersister persister, int anticipatedSize);

		// Since 5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public Task<ICollection> GetQueuedOrphansAsync(string entityName, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<ICollection>(cancellationToken);
			}

			try
			{
				return Task.FromResult(GetQueuedOrphans(entityName));
			}
			catch (Exception ex)
			{
				return Task.FromException<ICollection>(ex);
			}
		}
		
		// Since 5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
		public abstract Task<ICollection> GetOrphansAsync(object snapshot, string entityName, CancellationToken cancellationToken);

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
