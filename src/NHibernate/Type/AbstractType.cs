using System;
using System.Data;

using NHibernate.Engine;
using NHibernate.Sql;


namespace NHibernate.Type {

	/// <summary>
	/// Mapping of the built in Type hierarchy.
	/// </summary>
	public abstract class AbstractType : IType {
		
		public virtual bool IsAssociationType {
			get {
				return false;
			}
		}
	
		public virtual bool IsPersistentCollectionType {
			get {
				return false;
			}
		}
	
		public virtual bool IsComponentType {
			get {
				return false;
			}
		}
	
		public virtual bool IsEntityType {
			get {
				return false;
			}
		}

		public virtual object Disassemble(object value, ISessionImplementor session) {
			if (value==null) {
				return null;
			}
			else {
				return DeepCopy(value);
			}
		}
	
		
		public virtual object Assemble(object cached, ISessionImplementor session, object owner) {
			if ( cached==null ) {
				return null;
			}
			else {
				return DeepCopy(cached);
			}
		}
		
	
		public virtual bool IsDirty(object old, object current, ISessionImplementor session) {
			return !Equals(old, current);
		}
	

		public virtual object Hydrate(IDataReader rs, string[] names, ISessionImplementor session, object owner) {
			return NullSafeGet(rs, names, session, owner);
		}
				
		public virtual object ResolveIdentifier(object value, ISessionImplementor session, object owner) {
			return value;
		}
				
		public virtual bool IsObjectType {
			get {
				return false;
			}
		}

		public abstract object DeepCopy(object val);

		public abstract DbType[] SqlTypes(IMapping mapping);

		public abstract int GetColumnSpan(IMapping mapping);

		public abstract new bool Equals(object x, object y);  //We need "new" because object.Equal is not marked as virtual. Is it correct?

		public abstract bool IsMutable {get;}

		public abstract string Name { get; }

		public abstract bool HasNiceEquals { get; }

		public abstract object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner);

		public abstract object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, Object owner);
	
		public abstract void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session); 

		public abstract System.Type ReturnedClass { get; }

		public abstract string ToXML(object value, ISessionFactoryImplementor factory);
	}
}