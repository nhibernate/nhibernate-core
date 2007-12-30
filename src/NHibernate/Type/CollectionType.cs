using System;
using System.Collections;
using System.Data;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.SqlTypes;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Type
{
	/// <summary>
	/// The base class for an <see cref="IType"/> that maps collections
	/// to the database.
	/// </summary>
	[Serializable]
	public abstract class CollectionType : AbstractType, IAssociationType
	{
		private static readonly object NotNullCollection = new object();
		public static readonly object UnfetchedCollection = new object();

		private readonly string role;
		private readonly string foreignKeyPropertyName;

		private static readonly SqlType[] NoSqlTypes = {};
		private readonly bool isEmbeddedInXML;

		/// <summary>
		/// Initializes a new instance of a <see cref="CollectionType"/> class for
		/// a specific role.
		/// </summary>
		/// <param name="role">The role the persistent collection is in.</param>
		/// <param name="foreignKeyPropertyName">The name of the property in the
		/// owner object containing the collection ID, or <see langword="null" /> if it is
		/// the primary key.</param>
		protected CollectionType(string role, string foreignKeyPropertyName)
			: this(role, foreignKeyPropertyName, false) {} // TODO NH: Remove this ctor

		public CollectionType(string role, string foreignKeyPropertyName, bool isEmbeddedInXML)
		{
			this.role = role;
			this.foreignKeyPropertyName = foreignKeyPropertyName;
			this.isEmbeddedInXML = isEmbeddedInXML;
		}

		public virtual string Role
		{
			get { return role; }
		}

		public override bool IsCollectionType
		{
			get { return true; }
		}

		public override sealed bool Equals(object x, object y)
		{
			return x == y ||
			       (x is IPersistentCollection && ((IPersistentCollection) x).IsWrapper(y)) ||
			       (y is IPersistentCollection && ((IPersistentCollection) y).IsWrapper(x));
		}

		public override int GetHashCode(object x, ISessionFactoryImplementor factory)
		{
			throw new InvalidOperationException("cannot perform lookups on collections");
		}

		public abstract IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister);

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
		{
			throw new AssertionFailure("bug in CollectionType");
		}

		public override object NullSafeGet(IDataReader rs, string[] name, ISessionImplementor session, object owner)
		{
			return ResolveIdentifier(Hydrate(rs, name, session, owner), session, owner);
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			// NOOP
		}

		public override void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session)
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

			IType elemType = GetElementType(factory);
			if (NHibernateUtil.IsInitialized(value))
			{
				IList list = new ArrayList();
				ICollection elements = GetElementsCollection(value);
				foreach (object element in elements)
				{
					list.Add(elemType.ToLoggableString(element, factory));
				}
				return CollectionPrinter.ToString(list);
			}
			else
			{
				return "uninitialized";
			}
		}

		public override object FromString(string xml)
		{
			throw new NotSupportedException();
		}


		public override object DeepCopy(object value)
		{
			return value;
		}

		public override string Name
		{
			get { return ReturnedClass.Name; }
		}


		/// <summary>
		/// Returns a reference to the elements in the collection.  
		/// </summary>
		/// <param name="collection">The object that holds the ICollection.</param>
		/// <returns>An ICollection of the Elements(classes) in the Collection.</returns>
		/// <remarks>
		/// By default the parameter <c>collection</c> is just cast to an ICollection.  Collections
		/// such as Maps and Sets should override this so that the Elements are returned - not a
		/// DictionaryEntry.
		/// </remarks>
		public virtual ICollection GetElementsCollection(object collection)
		{
			return ((ICollection) collection);
		}

		public override bool IsMutable
		{
			get { return false; }
		}

		public override object Disassemble(object value, ISessionImplementor session)
		{
			return null;
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			//NH Different behavior
			object id = session.GetContextEntityIdentifier(owner);
			if (id == null)
			{
				throw new AssertionFailure("owner id unknown when re-assembling collection reference");
			}
			return ResolveIdentifier(id, session, owner);
		}

		private bool IsOwnerVersioned(ISessionImplementor session)
		{
			return GetPersister(session).OwnerEntityPersister.IsVersioned;
		}

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

		public override bool HasNiceEquals
		{
			get { return false; }
		}

		/// <summary>
		/// Wraps a collection from System.Collections or Iesi.Collections inside one of the 
		/// NHibernate collections.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> for the collection to be a part of.</param>
		/// <param name="collection">The unwrapped collection.</param>
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

		public override object Hydrate(IDataReader rs, string[] name, ISessionImplementor session, object owner)
		{
			return session.GetContextEntityIdentifier(owner);
		}

		public override object ResolveIdentifier(object key, ISessionImplementor session, object owner)
		{
			return key == null ? null : ResolveKey(GetKeyOfOwner(owner, session), session, owner);
		}

		private object ResolveKey(object key, ISessionImplementor session, object owner)
		{
			return key == null ? null : GetCollection(key, session, owner);
		}

		public object GetCollection(object key, ISessionImplementor session, object owner)
		{
			ICollectionPersister persister = GetPersister(session);
			IPersistenceContext persistenceContext = session.PersistenceContext;
			EntityMode entityMode = session.EntityMode;

			if (entityMode == EntityMode.Xml && !isEmbeddedInXML)
			{
				return UnfetchedCollection;
			}

			// check if collection is currently being loaded
			IPersistentCollection collection = persistenceContext.LoadContexts.LocateLoadingCollection(persister, key);
			if (collection == null)
			{
				// check if it is already completely loaded, but unowned
				collection = persistenceContext.UseUnownedCollection(new CollectionKey(persister, key));
				if (collection == null)
				{
					// create a new collection wrapper, to be initialized later
					collection = Instantiate(session, persister);
					collection.Owner = owner;

					persistenceContext.AddUninitializedCollection(persister, collection, key);

					// NH Different behavior
					if (IsArrayType)
					{
						session.InitializeCollection(collection, false);
						persistenceContext.AddCollectionHolder(collection);
					}
					else if (!persister.IsLazy)
					{
						persistenceContext.AddNonLazyCollection(collection);
					}
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
			get { return foreignKeyPropertyName == null; }
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

		public System.Type GetAssociatedClass(ISessionFactoryImplementor factory)
		{
			try
			{
				IQueryableCollection collectionPersister = (IQueryableCollection) factory.GetCollectionPersister(role);
				if (!collectionPersister.ElementType.IsEntityType)
				{
					throw new MappingException(string.Format("collection was not an association: {0}", collectionPersister.Role));
				}
				return collectionPersister.ElementPersister.MappedClass;
			}
			catch (InvalidCastException ice)
			{
				throw new MappingException("collection role is not queryable " + role, ice);
			}
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
			return Instantiate();
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

		public virtual object ReplaceElements(object original, object target, object owner, IDictionary copyCache,
		                                      ISessionImplementor session)
		{
			object result = target;
			Clear(result);

			// copy elements into newly empty target collection
			ICollectionPersister cp = session.Factory.GetCollectionPersister(role);
			foreach (object obj in (IEnumerable) original)
			{
				Add(result, CopyElement(cp, obj, session, owner, copyCache));
			}

			return result;
		}

		public IType GetElementType(ISessionFactoryImplementor factory)
		{
			return factory.GetCollectionPersister(Role).ElementType;
		}

		public override string ToString()
		{
			return base.ToString() + " for " + Role;
		}

		// Methods added in NH

		protected virtual void Clear(object collection)
		{
			throw new NotImplementedException(
				"CollectionType.Clear was not overriden for type "
				+ GetType().FullName);
		}

		protected virtual void Add(object collection, object element)
		{
			throw new NotImplementedException(
				"CollectionType.Add was not overriden for type "
				+ GetType().FullName);
		}

		protected virtual object CopyElement(ICollectionPersister persister, object element, ISessionImplementor session,
		                                     object owner, IDictionary copiedAlready)
		{
			return persister.ElementType.Replace(element, null, session, owner, copiedAlready);
		}

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
		/// need to incremement version number of owner and also because of 
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
		/// Get the key value from the owning entity instance, usually the identifier, but might be some
		/// other unique key, in the case of property-ref
		/// </summary>
		public object GetKeyOfOwner(object owner, ISessionImplementor session)
		{
			EntityEntry entityEntry = session.PersistenceContext.GetEntry(owner);
			if (entityEntry == null)
			{
				// This just handles a particular case of component
				// projection, perhaps get rid of it and throw an exception
				return null;
			}

			if (foreignKeyPropertyName == null)
			{
				return entityEntry.Id;
			}
			else
			{
				// TODO: at the point where we are resolving collection references, we don't
				// know if the uk value has been resolved (depends if it was earlier or
				// later in the mapping document) - now, we could try and use e.getStatus()
				// to decide to semiResolve(), trouble is that initializeEntity() reuses
				// the same array for resolved and hydrated values
				object id = entityEntry.GetLoadedValue(foreignKeyPropertyName);

				// NOTE VERY HACKISH WORKAROUND!!
				IType keyType = GetPersister(session).KeyType;
				if (!keyType.ReturnedClass.IsInstanceOfType(id))
				{
					id = keyType.SemiResolve(
						entityEntry.GetLoadedValue(foreignKeyPropertyName),
						session,
						owner
						);
				}

				return id;
			}
		}

		/// <summary>
		/// Instantiate an empty instance of the "underlying" collection (not a wrapper)
		/// </summary>
		public abstract object Instantiate();

		public string GetOnCondition(string alias, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			return GetAssociatedJoinable(factory).FilterFragment(alias, enabledFilters);
		}

		public bool IsEmbeddedInXML
		{
			get { return isEmbeddedInXML; }
		}
	}
}
