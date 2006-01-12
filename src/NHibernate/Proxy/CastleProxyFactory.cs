using System;
using System.Collections;
using System.Reflection;

using Castle.DynamicProxy;
using Castle.DynamicProxy.Builder.CodeGenerators;

using Iesi.Collections;

using log4net;

using NHibernate.Engine;

namespace NHibernate.Proxy
{
	public class CastleProxyFactory : IProxyFactory
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( CastleProxyFactory ) );
		private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

		private System.Type _persistentClass;
		private System.Type[ ] _interfaces;
		private MethodInfo _getIdentifierMethod;
		private MethodInfo _setIdentifierMethod;

		public void PostInstantiate( System.Type persistentClass, ISet interfaces,
			MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod )
		{
			_persistentClass = persistentClass;
			_interfaces = new System.Type[ interfaces.Count ];
			interfaces.CopyTo( _interfaces, 0 );
			_getIdentifierMethod = getIdentifierMethod;
			_setIdentifierMethod = setIdentifierMethod;
		}

		private bool IsClassProxy
		{
			get { return _interfaces.Length == 1; }
		}

		/// <summary>
		/// Build a proxy using the Castle.DynamicProxy library.
		/// </summary>
		/// <param name="id">The value for the Id.</param>
		/// <param name="session">The Session the proxy is in.</param>
		/// <returns>A fully built <c>INHibernateProxy</c>.</returns>
		public INHibernateProxy GetProxy( object id, ISessionImplementor session )
		{
			try
			{
				CastleLazyInitializer initializer = new CastleLazyInitializer( _persistentClass, id,
					_getIdentifierMethod, _setIdentifierMethod, session );
				
				object generatedProxy = null;

				if( IsClassProxy )
				{
					generatedProxy = _proxyGenerator.CreateClassProxy( _persistentClass, _interfaces, initializer, false );
				}
				else
				{
					generatedProxy = _proxyGenerator.CreateProxy( _interfaces, initializer, new object() );
				}

				initializer._constructed = true;
				return ( INHibernateProxy ) generatedProxy;
			}
			catch( Exception e )
			{
				log.Error( "Creating a proxy instance failed", e );
				throw new HibernateException( "Creating a proxy instance failed", e );
			}
		}
	}
}