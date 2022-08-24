using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlTypes;
using NHibernate.Util;

namespace NHibernate.Type
{
	/// <summary>
	/// A one-to-one association to an entity
	/// </summary>
	[Serializable]
	public partial class OneToOneType : EntityType, IAssociationType
	{
		private readonly ForeignKeyDirection foreignKeyDirection;
		private readonly string propertyName;
		private readonly string entityName;

		public override int GetColumnSpan(IMapping mapping)
		{
			// our column span is the number of columns in the PK
			return GetIdentifierOrUniqueKeyType(mapping).GetColumnSpan(mapping);
		}
		
		public override int GetOwnerColumnSpan(IMapping mapping)
		{
			return 0;
		}

		public override SqlType[] SqlTypes(IMapping mapping)
		{
			return GetIdentifierOrUniqueKeyType(mapping).SqlTypes(mapping);
		}

		public OneToOneType(string referencedEntityName, ForeignKeyDirection foreignKeyType, string uniqueKeyPropertyName, bool lazy, bool unwrapProxy, string entityName, string propertyName)
			: base(referencedEntityName, uniqueKeyPropertyName, !lazy, unwrapProxy)
		{
			foreignKeyDirection = foreignKeyType;
			this.propertyName = propertyName;
			this.entityName = entityName;
		}

		public override void NullSafeSet(DbCommand st, object value, int index, bool[] settable, ISessionImplementor session)
		{
			//nothing to do
		}

		public override void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
		{
			GetIdentifierOrUniqueKeyType(session.Factory)
				.NullSafeSet(cmd, GetReferenceValue(value, session, true), index, session);
		}

		public override bool IsOneToOne
		{
			get { return true; }
		}

		public override bool IsDirty(object old, object current, ISessionImplementor session)
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

			object oldId = GetIdentifier(old, session);
			object newId = GetIdentifier(current, session);
			IType identifierType = GetIdentifierType(session);

			return identifierType.IsDirty(oldId, newId, session);
		}

		public override bool IsDirty(object old, object current, bool[] checkable, ISessionImplementor session)
		{
			return this.IsDirty(old, current, session);
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
			if (propertyName != null && owner != null)
			{
				IEntityPersister ownerPersister = session.Factory.GetEntityPersister(entityName);
				object id = session.GetContextEntityIdentifier(owner);

				EntityKey entityKey = session.GenerateEntityKey(id, ownerPersister);

				return session.PersistenceContext.IsPropertyNull(entityKey, PropertyName);
			}
			else
			{
				return false;
			}
		}

		public override ForeignKeyDirection ForeignKeyDirection
		{
			get { return foreignKeyDirection; }
		}

		public override object Hydrate(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			if (owner == null && names.Length > 0)
			{
				// return the (fully resolved) identifier value, but do not resolve
				// to the actual referenced entity instance
				return GetIdentifierOrUniqueKeyType(session.Factory)
					.NullSafeGet(rs, names, session, null);
			}
			IType type = GetIdentifierOrUniqueKeyType(session.Factory);
			object identifier = session.GetContextEntityIdentifier(owner);

			//This ugly mess is only used when mapping one-to-one entities with component ID types
			if (type.IsComponentType && type is EmbeddedComponentType componentType)
			{
				if (session.GetEntityPersister(null, owner).IdentifierType is EmbeddedComponentType ownerIdType)
				{
					object[] values = ownerIdType.GetPropertyValues(identifier, session);
					object id = componentType.ResolveIdentifier(values, session, null);
					IEntityPersister persister = session.Factory.GetEntityPersister(type.ReturnedClass.FullName);
					var key = session.GenerateEntityKey(id, persister);
					return session.PersistenceContext.GetEntity(key);
				}
			}
			return identifier;
		}

		/// <inheritdoc />
		public override bool IsNullable
		{
			get { return foreignKeyDirection.Equals(ForeignKeyDirection.ForeignKeyToParent); }
		}

		public override bool UseLHSPrimaryKey
		{
			get { return true; }
		}

		public override object Disassemble(object value, ISessionImplementor session, object owner)
		{
			if (value == null)
			{
				return null;
			}

			object id = ForeignKeys.GetEntityIdentifierIfNotUnsaved(GetAssociatedEntityName(), value, session);

			if (id == null)
			{
				throw new AssertionFailure("cannot cache a reference to an object with a null id: " + GetAssociatedEntityName());
			}

			return GetIdentifierType(session).Disassemble(id, session, owner);
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner)
		{
			// the owner of the association is not the owner of the id
			object id = GetIdentifierType(session).Assemble(cached, session, null);

			if (id == null)
			{
				return null;
			}

			return ResolveIdentifier(id, session);
		}

		/// <summary>
		/// We only need to dirty check when the identifier can be null.
		/// </summary>
		public override bool IsAlwaysDirtyChecked
		{
			get { return IsNullable; }
		}

		public override string PropertyName
		{
			get { return propertyName; }
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
	}
}
