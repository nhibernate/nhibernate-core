using System;
using System.Collections;
using System.Reflection;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Builder.CodeGenerators;
using log4net;
using NHibernate.Engine;

namespace NHibernate.Proxy
{
	/// <summary>
	/// An <see cref="IProxyGenerator"/> that uses the Castle.DynamicProxy library.
	/// </summary>
	public class CastleProxyGenerator : IProxyGenerator
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( CastleProxyGenerator ) );

		// key = mapped type
		// value = proxy type
		private IDictionary cachedProxyTypes;

		private GeneratorContext _context;
		private ModuleScope _scope;

		/// <summary>
		/// Initializes a new instance of the <see cref="CastleProxyGenerator"/> class.
		/// </summary>
		internal CastleProxyGenerator()
		{
			cachedProxyTypes = new Hashtable();

			_scope = new ModuleScope();

			// the EnhanceTypeDelegate will add custom code gen that DynamicProxy does not provide
			// by default.
			_context = new GeneratorContext(); // new EnhanceTypeDelegate( EnhanceInterfaceType ), null );
		}

		#region IProxyGenerator Methods

		/// <summary>
		/// Build a proxy using the Castle.DynamicProxy library.
		/// </summary>
		/// <param name="persistentClass">.</param>
		/// <param name="interfaces">The extra interfaces the Proxy should implement.</param>
		/// <param name="identifierPropertyInfo">The PropertyInfo to get/set the Id.</param>
		/// <param name="id">The value for the Id.</param>
		/// <param name="concreteProxy"></param>
		/// <param name="session">The Session the proxy is in.</param>
		/// <returns>A fully built <c>INHibernateProxy</c>.</returns>
		public INHibernateProxy GetProxy( System.Type persistentClass, System.Type concreteProxy, System.Type[ ] interfaces, PropertyInfo identifierPropertyInfo, object id, ISessionImplementor session )
		{
			object generatedProxy = null;
			try
			{
				CastleLazyInitializer initializer = new CastleLazyInitializer( persistentClass, concreteProxy, interfaces, id, identifierPropertyInfo, session );
				System.Type proxyType = null;

				// if I try to generate a proxy twice for the same type the Castle library will do the same
				// in one ModuleBuilder.  The ModuleBuilder throws an exception (not suprisingly) when you try
				// to define the same type twice in it.  So nh needs to keep a cache of the proxy types that have
				// already been generated.
				lock( cachedProxyTypes.SyncRoot )
				{
					proxyType = cachedProxyTypes[ concreteProxy ] as System.Type;

					// if the pc is an interface then we need to add the interface to the 
					// interfaces array that was passed in because it only includes the extra
					// interfaces for that persistent class.
					if( concreteProxy.IsInterface )
					{
						if( proxyType == null )
						{
							//TODO: figure out if this is necessary because the concreteProxy is
							// already included in the interfaces array...
							System.Type[ ] temp = new System.Type[interfaces.Length + 1];
							interfaces.CopyTo( temp, 0 );
							temp[ interfaces.Length ] = concreteProxy; //persistentClass;
							interfaces = temp;

							InterfaceProxyGenerator _interfaceGenerator = new InterfaceProxyGenerator( _scope, _context );
							proxyType = _interfaceGenerator.GenerateCode( interfaces );

							cachedProxyTypes[ concreteProxy ] = proxyType;
						}
						// don't understand why a Interface proxy requires 2 arg constructor and a Class Interface
						// proxy only requires 1 - TODO: email Hammett about this.
						// hack with new object() because an interface proxy is expecting an actual target instance - we
						// don't have that yet and don't want to create it until it is actually needed.
						generatedProxy = Activator.CreateInstance( proxyType, new object[ ] {initializer, new object()} );

					}
					else
					{
						if( proxyType == null )
						{
							CastleCustomProxyGenerator _classGenerator = new CastleCustomProxyGenerator( _scope, _context );
							proxyType = _classGenerator.GenerateCode( concreteProxy );

							cachedProxyTypes[ concreteProxy ] = proxyType;
						}

						generatedProxy = Activator.CreateInstance( proxyType, new object[ ] {initializer} );

					}
				}
				return ( INHibernateProxy ) generatedProxy;

			}
			catch( Exception e )
			{
				log.Error( "Castle Dynamic Class Generator failed", e );
				throw new HibernateException( "Castle Dynamic Class Generator failed", e );
			}
		}

		/// <summary>
		/// Gets the <see cref="LazyInitializer"/> that is used by the Proxy.
		/// </summary>
		/// <param name="proxy">The Proxy object</param>
		/// <returns>The <see cref="LazyInitializer"/> that contains the details of the Proxied object.</returns>
		public LazyInitializer GetLazyInitializer( INHibernateProxy proxy )
		{
			// have to hard code in "__interceptor" - very dependant on them not changing their
			// implementation - TODO: email Hammet about this - or atleast to provide a static
			// field 
			object fieldValue = proxy.GetType().GetField( "__interceptor" ).GetValue( proxy );
			return ( LazyInitializer ) fieldValue;
		}

		/// <summary>
		/// Convenience method to figure out the underlying type for the object regardless of it
		/// is a Proxied object or the real object.
		/// </summary>
		/// <param name="obj">The object to get the type of.</param>
		/// <returns>The Underlying Type for the object regardless of if it is a Proxy.</returns>
		public System.Type GetClass( object obj )
		{
			if( obj is INHibernateProxy )
			{
				INHibernateProxy proxy = ( INHibernateProxy ) obj;
				LazyInitializer li = GetLazyInitializer( proxy );
				return li.PersistentClass;
			}
			else
			{
				return obj.GetType();
			}
		}

		#endregion
	}
}