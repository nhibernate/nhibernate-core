using System;
using System.Data;
using System.Reflection;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Util;
using NHibernate.Loader;

namespace NHibernate.Type {
	
	public class ComponentType : AbstractType, IAbstractComponentType {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ComponentType));

		private System.Type componentClass;
		private ConstructorInfo contructor;
		private IType[] types;
		private ReflectHelper.Getter[] getters;
		private ReflectHelper.Setter[] setters;
		private string[] propertyNames;
		private int propertySpan;
		private Cascades.CascadeStyle[] cascade;
		private OuterJoinLoaderType[] joinedFetch;
		private string parentProperty;
		private ReflectHelper.Setter parentSetter;

		//TODO: implement optimizer
		
		public override DbType[] SqlTypes(IMapping mapping) {
			//not called at runtime so doesn't matter if its slow :)
			DbType[] sqlTypes = new DbType[ GetColumnSpan(mapping) ];
			int n=0;
			for (int i=0;i<propertySpan; i++) {
				DbType[] subtypes = types[i].SqlTypes(mapping);
				for (int j=0; j<subtypes.Length; j++) {
					sqlTypes[n++] = subtypes[j];
				}
			}
			return sqlTypes;
		}

		public override int GetColumnSpan(IMapping mapping) {
			int span = 0;
			for (int i=0; i<propertySpan; i++) {
				span += types[i].GetColumnSpan(mapping);
			}
			return span;
		}

		public ComponentType(	System.Type componentClass,
			string[] properties,
			IType[] types,
			OuterJoinLoaderType[] joinedFetch,
			Cascades.CascadeStyle[] cascade,
			string parentProperty,
			bool embedded ) {
			this.componentClass = componentClass;
			this.types = types;
			propertySpan = properties.Length;
			getters = new ReflectHelper.Getter[propertySpan];
			setters = new ReflectHelper.Setter[propertySpan];
			string[] getterNames = new string[propertySpan];
			string[] setterNames = new string[propertySpan];
			System.Type[] propTypes = new System.Type[propertySpan];
			for (int i=0; i<propertySpan; i++) {
				getters[i] = ReflectHelper.GetGetter( componentClass, properties[i] );
				setters[i] = ReflectHelper.GetSetter( componentClass, properties[i] );
				getterNames[i] = getters[i].Property.Name;
				setterNames[i] = setters[i].Property.Name;
				propTypes[i] = getters[i].Property.PropertyType;
			}
			this.parentSetter = (parentProperty==null) ? null : ReflectHelper.GetSetter(componentClass, parentProperty);
			this.parentProperty = parentProperty;
			this.propertyNames = properties;
			this.cascade = cascade;
			this.joinedFetch = joinedFetch;
			contructor = ReflectHelper.GetDefaultConstructor(componentClass);

		}

		public override bool IsPersistentCollectionType {
			get { return false; }
		}
		public override bool IsComponentType {
			get { return true; }
		}
		public override bool IsEntityType {
			get { return false; }
		}

		public override System.Type ReturnedClass {
			get { return componentClass; }
		}

		public override bool Equals(object x, object y) {
			if (x==y) return true;
			if (x==null || y==null) return false;
			for (int i=0; i<propertySpan; i++) {
				if ( !types[i].Equals( getters[i].Get(x) , getters[i].Get(y) ) ) return false;
			}
			return true;
		}

		public override bool IsDirty(object x, object y, ISessionImplementor session) {
			if (x==y) return false;
			if (x==null || y==null) return true;
			for (int i=0; i<getters.Length; i++) {
				if (types[i].IsDirty( getters[i].Get(x), getters[i].Get(y), session ) ) return true;
			}
			return false;
		}

		public override object NullSafeGet(IDataReader rs, string[] names, ISessionImplementor session, object owner) {
			int begin = 0;
			bool notNull=false;
			object[] values = new object[propertySpan];
			for (int i=0; i<propertySpan; i++) {
				int length = types[i].GetColumnSpan( session.Factory );
				string[] range = ArrayHelper.Slice(names, begin, length);
				object val = types[i].NullSafeGet(rs, range, session, owner);
				if (val!=null) notNull=true;
				values[i] = val;
				begin+=length;
			}

			if (notNull) {
				object result = Instantiate(owner, session);
				for (int i=0; i<propertySpan; i++) {
					setters[i].Set(result, values[i]);
				}
				return result;
			} else {
				return null;
			}
		}

		public override void NullSafeSet(IDbCommand st, object value, int begin, ISessionImplementor session) {
			object[] subvalues = NullSafeGetValues(value);

			for (int i=0; i<propertySpan; i++) {
				types[i].NullSafeSet(st, subvalues[i], begin, session);
				begin += types[i].GetColumnSpan( session.Factory );
			}
		}

		private object[] NullSafeGetValues(object value) {
			if (value==null) {
				return new object[propertySpan];
			} else {
				return GetPropertyValues(value);
			}
		}

		public override object NullSafeGet(IDataReader rs, string name, ISessionImplementor session, object owner) {
			return NullSafeGet(rs, new string[] {name}, session, owner);
		}

		public object GetPropertyValue(object component, int i, ISessionImplementor session) {
			return GetPropertyValue(component, i);
		}

		public object GetPropertyValue(object component, int i) {
			return getters[i].Get(component);
		}

		public object[] GetPropertyValues(object component, ISessionImplementor session) {
			return GetPropertyValues(component);
		}

		public object[] GetPropertyValues(object component) {
			object[] values = new object[propertySpan];
			for (int i=0; i<propertySpan; i++) {
				values[i] = GetPropertyValue(component, i);
			}
			return values;
		}

		public void SetPropertyValues(object component, object[] values) {
			for (int i=0; i<propertySpan; i++) {
				setters[i].Set( component, values[i] );
			}
		}

		public IType[] Subtypes {
			get { return types; }
		}

		public override string Name {
			get { return componentClass.Name; }
		}

		public override string ToXML(object value, ISessionFactoryImplementor factory) {
			return (value==null) ? null : value.ToString();
		}

		public string[] PropertyNames {
			get { return propertyNames; }
		}

		public override object DeepCopy(object component) {
			if (component==null) return null;

			object[] values = GetPropertyValues(component);
			for (int i=0; i<propertySpan; i++) {
				values[i] = types[i].DeepCopy(values[i]);
			}

			object result = Instantiate();
			SetPropertyValues(result, values);
			return result;
		}

		public object Instantiate() {
			try {
				return contructor.Invoke(null);
			} catch (Exception e) {
				throw new InstantiationException("Could not instantiate component: ", componentClass, e);
			}
		}

		public object Instantiate(object parent, ISessionImplementor session) {
			object result = Instantiate();
			try {
				if (parentSetter!=null && parent!=null) parentSetter.Set(result, session.ProxyFor(parent) );
				return result;
			} catch(Exception e) {
				throw new InstantiationException("Could not set component parent for: ",  componentClass, e);
			}
		}

		public Cascades.CascadeStyle Cascade(int i) {
			return cascade[i];
		}

		public override bool IsMutable {
			get { return true; }
		}

		public override object Disassemble(object value, ISessionImplementor session) {
			if (value==null) {
				return null;
			} else {
				object[] values = GetPropertyValues(value);
				for (int i=0; i<types.Length; i++) {
					values[i] = types[i].Disassemble(values[i], session);
				}
				return values;
			}
		}

		public override object Assemble(object obj, ISessionImplementor session, object owner) {
			if (obj==null) {
				return null;
			} else {
				object[] values = (object[]) obj;
				object[] assembled = new object[values.Length];
				for (int i=0; i<types.Length; i++) {
					assembled[i] = types[i].Assemble( values[i], session, owner);
				}
				object result = Instantiate();
				SetPropertyValues(result, assembled);
				return result;
			}
		}

		public override bool HasNiceEquals {
			get { return false; }
		}

		public OuterJoinLoaderType EnableJoinedFetch(int i) {
			return joinedFetch[i];
		}
								

	}
}
