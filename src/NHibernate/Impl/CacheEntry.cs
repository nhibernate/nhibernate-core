using System;
using System.Data;
using System.Collections;
using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Engine;


namespace NHibernate.Impl {
	/// <summary>
	/// A cached instance of a persistent class
	/// </summary>
	[Serializable]
	public class CacheEntry {
	
		object[] state;
		System.Type subclass;

		public System.Type Subclass {
			get { return subclass; }
		}

		public CacheEntry(object obj, IClassPersister persister, ISessionImplementor session) {
			state = Disassemble(obj, persister, session);
			subclass = obj.GetType();
		}

		private object[] Disassemble(object obj, IClassPersister persister, ISessionImplementor session) {
			object[] values = persister.GetPropertyValues(obj);
			IType[] propertyTypes = persister.PropertyTypes;
			for (int i=0; i<values.Length; i++) {
				values[i] = propertyTypes[i].Disassemble(values[i], session);
			}
			return values;
		}

		public object[] Assemble(object instance, object id, IClassPersister persister, ISessionImplementor session) {

			if ( subclass!=persister.MappedClass ) throw new AssertionFailure("Tried to assemble a different subclass instance");

			return Assemble(state, instance, id, persister, session);
		}

		private object[] Assemble(object[] values, object result, object id, IClassPersister persister, ISessionImplementor session) {
			IType[] propertyTypes = persister.PropertyTypes;
			object[] assembledProps = new object[propertyTypes.Length];
			for (int i=0; i<values.Length; i++ ) {
				assembledProps[i] = propertyTypes[i].Assemble( values[i], session, result );
			}
			persister.SetPropertyValues(result, assembledProps);
			persister.SetIdentifier(result, id);

			if ( persister.ImplementsLifecycle ) {
				( (ILifecycle) result ).OnLoad(session, id);
			}

			return assembledProps;
		}
	}
}
