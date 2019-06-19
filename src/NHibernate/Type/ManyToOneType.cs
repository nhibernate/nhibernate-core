using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// A many-to-one association to an entity
	/// </summary>
	[Serializable]
	public partial class ManyToOneType : EntityType
	{
		private readonly bool ignoreNotFound;
		private readonly bool isLogicalOneToOne;

		public ManyToOneType(string className)
			: this(className, false)
		{
		}

		public ManyToOneType(string className, bool lazy)
			: base(className, null, !lazy, false)
		{
			ignoreNotFound = false;
			isLogicalOneToOne = false;
			PropertyName = null;
		}
		
		//Since 5.3
		[Obsolete("Use Constructor with property name")]
		public ManyToOneType(string entityName, string uniqueKeyPropertyName, bool lazy, bool unwrapProxy, bool ignoreNotFound, bool isLogicalOneToOne)
			: this(entityName, uniqueKeyPropertyName, lazy, unwrapProxy, ignoreNotFound, isLogicalOneToOne, null)
		{
			
		}


		public ManyToOneType(string entityName, string uniqueKeyPropertyName, bool lazy, bool unwrapProxy, bool ignoreNotFound, bool isLogicalOneToOne, string propertyName)
			: base(entityName, uniqueKeyPropertyName, !lazy, unwrapProxy)
		{
			this.ignoreNotFound = ignoreNotFound;
			this.isLogicalOneToOne = isLogicalOneToOne;
			PropertyName = propertyName;
		}

		public override int GetColumnSpan(IMapping mapping)
		{
			// our column span is the number of columns in the PK
			return GetIdentifierOrUniqueKeyType(mapping).GetColumnSpan(mapping);
		}

		public override SqlType[] SqlTypes(IMapping mapping)
		{
			return GetIdentifierOrUniqueKeyType(mapping).SqlTypes(mapping);
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			GetIdentifierOrUniqueKeyType(session.Factory)
				.NullSafeSet(st, GetReferenceValue(value, session), index, settable, session);
		}

		public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			GetIdentifierOrUniqueKeyType(session.Factory)
				.NullSafeSet(cmd, GetReferenceValue(value, session), index, session);
		}

		public override bool IsOneToOne
		{
			get { return false; }
		}

		public override bool IsLogicalOneToOne()
		{
			return isLogicalOneToOne;
		}

		public override string PropertyName { get; }

		public override ForeignKeyDirection ForeignKeyDirection
		{
			get { return ForeignKeyDirection.ForeignKeyFromParent; }
		}

		/// <summary>
		/// Hydrates the Identifier from <see cref="DbDataReader"/>.
		/// </summary>
		/// <param name="rs">The <see cref="DbDataReader"/> that contains the query results.</param>
		/// <param name="names">A string array of column names to read from.</param>
		/// <param name="session">The <see cref="ISessionImplementor"/> this is occurring in.</param>
		/// <param name="owner">The object that this Entity will be a part of.</param>
		/// <returns>
		/// An instantiated object that used as the identifier of the type.
		/// </returns>
		public override object Hydrate(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			// return the (fully resolved) identifier value, but do not resolve
			// to the actual referenced entity instance
			// NOTE: the owner of the association is not really the owner of the id!
			object id = GetIdentifierOrUniqueKeyType(session.Factory)
				.NullSafeGet(rs, names, session, owner);
			ScheduleBatchLoadIfNeeded(id, session, false);
			return id;
		}

		private void ScheduleBatchLoadIfNeeded(object id, ISessionImplementor session, bool addToQueryCacheBatch)
		{
			//cannot batch fetch by unique key (property-ref associations)
			if (uniqueKeyPropertyName == null && id != null)
			{
				var persister = session.Factory.GetEntityPersister(GetAssociatedEntityName());
				if (!persister.IsBatchLoadable && !addToQueryCacheBatch)
				{
					return;
				}

				var entityKey = session.GenerateEntityKey(id, persister);
				if (persister.IsBatchLoadable && !session.PersistenceContext.ContainsEntity(entityKey))
				{
					session.PersistenceContext.BatchFetchQueue.AddBatchLoadableEntityKey(entityKey);
				}

				if (addToQueryCacheBatch)
				{
					session.PersistenceContext.BatchFetchQueue.QueryCacheQueue?.AddEntityKey(entityKey);
				}
			}
		}

		public override bool UseLHSPrimaryKey
		{
			get { return false; }
		}

		public override bool IsModified(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			if (current == null)
			{
				return old != null;
			}
			if (old == null)
			{
				return true;
			}
			var oldIdentifier = IsIdentifier(old, session) ? old : GetIdentifier(old, session);
			var currentIdentifier = GetIdentifier(current, session);
			// the ids are fully resolved, so compare them with isDirty(), not isModified()
			return GetIdentifierOrUniqueKeyType(session.Factory).IsDirty(oldIdentifier, currentIdentifier, session);
		}

		private bool IsIdentifier(object value, ISessionImplementor session)
		{
			var identifierType = GetIdentifierType(session);
			if (identifierType == null)
			{
				return false;
			}
			return value.GetType() == identifierType.ReturnedClass;
		}

		public override bool IsNull(object owner, ISessionImplementor session)
		{
			if (IsNullable && !string.IsNullOrEmpty(PropertyName))
			{
				EntityEntry entry = session.PersistenceContext.GetEntry(owner);

				return session.PersistenceContext.IsPropertyNull(entry.EntityKey, PropertyName);
			}

			return false;
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			if (value == null)
			{
				return null;
			}
			else
			{
				// cache the actual id of the object, not the value of the
				// property-ref, which might not be initialized
				object id = ForeignKeys.GetEntityIdentifierIfNotUnsaved(GetAssociatedEntityName(), value, session);
				if (id == null)
				{
					throw new AssertionFailure("cannot cache a reference to an object with a null id: " + GetAssociatedEntityName());
				}
				return GetIdentifierType(session).Disassemble(id, session, owner);
			}
		}

		public override object Assemble(object oid, ISessionImplementor session, object owner)
		{
			//TODO: currently broken for unique-key references (does not detect
			//      change to unique key property of the associated object)

			object id = AssembleId(oid, session);

			if (id == null)
			{
				return null;
			}
			else
			{
				return ResolveIdentifier(id, session);
			}
		}

		public override void BeforeAssemble(object oid, ISessionImplementor session)
		{
			ScheduleBatchLoadIfNeeded(AssembleId(oid, session), session, true);
		}

		private object AssembleId(object oid, ISessionImplementor session)
		{
			//the owner of the association is not the owner of the id
			return GetIdentifierType(session).Assemble(oid, session, null);
		}

		public override bool IsAlwaysDirtyChecked
		{
			get { return ignoreNotFound; }
		}

		public override bool IsDirty(object old, object current, ISessionImplementor session)
		{
			return IsDirtyManyToOne(old, current, null, session);
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return IsDirtyManyToOne(old, current, IsAlwaysDirtyChecked ? null : checkable, session);
		}

		/// <inheritdoc />
		public override bool IsNullable
		{
			get { return ignoreNotFound; }
		}

		public override bool[] ToColumnNullness(object value, IMapping mapping)
		{
			bool[] result = new bool[GetColumnSpan(mapping)];
			if (value != null)
			{
				ArrayHelper.Fill(result, true);
			}
			return result;
		}

		private bool IsDirtyManyToOne(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			if (IsSame(old, current))
			{
				return false;
			}

			if (old == null || current == null)
			{
				return true;
			}

			if (ForeignKeys.IsTransientFast(GetAssociatedEntityName(), current, session).GetValueOrDefault())
			{
				return true;
			}

			object oldid = GetIdentifier(old, session);
			object newid = GetIdentifier(current, session);
			IType identifierType = GetIdentifierType(session);

			return checkable == null
				? identifierType.IsDirty(oldid, newid, session)
				: identifierType.IsDirty(oldid, newid, checkable, session);
		}
	}
}
