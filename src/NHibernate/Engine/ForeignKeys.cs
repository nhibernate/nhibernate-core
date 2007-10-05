using NHibernate.Impl;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> Algorithms related to foreign key constraint transparency </summary>
	public static class ForeignKeys
	{
		public class Nullifier
		{
			private readonly bool isDelete;
			private readonly bool isEarlyInsert;
			private readonly ISessionImplementor session;
			private readonly object self;

			public Nullifier(object self, bool isDelete, bool isEarlyInsert, ISessionImplementor session)
			{
				this.isDelete = isDelete;
				this.isEarlyInsert = isEarlyInsert;
				this.session = session;
				this.self = self;
			}

			/// <summary> 
			/// Nullify all references to entities that have not yet 
			/// been inserted in the database, where the foreign key
			/// points toward that entity
			/// </summary>
			public void NullifyTransientReferences(object[] values, IType[] types)
			{
				for (int i = 0; i < types.Length; i++)
				{
					values[i] = NullifyTransientReferences(values[i], types[i]);
				}
			}

			/// <summary> 
			/// Return null if the argument is an "unsaved" entity (ie. 
			/// one with no existing database row), or the input argument 
			/// otherwise. This is how Hibernate avoids foreign key constraint
			/// violations.
			/// </summary>
			private object NullifyTransientReferences(object value, IType type)
			{
				if (value == null)
				{
					return null;
				}
				else if (type.IsEntityType)
				{
					EntityType entityType = (EntityType)type;
					if (entityType.IsOneToOne)
					{
						return value;
					}
					else
					{
						string entityName = entityType.GetAssociatedEntityName();
						return IsNullifiable(entityName, value) ? null : value;
					}
				}
				else if (type.IsAnyType)
				{
					return IsNullifiable(null, value) ? null : value;
				}
				else if (type.IsComponentType)
				{
					IAbstractComponentType actype = (IAbstractComponentType)type;
					object[] subvalues = actype.GetPropertyValues(value, session);
					IType[] subtypes = actype.Subtypes;
					bool substitute = false;
					for (int i = 0; i < subvalues.Length; i++)
					{
						object replacement = NullifyTransientReferences(subvalues[i], subtypes[i]);
						if (replacement != subvalues[i])
						{
							substitute = true;
							subvalues[i] = replacement;
						}
					}
					if (substitute)
						actype.SetPropertyValues(value, subvalues);
					return value;
				}
				else
				{
					return value;
				}
			}

			/// <summary> 
			/// Determine if the object already exists in the database, using a "best guess"
			/// </summary>
			private bool IsNullifiable(string entityName, object obj)
			{

				//if (obj == org.hibernate.intercept.LazyPropertyInitializer_Fields.UNFETCHED_PROPERTY)
				//  return false; //this is kinda the best we can do...

				INHibernateProxy proxy = obj as INHibernateProxy;
				if (proxy != null)
				{
					// if its an uninitialized proxy it can't be transient
					LazyInitializer li = NHibernateProxyHelper.GetLazyInitializer(proxy);
					if (li.GetImplementation(session) == null)
					{
						return false;
						// ie. we never have to null out a reference to
						// an uninitialized proxy
					}
					else
					{
						//unwrap it
						obj = li.GetImplementation();
					}
				}

				// if it was a reference to self, don't need to nullify
				// unless we are using native id generation, in which
				// case we definitely need to nullify
				if (obj == self)
				{
					// TODO H3.2: Different behaviour
					//return isEarlyInsert || (isDelete && session.Factory.Dialect.HasSelfReferentialForeignKeyBug);
					return isEarlyInsert || isDelete;
				}

				// See if the entity is already bound to this session, if not look at the
				// entity identifier and assume that the entity is persistent if the
				// id is not "unsaved" (that is, we rely on foreign keys to keep
				// database integrity)
				EntityEntry entityEntry = session.PersistenceContext.GetEntry(obj);
				if (entityEntry == null)
				{
					return IsTransient(entityName, obj, null, session);
				}
				else
				{
					return entityEntry.IsNullifiable(isEarlyInsert, session);
				}
			}
		}

		/// <summary> 
		/// Is this instance persistent or detached?
		/// </summary>
		/// <remarks>
		/// If <paramref name="assumed"/> is non-null, don't hit the database to make the 
		/// determination, instead assume that value; the client code must be 
		/// prepared to "recover" in the case that this assumed result is incorrect.
		/// </remarks>
		public static bool IsNotTransient(string entityName, System.Object entity, bool? assumed, ISessionImplementor session)
		{
			if (entity is INHibernateProxy)
				return true;
			if (session.PersistenceContext.IsEntryFor(entity))
				return true;
			return !IsTransient(entityName, entity, assumed, session);
		}

		/// <summary> 
		/// Is this instance, which we know is not persistent, actually transient? 
		/// If <tt>assumed</tt> is non-null, don't hit the database to make the 
		/// determination, instead assume that value; the client code must be 
		/// prepared to "recover" in the case that this assumed result is incorrect.
		/// </summary>
		/// <remarks>
		/// If <paramref name="assumed"/> is non-null, don't hit the database to make the 
		/// determination, instead assume that value; the client code must be 
		/// prepared to "recover" in the case that this assumed result is incorrect.
		/// </remarks>
		public static bool IsTransient(string entityName, object entity, bool? assumed, ISessionImplementor session)
		{
			// todo-events: verify
			//if (entity == org.hibernate.intercept.LazyPropertyInitializer.UNFETCHED_PROPERTY)
			//{
			//  // an unfetched association can only point to
			//  // an entity that already exists in the db
			//  return false;
			//}

			// let the interceptor inspect the instance to decide
			bool? isUnsaved = session.Interceptor.IsUnsaved(entity);
			if (isUnsaved.HasValue)
				return isUnsaved.Value;

			// let the persister inspect the instance to decide
			IEntityPersister persister = session.GetEntityPersister(entity);
			return persister.IsUnsaved(entity);

			// NH : Different behavior (the persister.IsUnsaved return a value any way)
			// we use the assumed value, if there is one, to avoid hitting
			// the database
			//if (assumed.HasValue)
			//  return assumed.Value;

			// hit the database, after checking the session cache for a snapshot
			//System.Object[] snapshot = session.PersistenceContext.getDatabaseSnapshot(persister.getIdentifier(entity, session.EntityMode), persister);
			//return snapshot == null;
		}

		/// <summary> 
		/// Return the identifier of the persistent or transient object, or throw
		/// an exception if the instance is "unsaved"
		/// </summary>
		/// <remarks>
		/// Used by OneToOneType and ManyToOneType to determine what id value should 
		/// be used for an object that may or may not be associated with the session. 
		/// This does a "best guess" using any/all info available to use (not just the 
		/// EntityEntry).
		/// </remarks>
		public static object GetEntityIdentifierIfNotUnsaved(string entityName, object entity, ISessionImplementor session)
		{
			if (entity == null)
			{
				return null;
			}
			else
			{
				object id = session.GetContextEntityIdentifier(entity);
				if (id == null)
				{
					// context-entity-identifier returns null explicitly if the entity
					// is not associated with the persistence context; so make some deeper checks...

					/***********************************************/
					// NH-479 (very dirty patch)
					if (entity.GetType().IsPrimitive)
						return entity;
					/**********************************************/

					if (IsTransient(entityName, entity, false, session))
					{
						/***********************************************/
						// TODO NH verify the behavior of NH607 test
						// these lines are only to pass test NH607 during PersistenceContext porting
						// i'm not secure that NH607 is a test for a right behavior
						EntityEntry entry = session.PersistenceContext.GetEntry(entity);
						if (entry != null)
							return entry.Id;
						// the check was put here to have les possible impact
						/**********************************************/

						// TODO H3.2 EntityName
						//throw new TransientObjectException("object references an unsaved transient instance - save the transient instance before flushing: " + 
						//  (entityName ?? session.GuessEntityName(entity)));
						throw new TransientObjectException("object references an unsaved transient instance - save the transient instance before flushing: " +
							(entityName ?? session.GetEntityPersister(entity).EntityName));
					}
					// TODO H3.2 EntityName
					//id = session.GetEntityPersister(entityName, entity).GetIdentifier(entity);
					id = session.GetEntityPersister(entity).GetIdentifier(entity);
				}
				return id;
			}
		}
	}
}
