using System;
using System.Data;

using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Loader;
using NHibernate.SqlTypes;

namespace NHibernate.Type {
	

	///<summary>
	///	Handles "any" mappings and the old deprecated "object" type.
	///</summary>
	///<remarks>
	///	The identifierType is any NHibernate IType that can be serailized by default.
	///	For example, you can specify the identifierType as an Int32 or a custom identifier
	///	type that you built.  The identifierType matches to one or many columns.
	///	
	///	The metaType maps to a single column.  By default it stores the name of the Type
	///	that the Identifier identifies.  
	///	
	///	For example, we can store a link to any table.  It will have the results
	///	class_name					id_col1
	///	========================================
	///	Simple, AssemblyName			5
	///	DiffClass, AssebmlyName			5
	///	Simple, AssemblyName			4
	///	
	///	You can also provide you own type that might map the name of the class to a table
	///	with a giant switch statemet or a good naming convention for your class->table.  The
	///	data stored might look like
	///	class_name					id_col1
	///	========================================
	///	simple_table					5
	///	diff_table						5
	///	simple_table					4
	///	
	///</remarks>
	public class ObjectType : AbstractType, IAbstractComponentType, IAssociationType {

		private readonly IType identifierType;
		private readonly IType metaType;

		internal ObjectType(IType metaType, IType identifierType) {
			this.identifierType = identifierType;
			this.metaType = metaType;
		}
	
		internal ObjectType() : this(NHibernate.Class, NHibernate.Serializable) 
		{
		}
		
		public override object DeepCopy(object value) {
			return value;
		}

		public override bool Equals(object x, object y) {
			return x == y;
		}

		public override int GetColumnSpan(IMapping session) {
			/*
			 * This is set at 2 in Hibernate to support the old depreciated
			 * version of using type="object".  We are removing that support so
			 * I don't know if we need to keep this in.
			 */ 
			return 2;
		}

		public override string Name {
			get { return "Object"; }
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
			//if ( names.length!=2 ) throw new HibernateException("object type mapping must specify exactly two columns");
		
			System.Type clazz = (System.Type) metaType.NullSafeGet(rs, names[0], session, owner);
			object id = identifierType.NullSafeGet(rs, names[1], session, owner);
			if (clazz==null || id==null) {
				return null;
			}
			else {
				return session.Load(clazz, id);
			}
		}

		public override void NullSafeSet(IDbCommand st, object value, int index, ISessionImplementor session) {
			object id;
			System.Type clazz;

			if (value == null) {
				id = null;
				clazz = null;
			} 
			else {
				id = session.GetEntityIdentifierIfNotUnsaved(value);
				clazz = value.GetType();
			}
			metaType.NullSafeSet(st, clazz, index, session);
			identifierType.NullSafeSet(st, id, index+1, session); // metaType must be single-column type
		}

		public override System.Type ReturnedClass {
			get { return typeof(object); }
		}

		
		public override SqlType[] SqlTypes(IMapping mapping) {
			return ArrayHelper.Join(
				metaType.SqlTypes(mapping),
				identifierType.SqlTypes(mapping));

		}

		public override string ToXML(object value, ISessionFactoryImplementor factory) {
			return NHibernate.Entity( value.GetType() ).ToXML(value, factory);
		}

		[Serializable]
		public sealed class ObjectTypeCacheEntry 
		{
			public System.Type clazz;
			public object id;
			public ObjectTypeCacheEntry(System.Type clazz, object id) 
			{
				this.clazz = clazz;
				this.id = id;
			}
		}

		public override object Assemble(object cached, ISessionImplementor session, object owner) 
		{
			ObjectTypeCacheEntry e = (ObjectTypeCacheEntry) cached;
			return (cached==null) ? null : session.Load(e.clazz, e.id);
		}

		public override object Disassemble(object value, ISessionImplementor session) 
		{
			return (value==null) ? null : new ObjectTypeCacheEntry( value.GetType(), session.GetEntityIdentifier(value) );
		}

		public override bool IsObjectType {
			get { return true; }
		}

		public Cascades.CascadeStyle Cascade(int i) { 
			return Cascades.CascadeStyle.StyleNone; 
		} 
    
		public OuterJoinLoaderType EnableJoinedFetch(int i) { 
			return OuterJoinLoaderType.Lazy; 
		} 
    
		private static readonly string[] PROPERTY_NAMES = new string[] { "class", "id" }; 
    
		public string[] PropertyNames {
			get {
				return ObjectType.PROPERTY_NAMES; 
			}
		}

		public object GetPropertyValue(Object component, int i, ISessionImplementor session) {    
                   return (i==0) ? 
                           component.GetType() : 
                           Id(component, session); 
        } 
    
        public object[] GetPropertyValues(Object component, ISessionImplementor session) { 
                return new object[] { component.GetType(), Id(component, session) }; 
        } 
    
		private object Id(object component, ISessionImplementor session) { 
				try { 
					return session.GetEntityIdentifierIfNotUnsaved(component); 
				} 
				catch (TransientObjectException) { 
					return null; 
				} 
		} 
	
		public IType[] Subtypes {
			get {
				return new IType[] { metaType, identifierType };
			}
		} 
    
        public void SetPropertyValues(object component, object[] values) { 
                throw new NotSupportedException(); 
        } 
  
		public override bool IsComponentType {
			get {
				return true;
			}
		}

		public ForeignKeyType ForeignKeyType { 
			get {
				//return AssociationType.FOREIGN_KEY_TO_PARENT; //TODO: this is better but causes a transient object exception... 
				return ForeignKeyType.ForeignKeyFromParent; 
			}
		} 

		public override bool IsAssociationType {
			get {
				return true;
			}
		}
	}
}
