using System;
using System.Reflection;

using NHibernate.Engine;
using NHibernate.Util;

using log4net;

namespace NHibernate.Proxy
{
	/// <summary>
	/// Summary description for LazyInitializer.
	/// </summary>
	public abstract class LazyInitializer 
	{
		protected static readonly object INVOKE_IMPLEMENTATION = new object();
		
		private object target = null;
		private object id;
		private ISessionImplementor session;
		private System.Type persistentClass;
		private MethodInfo getIdentifierMethod;
		private bool overridesEquals;

		public LazyInitializer(System.Type persistentClass, object id, MethodInfo getIdentifierMethod, ISessionImplementor session) {
			this.id = id;
			this.session = session;
			this.persistentClass = persistentClass;
			this.getIdentifierMethod = getIdentifierMethod;
			overridesEquals = ReflectHelper.OverridesEquals(persistentClass);
		}
		
		public void Initialize() {
			if (target==null) {
				if ( session==null ) {
					throw new HibernateException("Could not initialize proxy - no Session");
				}
				else if ( !session.IsOpen ) {
					throw new HibernateException("Could not initialize proxy - the owning Session was closed");
				}
				else {
					target = session.ImmediateLoad(persistentClass, id);
				}
			}
		}

		private void InitializeWrapExceptions() {
			try {
				Initialize();
			}
			catch (Exception e) {
				LogManager.GetLogger( typeof(LazyInitializer) ).Error("Exception initializing proxy", e);
				throw new LazyInitializationException(e);
			}
		}

		protected abstract object SerializableProxy();

		protected object Invoke(MethodInfo method, object[] args) {
		
			string methodName = method.Name;
			int pars = method.GetParameters().Length;

			if ( pars==0 ) {

				if ( "WriteReplace".Equals(methodName) ) {

					if (target==null && session!=null ) target = session.GetEntity(
															new Key( id, session.Factory.GetPersister(persistentClass) )
															);
					if (target==null) {
						/*if ( session==null || !session.isOpen() ) {
							return session.getFactory().getPersister(persistentClass).instantiate(id); //A little "empty" object
						}
						else {*/
						return SerializableProxy();
						//}
					}
					else {
						return target;
					}

				}
				else if ( !overridesEquals && getIdentifierMethod!=null && "GetHashCode".Equals(methodName) ) {
					// kinda dodgy, since it redefines the hashcode of the proxied object.
					// but necessary if we are to keep proxies in HashSets without
					// forcing them to be initialized
					return id.GetHashCode();
				}
				else if ( method.Equals(getIdentifierMethod) ) {
					return id;
				}
				else if ( "Finalize".Equals( methodName ) ) {
					return null;
				}

			}
			else if ( pars==1 && !overridesEquals && getIdentifierMethod!=null && "Equals".Equals(methodName) ) {
				// less dodgy because Hibernate forces == to be same as identifier equals
				return (bool) id.Equals( getIdentifierMethod.Invoke( args[0], null ) );
			}

			// otherwise:
			return INVOKE_IMPLEMENTATION;

			/*try {
				return method.invoke( getImplementation(), args );
			}
			catch (InvocationTargetException ite) {
				throw ite.getTargetException();
			}*/

		}

		public object Identifier {
			get {
				return id;
			}
		}
		
		public System.Type PersistentClass {
			get {
				return persistentClass;
			}
		}
		
		public bool IsUninitialized {
			get {
				return target == null;
			}
		}
		
		public ISessionImplementor Session {
			get {
				return session;
			}
		}
		
		public void SetSession(ISessionImplementor s) {
			if (s!=session) {
				if ( session!=null && session.IsOpen ) {
					//TODO: perhaps this should be some other RuntimeException...
					throw new LazyInitializationException("Illegally attempted to associate a proxy with two open Sessions");
				}
				else {
					session = s;
				}
			}
		}

		/// <summary>
		/// Return the underlying persistent object, initializing if necessary
		/// </summary>
		public object GetImplementation() {
				InitializeWrapExceptions();
				return target;
		}
		
		/// <summary>
		/// Return the underlying persistent object in the given Session, or null
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public object GetImplementation(ISessionImplementor s) {
			return s.GetEntity( new Key(
				Identifier,
				s.Factory.GetPersister( PersistentClass )
			) );
		}
	}
}