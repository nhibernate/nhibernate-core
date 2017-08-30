using System;
using System.Collections;
using System.Data.Common;
using System.Text;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Type
{
	/// <summary>
	/// A reference to an entity class
	/// </summary>
	[Serializable]
	public abstract partial class EntityType : AbstractType, IAssociationType
	{
		protected readonly string uniqueKeyPropertyName;
		private readonly bool eager;
		private readonly string associatedEntityName;
		private readonly bool unwrapProxy;
		private System.Type returnedClass;

		/// <summary> Constructs the requested entity type mapping. </summary>
		/// <param name="entityName">The name of the associated entity. </param>
		/// <param name="uniqueKeyPropertyName">
		/// The property-ref name, or null if we
		/// reference the PK of the associated entity.
		/// </param>
		/// <param name="eager">Is eager fetching enabled. </param>
		/// <param name="unwrapProxy">
		/// Is unwrapping of proxies allowed for this association; unwrapping
		/// says to return the "implementation target" of lazy proxies; typically only possible
		/// with lazy="no-proxy".
		/// </param>
		protected internal EntityType(string entityName, string uniqueKeyPropertyName, bool eager, bool unwrapProxy)
		{
			associatedEntityName = entityName;
			this.uniqueKeyPropertyName = uniqueKeyPropertyName;
			this.eager = eager;
			this.unwrapProxy = unwrapProxy;
		}

		/// <summary> Explicitly, an entity type is an entity type </summary>
		/// <value> True. </value>
		public override sealed bool IsEntityType
		{
			get { return true; }
		}

		public override bool IsEqual(object x, object y, ISessionFactoryImplementor factory)
		{
			IEntityPersister persister = factory.GetEntityPersister(associatedEntityName);
			if (!persister.CanExtractIdOutOfEntity)
			{
				return base.IsEqual(x, y);
			}

			object xid;

			if (x.IsProxy())
			{
				INHibernateProxy proxy = x as INHibernateProxy;
				xid = proxy.HibernateLazyInitializer.Identifier;
			}
			else
			{
				xid = persister.GetIdentifier(x);
			}

			object yid;

			if (y.IsProxy())
			{
				INHibernateProxy proxy = y as INHibernateProxy;
				yid = proxy.HibernateLazyInitializer.Identifier;
			}
			else
			{
				yid = persister.GetIdentifier(y);
			}

			return persister.IdentifierType.IsEqual(xid, yid, factory);
		}

		public virtual bool IsNull(object owner, ISessionImplementor session)
		{
			return false;
		}

		/// <summary> Two entities are considered the same when their instances are the same. </summary>
		/// <param name="x">One entity instance </param>
		/// <param name="y">Another entity instance </param>
		/// <returns> True if x == y; false otherwise. </returns>
		public override bool IsSame(object x, object y)
		{
			return ReferenceEquals(x, y);
		}

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, new string[] {name}, session, owner);
		}

		/// <summary> 
		/// This returns the wrong class for an entity with a proxy, or for a named
		/// entity.  Theoretically it should return the proxy class, but it doesn't.
		/// <p/>
		/// The problem here is that we do not necessarily have a ref to the associated
		/// entity persister (nor to the session factory, to look it up) which is really
		/// needed to "do the right thing" here...
		///  </summary>
		override public System.Type ReturnedClass
		{
			get
			{
				if (returnedClass == null)
				{
					returnedClass = DetermineAssociatedEntityClass();
				}
				return returnedClass;
			}
		}

		/// <summary> 
		/// Get the identifier value of an instance or proxy.
		/// <p/>
		/// Intended only for loggin purposes!!!
		/// </summary>
		/// <param name="obj">The object from which to extract the identifier.</param>
		/// <param name="persister">The entity persister </param>
		/// <returns> The extracted identifier. </returns>
		private static object GetIdentifier(object obj, IEntityPersister persister)
		{
			if (obj.IsProxy())
			{
				INHibernateProxy proxy = obj as INHibernateProxy;
				ILazyInitializer li = proxy.HibernateLazyInitializer;

				return li.Identifier;
			}
			else
			{
				return persister.GetIdentifier(obj);
			}
		}

		protected internal object GetIdentifier(object value, ISessionImplementor session)
		{
			return ForeignKeys.GetEntityIdentifierIfNotUnsaved(GetAssociatedEntityName(), value, session); //tolerates nulls
		}

		protected internal object GetReferenceValue(object value, ISessionImplementor session)
		{
			if (value == null)
			{
				return null;
			}
			else if (IsReferenceToPrimaryKey)
			{
				return ForeignKeys.GetEntityIdentifierIfNotUnsaved(GetAssociatedEntityName(), value, session); //tolerates nulls
			}
			else
			{
				IEntityPersister entityPersister = session.Factory.GetEntityPersister(GetAssociatedEntityName());
				object propertyValue = entityPersister.GetPropertyValue(value, uniqueKeyPropertyName);

				// We now have the value of the property-ref we reference.  However,
				// we need to dig a little deeper, as that property might also be
				// an entity type, in which case we need to resolve its identitifier
				IType type = entityPersister.GetPropertyType(uniqueKeyPropertyName);
				if (type.IsEntityType)
				{
					propertyValue = ((EntityType) type).GetReferenceValue(propertyValue, session);
				}

				return propertyValue;
			}
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			if (value == null)
			{
				return "null";
			}

			IEntityPersister persister = factory.GetEntityPersister(associatedEntityName);
			StringBuilder result = new StringBuilder().Append(associatedEntityName);

			if (persister.HasIdentifierProperty)
			{
				var id = GetIdentifier(value, persister);

				result.Append('#').Append(persister.IdentifierType.ToLoggableString(id, factory));
			}

			return result.ToString();
		}

		public override string Name
		{
			get { return associatedEntityName; }
		}

		public override object DeepCopy(object value, ISessionFactoryImplementor factory)
		{
			return value; //special case ... this is the leaf of the containment graph, even though not immutable
		}

		public override bool IsMutable
		{
			get { return false; }
		}

		public abstract bool IsOneToOne { get; }

		public virtual bool IsLogicalOneToOne()
		{
			return IsOneToOne;
		}

		public override object Replace(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache)
		{
			if (original == null)
			{
				return null;
			}
			object cached = copyCache[original];
			if (cached != null)
			{
				return cached;
			}
			else
			{
				if (original == target)
				{
					return target;
				}
				if (session.GetContextEntityIdentifier(original) == null && ForeignKeys.IsTransientFast(associatedEntityName, original, session).GetValueOrDefault())
				{
					object copy = session.Factory.GetEntityPersister(associatedEntityName).Instantiate(null);
					//TODO: should this be Session.instantiate(Persister, ...)?
					copyCache.Add(original, copy);
					return copy;
				}
				else
				{
					object id = GetReferenceValue(original, session);
					if (id == null)
					{
						throw new AssertionFailure("non-transient entity has a null id");
					}
					id = GetIdentifierOrUniqueKeyType(session.Factory).Replace(id, null, session, owner, copyCache);
					return ResolveIdentifier(id, session, owner);
				}
			}
		}

		public override bool IsAssociationType
		{
			get { return true; }
		}

		/// <summary>
		/// Converts the id contained in the <see cref="DbDataReader"/> to an object.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the query results.</param>
		/// <param name="names">A string array of column names that contain the id.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> this is occurring in.</param>
		/// <param name="owner">The object that this Entity will be a part of.</param>
		/// <returns>
		/// An instance of the object or <see langword="null" /> if the identifer was null.
		/// </returns>
		public override sealed object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return ResolveIdentifier(Hydrate(rs, names, session, owner), session, owner);
		}

		public abstract override object Hydrate(DbDataReader rs, string[] names, ISessionImplementor session, object owner);

		public bool IsUniqueKeyReference
		{
			get { return uniqueKeyPropertyName != null; }
		}

		public abstract bool IsNullable { get; }

		/// <summary> Retrieves the {@link Joinable} defining the associated entity. </summary>
		/// <param name="factory">The session factory. </param>
		/// <returns> The associated joinable </returns>
		public IJoinable GetAssociatedJoinable(ISessionFactoryImplementor factory)
		{
			return (IJoinable) factory.GetEntityPersister(associatedEntityName);
		}

		/// <summary> 
		/// Determine the type of either (1) the identifier if we reference the
		/// associated entity's PK or (2) the unique key to which we refer (i.e.
		/// the property-ref). 
		/// </summary>
		/// <param name="factory">The mappings... </param>
		/// <returns> The appropriate type. </returns>
		public IType GetIdentifierOrUniqueKeyType(IMapping factory)
		{
			if (IsReferenceToPrimaryKey)
			{
				return GetIdentifierType(factory);
			}
			else
			{
				IType type = factory.GetReferencedPropertyType(GetAssociatedEntityName(), uniqueKeyPropertyName);
				if (type.IsEntityType)
				{
					type = ((EntityType) type).GetIdentifierOrUniqueKeyType(factory);
				}
				return type;
			}
		}

		/// <summary> 
		/// The name of the property on the associated entity to which our FK refers 
		/// </summary>
		/// <param name="factory">The mappings... </param>
		/// <returns> The appropriate property name. </returns>
		public string GetIdentifierOrUniqueKeyPropertyName(IMapping factory)
		{
			if (IsReferenceToPrimaryKey)
			{
				return factory.GetIdentifierPropertyName(GetAssociatedEntityName());
			}
			else
			{
				return uniqueKeyPropertyName;
			}
		}

		/// <summary> Convenience method to locate the identifier type of the associated entity. </summary>
		/// <param name="factory">The mappings... </param>
		/// <returns> The identifier type </returns>
		internal virtual IType GetIdentifierType(IMapping factory)
		{
			return factory.GetIdentifierType(GetAssociatedEntityName());
		}

		/// <summary> Convenience method to locate the identifier type of the associated entity. </summary>
		/// <param name="session">The originating session </param>
		/// <returns> The identifier type </returns>
		internal virtual IType GetIdentifierType(ISessionImplementor session)
		{
			return GetIdentifierType(session.Factory);
		}

		/// <summary>
		/// Resolves the identifier to the actual object.
		/// </summary>
		protected object ResolveIdentifier(object id, ISessionImplementor session)
		{
			string entityName = GetAssociatedEntityName();
			bool isProxyUnwrapEnabled = unwrapProxy && session.Factory
			                                                  .GetEntityPersister(entityName).IsInstrumented;

			object proxyOrEntity = session.InternalLoad(entityName, id, eager, IsNullable && !isProxyUnwrapEnabled);

			if (proxyOrEntity.IsProxy())
			{
				INHibernateProxy proxy = (INHibernateProxy) proxyOrEntity;
				proxy.HibernateLazyInitializer.Unwrap = isProxyUnwrapEnabled;
			}

			return proxyOrEntity;
		}

		/// <summary>
		/// Resolve an identifier or unique key value
		/// </summary>
		/// <param name="value"></param>
		/// <param name="session"></param>
		/// <param name="owner"></param>
		/// <returns></returns>
		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				if (IsNull(owner, session))
				{
					return null; //EARLY EXIT!
				}

				if (IsReferenceToPrimaryKey)
				{
					return ResolveIdentifier(value, session);
				}
				else
				{
					return LoadByUniqueKey(GetAssociatedEntityName(), uniqueKeyPropertyName, value, session);
				}
			}
		}

		/// <summary> The name of the associated entity. </summary>
		/// <param name="factory">The session factory, for resolution. </param>
		/// <returns> The associated entity name. </returns>
		public virtual string GetAssociatedEntityName(ISessionFactoryImplementor factory)
		{
			return GetAssociatedEntityName();
		}

		/// <summary> The name of the associated entity. </summary>
		/// <returns> The associated entity name.</returns>
		public string GetAssociatedEntityName()
		{
			return associatedEntityName;
		}

		/// <summary>
		/// When implemented by a class, gets the type of foreign key directionality 
		/// of this association.
		/// </summary>
		/// <value>The <see cref="ForeignKeyDirection"/> of this association.</value>
		public abstract ForeignKeyDirection ForeignKeyDirection { get; }

		/// <summary>
		/// Is the foreign key the primary key of the table?
		/// </summary>
		public abstract bool UseLHSPrimaryKey { get; }

		public string LHSPropertyName
		{
			get { return null; }
		}

		public string RHSUniqueKeyPropertyName
		{
			get { return uniqueKeyPropertyName; }
		}

		public virtual string PropertyName
		{
			get { return null; }
		}

		public override int GetHashCode(object x, ISessionFactoryImplementor factory)
		{
			IEntityPersister persister = factory.GetEntityPersister(associatedEntityName);
			if (!persister.CanExtractIdOutOfEntity)
			{
				return base.GetHashCode(x);
			}

			object id;

			if (x.IsProxy())
			{
				INHibernateProxy proxy = x as INHibernateProxy;
				id = proxy.HibernateLazyInitializer.Identifier;
			}
			else
			{
				id = persister.GetIdentifier(x);
			}
			return persister.IdentifierType.GetHashCode(id, factory);
		}

		public abstract bool IsAlwaysDirtyChecked { get; }

		public bool IsReferenceToPrimaryKey
		{
			get { return string.IsNullOrEmpty(uniqueKeyPropertyName); }
		}

		public string GetOnCondition(string alias, ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
		{
			if (IsReferenceToPrimaryKey)
			{
				//TODO: this is a bit arbitrary, expose a switch to the user?
				return string.Empty;
			}
			else
			{
				return GetAssociatedJoinable(factory).FilterFragment(alias, FilterHelper.GetEnabledForManyToOne(enabledFilters));
			}
		}

		public override IType GetSemiResolvedType(ISessionFactoryImplementor factory)
		{
			return factory.GetEntityPersister(associatedEntityName).IdentifierType;
		}

		/// <summary> 
		/// Load an instance by a unique key that is not the primary key. 
		/// </summary>
		/// <param name="entityName">The name of the entity to load </param>
		/// <param name="uniqueKeyPropertyName">The name of the property defining the unique key. </param>
		/// <param name="key">The unique key property value. </param>
		/// <param name="session">The originating session. </param>
		/// <returns> The loaded entity </returns>
		public object LoadByUniqueKey(string entityName, string uniqueKeyPropertyName, object key, ISessionImplementor session)
		{
			ISessionFactoryImplementor factory = session.Factory;
			IUniqueKeyLoadable persister = (IUniqueKeyLoadable) factory.GetEntityPersister(entityName);

			//TODO: implement caching?! proxies?!

			EntityUniqueKey euk =
				new EntityUniqueKey(
					entityName,
					uniqueKeyPropertyName,
					key,
					GetIdentifierOrUniqueKeyType(factory),
					session.Factory);

			IPersistenceContext persistenceContext = session.PersistenceContext;
			try
			{
				object result = persistenceContext.GetEntity(euk);
				if (result == null)
				{
					result = persister.LoadByUniqueKey(uniqueKeyPropertyName, key, session);
				}
				return result == null ? null : persistenceContext.ProxyFor(result);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(factory.SQLExceptionConverter, sqle, "Error performing LoadByUniqueKey");
			}
		}

		public override int Compare(object x, object y)
		{
			IComparable xComp = x as IComparable;
			IComparable yComp = y as IComparable;
			if (xComp != null)
				return xComp.CompareTo(y);
			if (yComp != null)
				return -yComp.CompareTo(x);

			return 0;
		}

		private System.Type DetermineAssociatedEntityClass()
		{
			try
			{
				return ReflectHelper.ClassForFullName(GetAssociatedEntityName());
			}
			catch (Exception)
			{
				return typeof(IDictionary);
			}
		}

		public override string ToString()
		{
			return GetType().FullName + '(' + GetAssociatedEntityName() + ')';
		}
	}
}
