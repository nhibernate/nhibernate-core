using System;
using System.Data;

using NHibernate.Type;
using NHibernate.Engine;
using NHibernate.Loader;

using log4net;

namespace NHibernate.Type {
	/// <summary>
	/// Summary description for CompositeCustomType.
	/// </summary>
	public class CompositeCustomType : AbstractType, IAbstractComponentType {
		private readonly ICompositeUserType userType;
		private readonly string name;

		public CompositeCustomType(System.Type userTypeClass) {

			name = userTypeClass.Name;
		
			try {
				userType = (ICompositeUserType) Activator.CreateInstance(userTypeClass);
			}
			catch (MethodAccessException mae) {
				throw new MappingException( "MethodAccessException trying to instantiate custom type: " + name, mae);
			}
			catch (System.Reflection.TargetInvocationException tie) {
				throw new MappingException( "TargetInvocationException trying to instantiate custom type: " + name, tie);
			}
			catch (ArgumentException ae) {
				throw new MappingException( "ArgumentException trying to instantiate custom type: " + name, ae);
			}
			catch (InvalidCastException ice) {
				throw new MappingException( name + " must implement NHibernate.ICompositeUserType", ice);
			}
			if ( !userType.ReturnedClass.IsSerializable )  {
				LogManager.GetLogger( typeof(CustomType) ).Warn("custom type does not implement Serializable: " + userTypeClass);
			}
		}			

		public IType[] Subtypes {
			get {
				return userType.PropertyTypes;
			}
		}

		public string[] PropertyNames {
			get {
				return userType.PropertyNames;
			}
		}

		public object[] GetPropertyValues(object component, ISessionImplementor session) {
			int len = Subtypes.Length;
			object[] result = new object[len];
			for ( int i=0; i<len; i++ ) {
				result[i] = GetPropertyValue(component, i, session);
			}
			return result;
		}

		public void SetPropertyValues(Object component, Object[] values) {
			for (int i=0; i<values.Length; i++) {
				userType.SetPropertyValue( component, i, values[i] );
			}
		}

		public object GetPropertyValue(object component, int i, ISessionImplementor session) {
			return userType.GetPropertyValue(component, i);
		}

		public Cascades.CascadeStyle Cascade(int i) {
			return Cascades.CascadeStyle.StyleNone;
		}


		public OuterJoinLoaderType EnableJoinedFetch(int i) {
			return OuterJoinLoaderType.Auto;
		}

		public override bool IsComponentType {
			get { return true; }
		}

		public override object Assemble(object cached, ISessionImplementor session,
			object owner) {
			return userType.Assemble(cached, session, owner);
		}

		public override object DeepCopy(object value) {
			return userType.DeepCopy(value);
		}

		public override object Disassemble(object value, ISessionImplementor session) {
			return userType.Disassemble(value, session);
		}

		public override bool Equals(object x, object y) {
			return userType.Equals(x, y);
		}

		public override int GetColumnSpan(IMapping mapping) {
			IType[] types = userType.PropertyTypes;
			int n=0;
			for (int i=0; i<types.Length; i++) {
				n+=types[i].GetColumnSpan(mapping); //Can nested type cause recursion???
			}
			return n;
		}

		public override string Name {
			get { return name; }
		}

		public override System.Type ReturnedClass {
			get {
				return userType.ReturnedClass;
			}
		}

		public override bool HasNiceEquals {
			get { return false; }
		}

		public override bool IsMutable {
			get { return userType.IsMutable; }
		}

		public override object NullSafeGet( IDataReader rs, string name, ISessionImplementor session, object owner) {
			return userType.NullSafeGet(rs, new string[] {name}, session, owner);
		}

		public override object NullSafeGet( IDataReader rs, string[] names, ISessionImplementor session, object owner ) {
			return userType.NullSafeGet(rs, names, session, owner);
		}

		public override void NullSafeSet(
			IDbCommand cmd,
			object value,
			int index,
			ISessionImplementor session) {
			userType.NullSafeSet(cmd, value, index, session);
		}

		public override DbType[] SqlTypes(IMapping mapping) {

			IType[] types = userType.PropertyTypes;
			DbType[] result = new DbType[ GetColumnSpan(mapping) ];
			int n=0;
			for (int i=0; i<types.Length; i++) {
				DbType[] sqlTypes = types[i].SqlTypes(mapping);
				for ( int k=0; k<sqlTypes.Length; k++ ) result[n++] = sqlTypes[k];
			}
			return result;
		}

		public override string ToXML(object value, ISessionFactoryImplementor factory) {
			return value.ToString();
		}
	}
}
