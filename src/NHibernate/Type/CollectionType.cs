using System;
using System.Collections;
using System.Data.Common;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.SqlTypes;
using NHibernate.Util;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Impl;

namespace NHibernate.Type
{
	/// <summary>
	/// The base class for an <see cref="IType"/> that maps collections
	/// to the database.
	/// </summary>
	[Serializable]
	public abstract partial class CollectionType : AbstractType, IAssociationType
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(CollectionType));

		private static readonly object NotNullCollection = new object(); // place holder
		public static readonly object UnfetchedCollection = new object(); // place holder

		private readonly string role;
		private readonly string foreignKeyPropertyName;

		private static readonly SqlType[] NoSqlTypes = {};

		/// <summary>
		/// Initializes a new instance of a <see cref="CollectionType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="foreignKeyPropertyName">
		/// The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.
		/// </param>
		protected CollectionType(string role, string foreignKeyPropertyName)
		{
			this.role = role;
			this.foreignKeyPropertyName = foreignKeyPropertyName;
		}

		public virtual string Role
		{
			get { return role; }
		}

		public override bool IsCollectionType
		{
			get { return true; }
		}

		public override bool IsEqual(object x, object y)
		{
			return x == y || 
				(x is IPersistentCollection && ((IPersistentCollection)x).IsWrapper(y)) || 
				(y is IPersistentCollection && ((IPersistentCollection)y).IsWrapper(x));
		}

		public override int GetHashCode(object x)
		{
			throw new InvalidOperationException("cannot perform lookups on collections");
		}

		/// <summary> 
		/// Instantiate an uninitialized collection wrapper or holder. Callers MUST add the holder to the
		/// persistence context! 
		/// </summary>
		/// <param name="session">The session from which the request is originating. </param>
		/// <param name="persister">The underlying collection persister (metadata) </param>
		/// <param name="key">The owner key. </param>
		/// <returns> The instantiated collection. </returns>
		public abstract IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key);

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, new string[] { name }, session, owner);
		}

		public override object NullSafeGet(DbDataReader rs, string[] name, ISessionImplementor session, object owner)
		{
			return ResolveIdentifier(null, session, owner);
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			// NOOP
		}

		public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
		}

		public override SqlType[] SqlTypes(IMapping session)
		{
			return NoSqlTypes;
		}

		public override int GetColumnSpan(IMapping session)
		{
			return 0;
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			if (value == null)
			{
				return "null";
			}
			else if (!NHibernateUtil.IsInitialized(value))
			{
				return "<uninitialized>";
			}
			else
			{
				return RenderLoggableString(value, factory);
			}
		}

		public override object DeepCopy(object value, ISessionFactoryImplementor factory)
		{
			return value;
		}

		public override string Name
		{
			get { return ReturnedClass.FullName + '(' + Role + ')'; }
		}

		public override bool IsMutable
		{
			get { return false; }
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			//remember the uk value

			//This solution would allow us to eliminate the owner arg to disassemble(), but
			//what if the collection was null, and then later had elements added? seems unsafe
			//session.getPersistenceContext().getCollectionEntry( (PersistentCollection) value ).getKey();

			object key = GetKeyOfOwner(owner, session);
			if (key == null)
			{
				return null;
			}
			else
			{
				return GetPersister(session).KeyType.Disassemble(key, session, owner);
			}
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			//we must use the "remembered" uk value, since it is 
			//not available from the EntityEntry during assembly
			if (cached == null)
			{
				return null;
			}
			else
			{
				object key = GetPersister(session).KeyType.Assemble(cached, session, owner);
				return ResolveKey(key, session, owner);
			}
		}

		private bool IsOwnerVersioned(ISessionImplementor session)
		{
			return GetPersister(session).OwnerEntityPersister.IsVersioned;
		}

		// Get our underlying collection persister (using the session to access the factory). 
		private ICollectionPersister GetPersister(ISessionImplementor session)
		{
			return session.Factory.GetCollectionPersister(role);
		}

		public override bool IsDirty(object old, object current, ISessionImplementor session)
		{
			// collections don't dirty an unversioned parent entity

			// TODO: I don't like this implementation; it would be better if this was handled by SearchForDirtyCollections();
			return IsOwnerVersioned(session) && base.IsDirty(old, current, session);
		}

		/// <summary> 
		/// Wrap the naked collection instance in a wrapper, or instantiate a
		/// holder. Callers <b>MUST</b> add the holder to the persistence context!
		///  </summary>
		/// <param name="session">The session from which the request is originating. </param>
		/// <param name="collection">The bare collection to be wrapped. </param>
		/// <returns>
		/// A subclass of <see cref="IPersistentCollection"/> that wraps the non NHibernate collection.
		/// </returns>
		public abstract IPersistentCollection Wrap(ISessionImplementor session, object collection);

		// Note: return true because this type is castable to IAssociationType. Not because
		// all collections are associations.
		public override bool IsAssociationType
		{
			get { return true; }
		}

		public virtual ForeignKeyDirection ForeignKeyDirection
		{
			get { return ForeignKeyDirection.ForeignKeyToParent; }
		}

		public override object Hydrate(DbDataReader rs, string[] name, ISessionImplementor session, object owner)
		{
			// can't just return null here, since that would
			// cause an owning component to become null
			return NotNullCollection;
		}

		public override object ResolveIdentifier(object key, ISessionImplementor session, object owner)
		{
			return ResolveKey(GetKeyOfOwner(owner, session), session, owner);
		}

		private object ResolveKey(object key, ISessionImplementor session, object owner)
		{
			return key == null ? null : GetCollection(key, session, owner);
		}

		public object GetCollection(object key, ISessionImplementor session, object owner)
		{
			ICollectionPersister persister = GetPersister(session);
			IPersistenceContext persistenceContext = session.PersistenceContext;

			// check if collection is currently being loaded
			IPersistentCollection collection = persistenceContext.LoadContexts.LocateLoadingCollection(persister, key);
			if (collection == null)
			{
				// check if it is already completely loaded, but unowned
				collection = persistenceContext.UseUnownedCollection(new CollectionKey(persister, key));
				if (collection == null)
				{
					// create a new collection wrapper, to be initialized later
					collection = Instantiate(session, persister, key);
					collection.Owner = owner;

					persistenceContext.AddUninitializedCollection(persister, collection, key);

					// some collections are not lazy:
					if (InitializeImmediately())
					{
						session.InitializeCollection(collection, false);
					}
					else if (!persister.IsLazy)
					{
						persistenceContext.AddNonLazyCollection(collection);
					}

					if (HasHolder())
					{
						session.PersistenceContext.AddCollectionHolder(collection);
					}
				}

				if (log.IsDebugEnabled)
				{
					log.Debug("Created collection wrapper: " + MessageHelper.CollectionInfoString(persister, collection, key, session));
				}
			}
			collection.Owner = owner;
			return collection.GetValue();
		}

		public override object SemiResolve(object value, ISessionImplementor session, object owner)
		{
			throw new NotSupportedException("collection mappings may not form part of a property-ref");
		}

		public virtual bool IsArrayType
		{
			get { return false; }
		}

		public bool UseLHSPrimaryKey
		{
			get { return string.IsNullOrEmpty(foreignKeyPropertyName); }
		}

		public IJoinable GetAssociatedJoinable(ISessionFactoryImplementor factory)
		{
			return (IJoinable) factory.GetCollectionPersister(role);
		}

		public string[] GetReferencedColumns(ISessionFactoryImplementor factory)
		{
			//I really, really don't like the fact that a Type now knows about column mappings!
			//bad seperation of concerns ... could we move this somehow to Joinable interface??
			return GetAssociatedJoinable(factory).KeyColumnNames;
		}

		public string GetAssociatedEntityName(ISessionFactoryImplementor factory)
		{
			try
			{
				IQueryableCollection collectionPersister = (IQueryableCollection)factory.GetCollectionPersister(role);

				if (!collectionPersister.ElementType.IsEntityType)
				{
					throw new MappingException("collection was not an association: " + collectionPersister.Role);
				}

				return collectionPersister.ElementPersister.EntityName;
			}
			catch (InvalidCastException cce)
			{
				throw new MappingException("collection role is not queryable " + role, cce);
			}
		}

		public virtual object InstantiateResult(object original)
		{
			return Instantiate(-1);
		}

		public override object Replace(object original, object target, ISessionImplementor session, object owner,
									   IDictionary copyCache)
		{
			if (original == null)
			{
				return null;
			}

			if (!NHibernateUtil.IsInitialized(original))
			{
				return target;
			}

			object result = target == null || target == original
								? InstantiateResult(original)
								: target;

			//for arrays, replaceElements() may return a different reference, since
			//the array length might not match
			result = ReplaceElements(original, result, owner, copyCache, session);

			if (original == target)
			{
				//get the elements back into the target
				//TODO: this is a little inefficient, don't need to do a whole
				//	  deep replaceElements() call
				ReplaceElements(result, target, owner, copyCache, session);
				result = target;
			}

			return result;
		}

		public virtual object ReplaceElements(object original, object target, object owner, IDictionary copyCache, ISessionImplementor session)
		{
			var elemType = GetElementType(session.Factory);
			var targetPc = target as IPersistentCollection;
			var originalPc = original as IPersistentCollection;
			var iterOriginal = (IEnumerable)original;
			var clearTargetsDirtyFlag = ShouldTargetsDirtyFlagBeCleared(targetPc, originalPc, iterOriginal);

			// copy elements into newly empty target collection
			Clear(target);
			foreach (var obj in iterOriginal)
			{
				Add(target, elemType.Replace(obj, null, session, owner, copyCache));
			}

			if(clearTargetsDirtyFlag)
			{
				targetPc.ClearDirty();
			}

			return target;
		}

		internal bool ShouldTargetsDirtyFlagBeCleared(IPersistentCollection targetPc, IPersistentCollection originalPc, IEnumerable original)
		{
			if (targetPc == null)
				return false;

			if (originalPc == null)
			{
				if (!targetPc.IsDirty && AreCollectionElementsEqual(original, (IEnumerable)targetPc))
				{
					return true;
				}
			}
			else
			{
				if (!originalPc.IsDirty)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual bool AreCollectionElementsEqual(IEnumerable original, IEnumerable target)
		{ 
			return original.Cast<object>().SequenceEqual(target.Cast<object>());
		}

		public IType GetElementType(ISessionFactoryImplementor factory)
		{
			return factory.GetCollectionPersister(Role).ElementType;
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + Role + ')';
		}


		#region Methods added in NH

		protected virtual void Clear(object collection)
		{
			throw new NotImplementedException(
				"CollectionType.Clear was not overridden for type "
				+ GetType().FullName);
		}

		protected virtual void Add(object collection, object element)
		{
			throw new NotImplementedException(
				"CollectionType.Add was not overridden for type "
				+ GetType().FullName);
		}

		#endregion

		public string LHSPropertyName
		{
			get { return foreignKeyPropertyName; }
		}

		public string RHSUniqueKeyPropertyName
		{
			get { return null; }
		}

		/// <summary>
		/// We always need to dirty check the collection because we sometimes 
		/// need to increment version number of owner and also because of 
		/// how assemble/disassemble is implemented for uks
		/// </summary>
		public bool IsAlwaysDirtyChecked
		{
			get { return true; }
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return IsDirty(old, current, session);
		}

		public override bool IsModified(object oldHydratedState, object currentState, bool[] checkable,
										ISessionImplementor session)
		{
			return false;
		}

		/// <summary>
		/// Get the key value from the owning entity instance.
		/// </summary>
		public object GetKeyOfOwner(object owner, ISessionImplementor session)
		{
			EntityEntry entityEntry = session.PersistenceContext.GetEntry(owner);
			if (entityEntry == null)
				return null; // This just handles a particular case of component
			// projection, perhaps get rid of it and throw an exception

			return entityEntry.Id;
		}

		/// <summary> 
		/// Get the id value from the owning entity key, usually the same as the key, but might be some
		/// other property, in the case of property-ref 
		/// </summary>
		/// <param name="key">The collection owner key </param>
		/// <param name="session">The session from which the request is originating. </param>
		/// <returns> 
		/// The collection owner's id, if it can be obtained from the key;
		/// otherwise, null is returned
		/// </returns>
		public virtual object GetIdOfOwnerOrNull(object key, ISessionImplementor session)
		{
			object ownerId = null;
			if (foreignKeyPropertyName == null)
			{
				ownerId = key;
			}
			else
			{
				IType keyType = GetPersister(session).KeyType;
				IEntityPersister ownerPersister = GetPersister(session).OwnerEntityPersister;
				// TODO: Fix this so it will work for non-POJO entity mode
				System.Type ownerMappedClass = ownerPersister.MappedClass;
				if (ownerMappedClass.IsAssignableFrom(keyType.ReturnedClass) && keyType.ReturnedClass.IsInstanceOfType(key))
				{
					// the key is the owning entity itself, so get the ID from the key
					ownerId = ownerPersister.GetIdentifier(key);
				}
				else
				{
					// TODO: check if key contains the owner ID
				}
			}
			return ownerId;
		}

		/// <summary> 
		/// Instantiate an empty instance of the "underlying" collection (not a wrapper),
		/// but with the given anticipated size (i.e. accounting for initial capacity
		/// and perhaps load factor).
		/// </summary>
		/// <param name="anticipatedSize">
		/// The anticipated size of the instantiated collection after we are done populating it.
		/// </param>
		/// <returns> A newly instantiated collection to be wrapped. </returns>
		public abstract object Instantiate(int anticipatedSize);

		public string GetOnCondition(string alias, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			return GetAssociatedJoinable(factory).FilterFragment(alias, enabledFilters);
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			return ArrayHelper.EmptyBoolArray;
		}

		public override int Compare(object x, object y)
		{
			return 0; // collections cannot be compared
		}

		public virtual bool Contains(object collection, object childObject, ISessionImplementor session)
		{
			// we do not have to worry about queued additions to uninitialized
			// collections, since they can only occur for inverse collections!
			IEnumerable elems = GetElementsIterator(collection, session);
			foreach (object elem in elems)
			{
				object element = elem;
				// worrying about proxies is perhaps a little bit of overkill here...
				
				if (element.IsProxy())
				{
					INHibernateProxy proxy = element as INHibernateProxy; 
					
					ILazyInitializer li = proxy.HibernateLazyInitializer;
					if (!li.IsUninitialized)
						element = li.GetImplementation();
				}

				if (element == childObject)
					return true;				
			}
			return false;
		}

		/// <summary> 
		/// Get an iterator over the element set of the collection, which may not yet be wrapped 
		/// </summary>
		/// <param name="collection">The collection to be iterated </param>
		/// <param name="session">The session from which the request is originating. </param>
		/// <returns> The iterator. </returns>
		public virtual IEnumerable GetElementsIterator(object collection, ISessionImplementor session)
		{
			return GetElementsIterator(collection);
		}

		/// <summary> 
		/// Get an iterator over the element set of the collection in POCO mode 
		/// </summary>
		/// <param name="collection">The collection to be iterated </param>
		/// <returns> The iterator. </returns>
		public virtual IEnumerable GetElementsIterator(object collection)
		{
			return ((IEnumerable)collection);
		}

		public virtual bool HasHolder()
		{
			return false;// entityMode == EntityMode.DOM4J;
		}

		protected internal virtual bool InitializeImmediately()
		{
			return false;// entityMode == EntityMode.DOM4J;
		}

		public virtual object IndexOf(object collection, object element)
		{
			throw new NotSupportedException("generic collections don't have indexes");
		}

		protected internal virtual string RenderLoggableString(object value, ISessionFactoryImplementor factory)
		{
			IList list = new List<object>();
			IType elemType = GetElementType(factory);
			IEnumerable iter = GetElementsIterator(value);
			foreach (object o in iter)
				list.Add(elemType.ToLoggableString(o, factory));

			return CollectionPrinter.ToString(list);
		}
	}
}
