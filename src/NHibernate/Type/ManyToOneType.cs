using System;
using System.Data;

using NHibernate.Mapping;
using NHibernate.Engine;
using NHibernate.Sql;

namespace NHibernate.Type {

	/// <summary>
	/// A many-to-one association to an entity
	/// </summary>
	public class ManyToOneType : EntityType, IAssociationType {

		public override int GetColumnSpan(IMapping session) {
            return session.GetIdentifierType( PersistentClass ).GetColumnSpan(session);
		}

		public override DbType[] SqlTypes(IMapping session) {
            return session.GetIdentifierType( PersistentClass ).SqlTypes(session);
		}
	
		public ManyToOneType(System.Type persistentClass) : base(persistentClass) {
		}

		public override void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) {
			session.GetFactory().GetIdentifierType( PersistentClass )
			.NullSafeSet(cmd, GetIdentifier(value, session), index, session);
		}
		
		public override bool IsOneToOne {
			get { return false; }
		}

		public virtual ForeignKeyType ForeignKeyType {
			get { return ForeignKeyType.ForeignKeyFromParent; }
		}

		public override object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner) {
			return session.GetFactory().GetIdentifierType( PersistentClass )
			.NullSafeGet(rs, names, session, owner);
		}

		public override object ResolveIdentifier(object value, ISessionImplementor session, object owner) {
			if (value==null) {
				return null;
			}
			else {
				return session.InternalLoad( PersistentClass, value );
			}
		}
	}
}