using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

using Apache.Avalon.DynamicProxy;
using Apache.Avalon.DynamicProxy.Builder;
using Apache.Avalon.DynamicProxy.Builder.CodeGenerators;

using NHibernate.Engine;

namespace NHibernate.Proxy
{
	/// <summary>
	/// An <see cref="IProxyGenerator"/> that uses the Apache.Avalon.DynamicProxy library.
	/// </summary>
	public class AvalonProxyGenerator : IProxyGenerator
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof(AvalonProxyGenerator) );

		// key = mapped type
		// value = proxy type
		private IDictionary cachedProxyTypes;

		private GeneratorContext _context;
		private ModuleScope _scope;

		/// <summary>
		/// Initializes a new instance of the <see cref="AvalonProxyGenerator"/> class.
		/// </summary>
		internal AvalonProxyGenerator()
		{
			cachedProxyTypes = new Hashtable();

			_scope = new ModuleScope();

			// the EnhanceTypeDelegate will add custom code gen that DynamicProxy does not provide
			// by default.
			_context = new GeneratorContext( new EnhanceTypeDelegate( EnhanceInterfaceType ), null );
		}

		/// <summary>
		/// Marks the Proxy with the <see cref="SerializableAttribute"/>.
		/// </summary>
		/// <param name="mainType"></param>
		/// <param name="handlerFieldBuilder"></param>
		/// <param name="constructorBuilder"></param>
		/// <remarks>
		/// The proxy itself is not really serializable, it is replaced with a different object during
		/// serialization.  This object knows how to recreate the proxy during the deserialization
		/// process.
		/// </remarks>
		private void EnhanceInterfaceType(TypeBuilder mainType, FieldBuilder handlerFieldBuilder, ConstructorBuilder constructorBuilder)
		{
			bool isSerializableAttFound = false;
			System.Type baseType = mainType.BaseType;

			// I don't know why I can't find the [Serializable] on the generated type
			if( isSerializableAttFound==false ) 
			{
				ConstructorInfo serAttConstructor = typeof(SerializableAttribute).GetConstructor( new System.Type[0] );
				CustomAttributeBuilder serializableAttBuilder = new CustomAttributeBuilder( serAttConstructor, new object[0] );
				mainType.SetCustomAttribute( serializableAttBuilder );
			}
		}

		#region IProxyGenerator Methods

		/// <summary>
		/// Build a proxy using the Apache.Avalon.DynamicProxy library.
		/// </summary>
		/// <param name="persistentClass">.</param>
		/// <param name="interfaces">The extra interfaces the Proxy should implement.</param>
		/// <param name="identifierPropertyInfo">The PropertyInfo to get/set the Id.</param>
		/// <param name="id">The value for the Id.</param>
		/// <param name="session">The Session the proxy is in.</param>
		/// <returns>A fully built <c>INHibernateProxy</c>.</returns>
		public INHibernateProxy GetProxy(System.Type persistentClass, System.Type concreteProxy, System.Type[] interfaces, PropertyInfo identifierPropertyInfo, object id, ISessionImplementor session) 
		{
			
			try 
			{	
				AvalonLazyInitializer initializer = new AvalonLazyInitializer( persistentClass, concreteProxy, interfaces, id, identifierPropertyInfo, session );
				System.Type proxyType = null;

				// if I try to generate a proxy twice for the same type the Avalon library will do the same
				// in one ModuleBuilder.  The ModuleBuilder throws an exception (not suprisingly) when you try
				// to define the same type twice in it.  So nh needs to keep a cache of the proxy types that have
				// already been generated.
				lock( cachedProxyTypes.SyncRoot ) 
				{
					proxyType = cachedProxyTypes[ concreteProxy ] as System.Type;

					if( proxyType==null ) 
					{
						// if the pc is an interface then we need to add the interface to the 
						// interfaces array that was passed in because it only includes the extra
						// interfaces for that persistent class.
						//if( persistentClass.IsInterface ) 
						if( concreteProxy.IsInterface )
						{
							//TODO: figure out if this is necessary because the concreteProxy is
							// already included in the interfaces array...
							System.Type[] temp = new System.Type[ interfaces.Length + 1 ];
							interfaces.CopyTo( temp, 0 );
							temp[ interfaces.Length ] = concreteProxy; //persistentClass;
							interfaces = temp;
					
							InterfaceProxyGenerator _interfaceGenerator = new InterfaceProxyGenerator( _scope, _context );
							proxyType = _interfaceGenerator.GenerateCode( interfaces ); 
				
						}
						else 
						{
							AvalonCustomProxyGenerator _classGenerator = new AvalonCustomProxyGenerator( _scope, _context );
							proxyType = _classGenerator.GenerateCode( concreteProxy );
						}

						cachedProxyTypes[ concreteProxy ] = proxyType;
					}
				}
				object generatedProxy = Activator.CreateInstance( proxyType, new object[] { initializer } );
				return (INHibernateProxy)generatedProxy;

			}
			catch(Exception e) 
			{
				log.Error("Avalon Dynamic Class Generator failed", e);
				throw new HibernateException( "Avalon Dynamic Class Generator failed", e);
			}
		}

		/// <summary>
		/// Gets the <see cref="LazyInitializer"/> that is used by the Proxy.
		/// </summary>
		/// <param name="proxy">The Proxy object</param>
		/// <returns>The <see cref="LazyInitializer"/> that contains the details of the Proxied object.</returns>
		public LazyInitializer GetLazyInitializer(INHibernateProxy proxy) 
		{
			// have to hard code in "handler" - very dependant on them not changing their
			// implementation - email Hammet about this - or atleast to provide a static
			// field 
			object fieldValue = proxy.GetType().GetField( "handler" ).GetValue( proxy );
			return (LazyInitializer)fieldValue;
		}

		/// <summary>
		/// Convenience method to figure out the underlying type for the object regardless of it
		/// is a Proxied object or the real object.
		/// </summary>
		/// <param name="obj">The object to get the type of.</param>
		/// <returns>The Underlying Type for the object regardless of if it is a Proxy.</returns>
		public System.Type GetClass(object obj) 
		{
			if (obj is INHibernateProxy) 
			{
				INHibernateProxy proxy = (INHibernateProxy) obj;
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
