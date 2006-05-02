using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	/// <summary>
	/// Provides the base functionallity to Handle Member calls into a dynamically
	/// generated NHibernate Proxy.
	/// </summary>
	/// <remarks>
	/// This could be an extension point later if the .net framework ever gets a Proxy
	/// class that is similar to the java.lang.reflect.Proxy or if a library similar
	/// to cglib was made in .net.
	/// </remarks>
	[Serializable]
	public abstract class LazyInitializer
	{
		private static readonly IHashCodeProvider IdentityHashCodeProvider =
			new NHibernate.IdentityHashCodeProvider();

		/// <summary>
		/// If this is returned by Invoke then the subclass needs to Invoke the
		/// method call against the object that is being proxied.
		/// </summary>
		protected static readonly object InvokeImplementation = new object();

		private object _target = null;
		private bool initialized;
		private object _id;

		[NonSerialized]
		private ISessionImplementor _session;

		private System.Type _persistentClass;
		private MethodInfo _getIdentifierMethod;
		private MethodInfo _setIdentifierMethod;
		private bool _overridesEquals;

		/// <summary>
		/// Create a LazyInitializer to handle all of the Methods/Properties that are called
		/// on the Proxy.
		/// </summary>
		/// <param name="persistentClass">The Class to Proxy.</param>
		/// <param name="id">The Id of the Object we are Proxying.</param>
		/// <param name="getIdentifierMethod"></param>
		/// <param name="setIdentifierMethod"></param>
		/// <param name="session">The ISession this Proxy is in.</param>
		protected LazyInitializer( System.Type persistentClass, object id,
			MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
			ISessionImplementor session )
		{
			_id = id;
			_session = session;
			_persistentClass = persistentClass;
			_getIdentifierMethod = getIdentifierMethod;
			_setIdentifierMethod = setIdentifierMethod;
			
			_overridesEquals = ReflectHelper.OverridesEquals( _persistentClass );
		}

		/// <summary>
		/// Perform an ImmediateLoad of the actual object for the Proxy.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the Proxy has no Session or the Session is closed or disconnected.
		/// </exception>
		public void Initialize()
		{
			if( !initialized )
			{
				if( _session == null )
				{
					throw new LazyInitializationException( "Could not initialize proxy - no Session." );
				}
				else if( !_session.IsOpen )
				{
					throw new LazyInitializationException( "Could not initialize proxy - the owning Session was closed." );
				}
				else if( !_session.IsConnected )
				{
					throw new LazyInitializationException( "Could not initialize proxy - the owning Session is disconnected." );
				}
				else
				{
					_target = _session.ImmediateLoad( _persistentClass, _id );
					initialized = true;
				}
			}
		}

		/// <summary>
		/// Adds all of the information into the SerializationInfo that is needed to
		/// reconstruct the proxy during deserialization or to replace the proxy
		/// with the instantiated target.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo"/> to write the object to.</param>
		/// <remarks>
		/// This will only be called if the Dynamic Proxy generator does not handle serialization
		/// itself or delegates calls to the method GetObjectData to the LazyInitializer.
		/// </remarks>
		protected virtual void AddSerializationInfo( SerializationInfo info, StreamingContext context )
		{
		}

		/// <summary>
		/// Invokes the method if this is something that the LazyInitializer can handle
		/// without the underlying proxied object being instantiated.
		/// </summary>
		/// <param name="method">The name of the method/property to Invoke.</param>
		/// <param name="args">The arguments to pass the method/property.</param>
		/// <returns>
		/// The result of the Invoke if the underlying proxied object is not needed.  If the 
		/// underlying proxied object is needed then it returns the result <see cref="InvokeImplementation"/>
		/// which indicates that the Proxy will need to forward to the real implementation.
		/// </returns>
		public virtual object Invoke( MethodBase method, object[ ] args, object proxy )
		{
			string methodName = method.Name;
			int paramCount = method.GetParameters().Length;

			if( paramCount == 0 )
			{
				if( !_overridesEquals && methodName == "GetHashCode" )
				{
					return IdentityHashCodeProvider.GetHashCode( proxy );
				}
				else if( method.Equals( _getIdentifierMethod ) )
				{
					return _id;
				}
				else if( methodName == "Finalize" )
				{
					return null;
				}
			}
			else if( paramCount == 1 )
			{
				if( !_overridesEquals && methodName == "Equals" )
				{
					return args[0] == proxy;
				}
				else if( method.Equals( _setIdentifierMethod ) )
				{
					Initialize();
					_id = args[ 0 ];
					return InvokeImplementation;
				}
			}
			else if( paramCount == 2)
			{
				// if the Proxy Engine delegates the call of GetObjectData to the Initializer
				// then we need to handle it.  Castle.DynamicProxy takes care of serializing
				// proxies for us, but other providers might not.
				if( methodName == "GetObjectData" )
				{
					SerializationInfo info = ( SerializationInfo ) args[ 0 ];
					StreamingContext context = ( StreamingContext ) args[ 1 ]; // not used !?!

					if( _target == null & _session != null )
					{
						EntityKey key = new EntityKey( _id, _session.Factory.GetEntityPersister( _persistentClass ) );
						_target = _session.GetEntity( key );
					}

					// let the specific LazyInitializer write its requirements for deserialization 
					// into the stream.
					AddSerializationInfo( info, context );

					// don't need a return value for proxy.
					return null;
				}
			}

			return InvokeImplementation;
		}

		/// <summary></summary>
		public object Identifier
		{
			get { return _id; }
			set { _id = value; }
		}

		/// <summary></summary>
		public System.Type PersistentClass
		{
			get { return _persistentClass; }
		}

		/// <summary></summary>
		public bool IsUninitialized
		{
			get { return ( _target == null ); }
		}

		/// <summary></summary>
		public ISessionImplementor Session
		{
			get { return _session; }
			set
			{
				if( value != _session )
				{
					if( _session != null && _session.IsOpen )
					{
						//TODO: perhaps this should be some other RuntimeException...
						throw new LazyInitializationException( "Illegally attempted to associate a proxy with two open Sessions" );
					}
					else
					{
						_session = value;
					}
				}
			}
		}

		/// <summary>
		/// Return the Underlying Persistent Object, initializing if necessary.
		/// </summary>
		/// <returns>The Persistent Object this proxy is Proxying.</returns>
		public object GetImplementation()
		{
			Initialize();
			return _target;
		}

		/// <summary>
		/// Return the Underlying Persistent Object in a given <see cref="ISession"/>, or null.
		/// </summary>
		/// <param name="s">The Session to get the object from.</param>
		/// <returns>The Persistent Object this proxy is Proxying, or <c>null</c>.</returns>
		public object GetImplementation( ISessionImplementor s )
		{
			EntityKey key = new EntityKey( Identifier, s.Factory.GetEntityPersister( PersistentClass ) );
			return s.GetEntity( key );
		}

		public void SetImplementation( object target )
		{
			this._target = target;
			initialized = true;
		}

	}
}
