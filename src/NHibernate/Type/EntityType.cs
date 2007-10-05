using System;
using System.Collections;
using System.Data;
using System.Text;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// A reference to an entity class
	/// </summary>
	[Serializable]
	public abstract class EntityType : AbstractType, IAssociationType
	{
		private readonly System.Type associatedClass;
		private readonly bool niceEquals;
		protected readonly string uniqueKeyPropertyName;
		private bool eager;

		public override sealed bool IsEntityType
		{
			get { return true; }
		}

		public System.Type AssociatedClass
		{
			get { return associatedClass; }
		}

		public override sealed bool Equals(object x, object y)
		{
			return x == y;
		}

		public override int GetHashCode(object x, ISessionFactoryImplementor factory)
		{
			IEntityPersister persister = factory.GetEntityPersister(associatedClass);
			if (!persister.HasIdentifierPropertyOrEmbeddedCompositeIdentifier)
			{
				return base.GetHashCode(x, factory);
			}

			object id;
			if (x is INHibernateProxy)
			{
				id = NHibernateProxyHelper.GetLazyInitializer((INHibernateProxy) x).Identifier;
			}
			else
			{
				id = persister.GetIdentifier(x);
			}
			return persister.IdentifierType.GetHashCode(id, factory);
		}

		protected EntityType(System.Type persistentClass, string uniqueKeyPropertyName, bool eager)
		{
			this.associatedClass = persistentClass;
			this.niceEquals = !ReflectHelper.OverridesEquals(persistentClass);
			this.uniqueKeyPropertyName = uniqueKeyPropertyName;
			this.eager = eager;
		}

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner)
		{
			return NullSafeGet(rs, new string[] {name}, session, owner);
		}

		// This returns the wrong class for an entity with a proxy. Theoretically
		// it should return the proxy class, but it doesn't.
		public override sealed System.Type ReturnedClass
		{
			get { return associatedClass; }
		}

		protected internal object GetIdentifier(object value, ISessionImplementor session)
		{
			if (IsReferenceToPrimaryKey)
			{
				return ForeignKeys.GetEntityIdentifierIfNotUnsaved(GetAssociatedEntityName(), value, session); //tolerates nulls
			}
			else if (value == null)
			{
				return null;
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
					propertyValue = ((EntityType)type).GetIdentifier(propertyValue, session);
				}

				return propertyValue;
			}
		}

		public override string ToLoggableString(object value, ISessionFactoryImplementor factory)
		{
			IEntityPersister persister = factory.GetEntityPersister(associatedClass);
			if (value == null)
			{
				return "null";
			}
			StringBuilder result = new StringBuilder();
			result.Append(StringHelper.Unqualify(NHibernateProxyHelper.GetClass(value).FullName));
			if (persister.HasIdentifierProperty)
			{
				result.Append('#')
					.Append(persister.IdentifierType.ToLoggableString(NHibernateProxyHelper.GetIdentifier(value, persister), factory));
			}

			return result.ToString();
		}

		public override object FromString(string xml)
		{
			throw new NotSupportedException(); //TODO: is this correct???
		}

		public override string Name
		{
			get { return associatedClass.Name; }
		}

		public override object DeepCopy(object value)
		{
			return value; //special case ... this is the leaf of the containment graph, even though not immutable
		}

		/// <summary></summary>
		public override bool IsMutable
		{
			get { return false; }
		}

		public abstract bool IsOneToOne { get; }

		public override object Replace(object original, object target, ISessionImplementor session, object owner,
		                               IDictionary copyCache)
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

				IType idType = GetIdentifierOrUniqueKeyType(session.Factory);
				object id = GetIdentifier(original, session);
				if (id == null)
				{
					throw new AssertionFailure("cannot copy a reference to an object with a null id");
				}

				id = idType.Replace(id, null, session, owner, copyCache);

				// Replace a property-ref'ed entity by its id
				if (IsUniqueKeyReference && idType.IsEntityType)
				{
					id = ((EntityType) idType).GetIdentifier(id, session);
				}
				return ResolveIdentifier(id, session, owner);
			}
		}

		/// <summary></summary>
		public override bool HasNiceEquals
		{
			get { return niceEquals; }
		}

		/// <summary></summary>
		public override bool IsAssociationType
		{
			get { return true; }
		}

		/// <summary>
		/// Converts the id contained in the <see cref="IDataReader"/> to an object.
		/// </summary>
		/// <param name="rs">The <see cref="IDataReader"/> that contains the query results.</param>
		/// <param name="names">A string array of column names that contain the id.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> this is occurring in.</param>
		/// <param name="owner">The object that this Entity will be a part of.</param>
		/// <returns>
		/// An instance of the object or <see langword="null" /> if the identifer was null.
		/// </returns>
		public override sealed object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			return ResolveIdentifier(Hydrate(rs, names, session, owner), session, owner);
		}

		public abstract override object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner);

		public override bool IsDirty(object old, object current, ISessionImplementor session)
		{
			if (Equals(old, current))
			{
				return false;
			}

			object oldid = GetIdentifier(old, session);
			object newid = GetIdentifier(current, session);
			return !GetIdentifierType(session).Equals(oldid, newid);
		}

		public bool IsUniqueKeyReference
		{
			get { return uniqueKeyPropertyName != null; }
		}

		public abstract bool IsNullable { get; }

		public IJoinable GetAssociatedJoinable(ISessionFactoryImplementor factory)
		{
			return (IJoinable) factory.GetEntityPersister(associatedClass);
		}

		protected IType GetIdentifierType(ISessionImplementor session)
		{
			return session.Factory.GetIdentifierType(associatedClass);
		}

		public IType GetIdentifierOrUniqueKeyType(IMapping factory)
		{
			if (IsReferenceToPrimaryKey)
			{
				return factory.GetIdentifierType(associatedClass);
			}
			else
			{
				return factory.GetPropertyType(associatedClass, uniqueKeyPropertyName);
			}
		}

		public string GetIdentifierOrUniqueKeyPropertyName(IMapping factory)
		{
			if (IsReferenceToPrimaryKey)
			{
				return factory.GetIdentifierPropertyName(associatedClass);
			}
			else
			{
				return uniqueKeyPropertyName;
			}
		}

		/// <summary>
		/// Resolves the identifier to the actual object.
		/// </summary>
		protected object ResolveIdentifier(object id, ISessionImplementor session)
		{
			object proxyOrEntity = session.InternalLoad(AssociatedClass, id, eager, IsNullable);
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

		public System.Type GetAssociatedClass(ISessionFactoryImplementor factory)
		{
			return associatedClass;
		}

		/// <summary> 
		/// The name of the associated entity.
		/// </summary>
		/// <returns> The associated entity name.
		/// </returns>
		public System.String GetAssociatedEntityName()
		{
			// TODO H3: Until we don't implements EntityName this method have the same behaviour of AbstractEntityPersister
			return associatedClass.FullName;
		}

		public string GetAssociatedEntityName(ISessionFactoryImplementor factory)
		{
			return GetAssociatedEntityName();
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

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}

			return ((EntityType) obj).associatedClass == associatedClass;
		}

		public override int GetHashCode()
		{
			return associatedClass.GetHashCode();
		}

		public abstract bool IsAlwaysDirtyChecked { get; }

		public bool IsReferenceToPrimaryKey
		{
			get { return uniqueKeyPropertyName == null; }
		}

		public string GetOnCondition(string alias, ISessionFactoryImplementor factory, IDictionary enabledFilters)
		{
			if (IsReferenceToPrimaryKey)
			{
				//TODO: this is a bit arbitrary, expose a switch to the user?
				return "";
			}
			else
			{
				return GetAssociatedJoinable(factory).FilterFragment(alias, enabledFilters);
			}
		}

		/// <summary> 
		/// Load an instance by a unique key that is not the primary key. 
		/// </summary>
		/// <param name="entityName">The name of the entity to load </param>
		/// <param name="uniqueKeyPropertyName">The name of the property defining the uniqie key. </param>
		/// <param name="key">The unique key property value. </param>
		/// <param name="session">The originating session. </param>
		/// <returns> The loaded entity </returns>
		public object LoadByUniqueKey(string entityName, string uniqueKeyPropertyName, object key, ISessionImplementor session)
		{
			ISessionFactoryImplementor factory = session.Factory;
			IUniqueKeyLoadable persister = (IUniqueKeyLoadable)factory.GetEntityPersister(entityName);
			IPersistenceContext persistenceContext = session.PersistenceContext;

			//TODO: implement caching?! proxies?!
			try
			{
				object result;
				// NH Different behavior
				//EntityUniqueKey euk = new EntityUniqueKey(entityName, uniqueKeyPropertyName, key, GetIdentifierOrUniqueKeyType(factory), session.Factory);
				//result = persistenceContext.GetEntity(euk);
				//if (result == null)
					result = persister.LoadByUniqueKey(uniqueKeyPropertyName, key, session);

				return result == null ? null : persistenceContext.ProxyFor(result);
			}
			catch (HibernateException)
			{
				// Do not call Convert on HibernateExceptions
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert( /*Factory.SQLExceptionConverter,*/ sqle, "Error performing LoadByUniqueKey");
			}
		}
	}
}
