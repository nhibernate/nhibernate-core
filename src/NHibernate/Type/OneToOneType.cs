using System;
using System.Data;

using NHibernate.Sql;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Type {

	/// <summary>
	/// A one-to-one association to an entity
	/// </summary>
	public class OneToOneType : EntityType, IAssociationType {

		private static readonly DbType[] NoSqlTypes = new DbType[0];
		private readonly ForeignKeyType foreignKeyType;
	
		public override int GetColumnSpan(IMapping session) {
			return 0;
		}

		public override DbType[] SqlTypes(IMapping session) {
			return NoSqlTypes;
		}
	
		public OneToOneType(System.Type persistentClass, ForeignKeyType foreignKeyType) : base(persistentClass) {
			this.foreignKeyType = foreignKeyType;
		}
	
		public override void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) {
			//nothing to do
		}
	
		public override bool IsOneToOne {
			get { return true; }
		}
		
		public override bool IsDirty(object old, object current, ISessionImplementor session) {
			return false;
		}
	
		public virtual ForeignKeyType ForeignKeyType {
			get { return foreignKeyType; }
		}
	
		public override object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner) {
			return session.GetEntityIdentifier(owner);
		}
	
		public override object ResolveIdentifier(object value, ISessionImplementor session, Object owner) {
			if (value==null) return null;
		
			System.Type clazz = PersistentClass;
			
			return IsNullable ?
			session.InternalLoadOneToOne(clazz, value) :
			session.InternalLoad(clazz, value);
		}
	
		public virtual bool IsNullable {
			get { return foreignKeyType==ForeignKeyType.ForeignKeyToParent; }
		}
		
	}
}
