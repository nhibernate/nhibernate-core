using System;
using System.Reflection;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Proxy {
	
	public class HibernateProxy : RealProxy {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HibernateProxy));

		protected object target = null;
		protected object id;
		protected ISessionImplementor session;
		protected System.Type persistentClass;
		protected PropertyInfo identifierProperty;
		protected bool overridesEquals;

		private HibernateProxy(System.Type type, object id, PropertyInfo identifierProperty, ISessionImplementor session ) : base(type) {
			this.id = id;
			this.session = session;
			this.persistentClass = persistentClass;
			this.identifierProperty = identifierProperty;
			overridesEquals = ReflectHelper.OverridesEquals(persistentClass);
		}

		public void Initialize() {
			if (target==null) {
				if (session==null) {
					throw new HibernateException("Could not initialize proxy - no Session");
				} else if (!session.IsOpen) {
					throw new HibernateException("Could not initialize proxy - the owning Session was closed");
				} else {
					target = session.ImmediateLoad(persistentClass, id);
				}
			}
		}

		private void InitializeWrapExceptions() {
			try {
				Initialize();
			} catch (Exception e) {
				log.Error("Exception initializing proxy", e);
				throw new LazyInitializationException(e);
			}
		}

		protected object SerializableProxy {
			get {
				return this.GetTransparentProxy();
			}
		}

		public override IMessage Invoke(IMessage message) {

			IMethodCallMessage methodMessage = message as IMethodCallMessage;
			//TODO: Are properties not going to be an IMethodCallMessage?

			if (methodMessage != null) {
				MethodBase method = methodMessage.MethodBase;
				string methodName = method.Name;
				int parms = method.GetParameters().Length;

				if ( parms == 0 ) {
					if ( "WriteReplace".Equals(methodName)) {
						if (target==null && session!=null)
							target = session.GetEntity( new Key( id, session.Factory.GetPersister(persistentClass)) );
						if (target==null) {
							return new ReturnMessage(SerializableProxy, null, 0, methodMessage.LogicalCallContext, methodMessage);
						} else {
							return new ReturnMessage(target, null, 0, methodMessage.LogicalCallContext, methodMessage);
						}
					} else if (!overridesEquals && "GetHashCode".Equals(methodName)) {
						//kind bad, since we redefine hashcode of the proxied object
						//necessary for keeing proxies in hashtables without forcing them
						//to be initialized
						return new ReturnMessage(id.GetHashCode(), null, 0, methodMessage.LogicalCallContext, methodMessage);
					} else if (method.Name.Equals(identifierProperty.Name)) {
						//TODO: fix this. the name wont be the same...see what it is
						return new ReturnMessage(id, null, 0, methodMessage.LogicalCallContext, methodMessage);
					} 
					//TODO "finalize?"
					//else if ("Finalize".Equals( method.Name )) {
					//	return null;
					//}
				} else if ( parms==1 && !overridesEquals && "Equals".Equals(methodName)) {
					return new ReturnMessage( id.Equals(identifierProperty.GetValue(methodMessage.Args[0], null)), null, 0, methodMessage.LogicalCallContext, methodMessage );
				}

				// otherwise:
				try {
					object returnValue = method.Invoke(Implementation, methodMessage.Args);
					return new ReturnMessage(returnValue, null, 0, methodMessage.LogicalCallContext, methodMessage);
				} catch (Exception e) {
					return new ReturnMessage(e, methodMessage);
				}
			}
			return null;
		}

		public object Identifier {
			get { return id; }
		}

		public System.Type PersistentClass {
			get { return persistentClass; }
		}

		public bool IsUninitialized {
			get { return target == null; }
		}

		public ISessionImplementor Session {
			get { return session; }
			set {
				if (value!=session) {
					if (session!=null && session.IsOpen ) {
						throw new LazyInitializationException("Illegally attempted to associate a proxy with two open sessions");
					} else {
						session = value;
					}
				}
			}
		}

		public object Implementation {
			get {
				InitializeWrapExceptions();
				return target;
			}
		}

		public object GetImplementation(ISessionImplementor s) {
			return s.GetEntity(
				new Key( Identifier, s.Factory.GetPersister( PersistentClass ) )
				);
		}
	}
}
