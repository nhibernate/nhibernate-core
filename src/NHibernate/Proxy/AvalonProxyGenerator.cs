using System;
using System.Collections;
using System.Reflection;

using Apache.Avalon.DynamicProxy;

using NHibernate.Engine;

namespace NHibernate.Proxy
{
	/// <summary>
	/// An <see cref="IProxyGenerator"/> for the Apache.Avalon.DynamicProxy library.
	/// </summary>
	public class AvalonProxyGenerator : IProxyGenerator
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof(AvalonProxyGenerator) );

		private ProxyGenerator _generator = new ProxyGenerator();

		/// <summary>
		/// Initializes a new instance of the <see cref="AvalonProxyGenerator"/> class.
		/// </summary>
		internal AvalonProxyGenerator()
		{			
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

				object generatedProxy = _generator.CreateProxy( interfaces, initializer );
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
