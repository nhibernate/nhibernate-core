using System;

using NHibernate.Engine;
using NHibernate.Sql;


namespace NHibernate.Type {

	/// <summary>
	/// Mapping of the built in Type hierarchy.
	/// </summary>
	public abstract class AbstractType : IType {
		
		public bool IsAssociationType {
			get {
				return false;
			}
		}
	
		public bool IsPersistentCollectionType {
			get {
				return false;
			}
		}
	
		public bool IsComponentType {
			get {
				return false;
			}
		}
	
		public bool IsEntityType {
			get {
				return false;
			}
		}

		/*
		public Serializable Disassemble(object value, ISessionImplementor session) {
			if (value==null) {
				return null;
			}
			else {
				return (Serializable) DeepCopy(value);
			}
		}
		*/
	
		/*
		public object Assemble(Serializable cached, ISessionImplementor session, object owner) {
			if ( cached==null ) {
				return null;
			}
			else {
				return DeepCopy(cached);
			}
		}
		*/
	
		public bool IsDirty(object old, object current, ISessionImplementor session) {
			return !Equals(old, current);
		}
	
		/*
		public object Hydrate(ResultSet rs, string[] names,	ISessionImplementor session, object owner) {
			return NullSafeGet(rs, names, session, owner);
		}
		*/
				
		public object ResolveIdentifier(object value, ISessionImplementor session, object owner) {
			return value;
		}
				
		public bool IsObjectType {
			get {
				return false;
			}
		}

		public abstract object DeepCopy(object val);

		public abstract Types[] SqlTypes(IMapping mapping);

		public abstract int GetColumnSpan(IMapping mapping);

		public abstract new bool Equals(object x, object y);  //We need "new" because object.Equal is not marked as virtual. Is it correct?

		public abstract bool IsMutable {get;}

		public abstract string Name { get; }
	}
}