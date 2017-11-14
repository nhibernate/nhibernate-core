
using NHibernate.Id;
using NHibernate.Persister.Entity;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.Engine
{
	/// <summary> Algorithms related to foreign key constraint transparency </summary>
	public static partial class ForeignKeys
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ForeignKeys));

		public partial class Nullifier
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

				if (obj.IsProxy())
				{
                    INHibernateProxy proxy = obj as INHibernateProxy;
                    
                    // if its an uninitialized proxy it can't be transient
					ILazyInitializer li = proxy.HibernateLazyInitializer;
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
					return IsTransientSlow(entityName, obj, session);
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
		/// Hit the database to make the determination.
		/// </remarks>
		public static bool IsNotTransientSlow(string entityName, object entity, ISessionImplementor session)
		{
			if (entity.IsProxy())
				return true;
			if (session.PersistenceContext.IsEntryFor(entity))
				return true;
			return !IsTransientSlow(entityName, entity, session);
		}

		/// <summary> 
		/// Is this instance, which we know is not persistent, actually transient? 
		/// Don't hit the database to make the determination, instead return null; 
		/// </summary>
		/// <remarks>
		/// Don't hit the database to make the determination, instead return null; 
		/// </remarks>
		public static bool? IsTransientFast(string entityName, object entity, ISessionImplementor session)
		{
			if (Equals(Intercept.LazyPropertyInitializer.UnfetchedProperty, entity))
			{
				// an unfetched association can only point to
				// an entity that already exists in the db
				return false;
			}

			// let the interceptor inspect the instance to decide
			// let the persister inspect the instance to decide
			return session.Interceptor.IsTransient(entity) ??
			       session.GetEntityPersister(entityName, entity).IsTransient(entity, session);
		}

		/// <summary> 
		/// Is this instance, which we know is not persistent, actually transient? 
		/// </summary>
		/// <remarks>
		/// Hit the database to make the determination.
		/// </remarks>
		public static bool IsTransientSlow(string entityName, object entity, ISessionImplementor session)
		{
			return IsTransientFast(entityName, entity, session) ??
			       HasDbSnapshot(entityName, entity, session);
		}

		static bool HasDbSnapshot(string entityName, object entity, ISessionImplementor session)
		{
			IEntityPersister persister = session.GetEntityPersister(entityName, entity);
			if (persister.IdentifierGenerator is Assigned)
			{
				// When using assigned identifiers we cannot tell if an entity
				// is transient or detached without querying the database.
				// This could potentially cause Select N+1 in cascaded saves, so warn the user.
				log.Warn(
					"Unable to determine if " + entity.ToString()
					+ " with assigned identifier " + persister.GetIdentifier(entity)
					+ " is transient or detached; querying the database."
					+ " Use explicit Save() or Update() in session to prevent this.");
			}

			// hit the database, after checking the session cache for a snapshot
			System.Object[] snapshot =
				session.PersistenceContext.GetDatabaseSnapshot(persister.GetIdentifier(entity), persister);
			return snapshot == null;
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

					if (IsTransientFast(entityName, entity, session).GetValueOrDefault())
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

						entityName = entityName ?? session.GuessEntityName(entity);
						string entityString = entity.ToString();
						throw new TransientObjectException(
							string.Format("object references an unsaved transient instance - save the transient instance before flushing or set cascade action for the property to something that would make it autosave. Type: {0}, Entity: {1}", entityName, entityString));

					}
					id = session.GetEntityPersister(entityName, entity).GetIdentifier(entity);
				}
				return id;
			}
		}
	}
}
