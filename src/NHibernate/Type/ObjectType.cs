using System;
using System.Data;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Type {
	
	public class ObjectType : AbstractType {
		
		public override object DeepCopy(object value) {
			return value;
		}

		public override bool Equals(object x, object y) {
			return x == y;
		}

		public override int GetColumnSpan(IMapping session) {
			return 2;
		}

		public override string Name {
			get { return "object"; }
		}

		public override bool HasNiceEquals {
			get { return false; }
		}

		public override bool IsMutable {
			get { return false; }
		}

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner) {
			throw new NotSupportedException("object is a multicolumn type");
		}

		public override object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner) {
			string className = (string) NHibernate.String.NullSafeGet( rs, names[0] );
			object id = NHibernate.Serializable.NullSafeGet( rs, names[1] );
			if (className==null || id==null) {
				return null;
			} else {
				try {
					return session.Load( System.Type.GetType(className), id );
				} catch (Exception) {
					throw new HibernateException("Class not found: " + className);
				}
			}
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session) {
			if (value == null) {
				IDataParameter parm = st.Parameters[index] as IDataParameter;
				parm.DbType = DbType.String;
				parm.Value = null;
				parm = st.Parameters[index + 1] as IDataParameter;
				parm.DbType = DbType.Binary;
				parm.Value = null;
			} else {
				object id = session.GetEntityIdentifierIfNotUnsaved(value);
				string className = value.GetType().FullName;
				NHibernate.String.NullSafeSet(st, className, index, session);
				NHibernate.Serializable.NullSafeSet(st, id, index+1, session);
			}
		}
		public override System.Type ReturnedClass {
			get { return typeof(object); }
		}

		private static readonly DbType[] theSqlTypes = new DbType[] { DbType.String, DbType.Binary };

		public override DbType[] SqlTypes(IMapping pi) {
			return theSqlTypes;
		}

		public override string ToXML(object value, ISessionFactoryImplementor factory) {
			return NHibernate.Association( value.GetType() ).ToXML(value, factory);
		}

		[Serializable]
		public sealed class ObjectTypeCacheEntry {
			public System.Type clazz;
			public object id;
			public ObjectTypeCacheEntry(System.Type clazz, object id) {
				this.clazz = clazz;
				this.id = id;
			}
		}

		public object Assembly(object cached, ISessionImplementor session, object owner) {
			ObjectTypeCacheEntry e = (ObjectTypeCacheEntry) cached;
			return (cached==null) ? null : session.Load(e.clazz, e.id);
		}

		public override object Disassemble(object value, ISessionImplementor session) {
			return (value==null) ? null : new ObjectTypeCacheEntry( value.GetType(), session.GetEntityIdentifier(value) );
		}

		public override bool IsObjectType {
			get { return true; }
		}

	}
}
