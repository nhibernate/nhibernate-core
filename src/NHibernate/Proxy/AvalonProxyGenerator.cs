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

		private ProxyGenerator _generator;
		private DefaultProxyBuilder _defaultBuilder;
		private GeneratorContext _context;

		/// <summary>
		/// Initializes a new instance of the <see cref="AvalonProxyGenerator"/> class.
		/// </summary>
		internal AvalonProxyGenerator()
		{
			_defaultBuilder = new DefaultProxyBuilder( );

			// the EnhanceTypeDelegate will add custom code gen that DynamicProxy does not provide
			// by default.
			_context = new GeneratorContext( new EnhanceTypeDelegate( EnhanceInterfaceType ), null );

			// only create the genator once so I should be okay with the DefaultProxyBuilder
			// maintaining the ModuleScope
			_generator = new ProxyGenerator();

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
			ConstructorInfo serAttConstructor = typeof(SerializableAttribute).GetConstructor( new System.Type[0] );
			CustomAttributeBuilder serializableAttBuilder = new CustomAttributeBuilder( serAttConstructor, new object[0] );
			mainType.SetCustomAttribute( serializableAttBuilder );
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
		public INHibernateProxy GetProxy(System.Type persistentClass, System.Type[] interfaces, PropertyInfo identifierPropertyInfo, object id, ISessionImplementor session) 
		{
			
			try 
			{	
				AvalonLazyInitializer initializer = new AvalonLazyInitializer( persistentClass, interfaces, id, identifierPropertyInfo, session );

				// if the pc is an interface then we need to add the interface to the 
				// interfaces array that was passed in because it only includes the extra
				// interfaces for that persistent class.
				if( persistentClass.IsInterface ) 
				{
					System.Type[] temp = new System.Type[ interfaces.Length + 1 ];
					interfaces.CopyTo( temp, 0 );
					temp[ interfaces.Length ] = persistentClass;
					interfaces = temp;
				}

				System.Type proxyType = _defaultBuilder.CreateCustomInterfaceProxy( interfaces, _context ); 
				object generatedProxy = Activator.CreateInstance( proxyType, new object[] { initializer } );
				return (INHibernateProxy)generatedProxy;

			}
			catch(Exception e) 
			{
				log.Error("Avalon Dynamic Class Generator failed", e);
				throw new HibernateException( "Avalon Dynamic Class Generator failed", e);
			}
		}

		#endregion
	}
}
