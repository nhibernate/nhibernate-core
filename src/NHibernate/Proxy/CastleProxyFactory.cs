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
		private static readonly ILog log = LogManager.GetLogger( typeof( CastleProxyGenerator ) );
		private static readonly ModuleScope _moduleScope = new ModuleScope();
		private readonly InterfaceProxyGenerator _interfaceProxyGenerator = new InterfaceProxyGenerator(_moduleScope);
		private readonly ClassProxyGenerator _classProxyGenerator = new ClassProxyGenerator(_moduleScope);

		private System.Type _persistentClass;
		private System.Type[ ] _interfaces;
		private MethodInfo _getIdentifierMethod;
		private MethodInfo _setIdentifierMethod;
		private object _lockObject = new object();
		private System.Type _generatedProxyType;

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

		private System.Type GenerateProxyType()
		{
			try
			{
				if( IsClassProxy )
				{
					return _classProxyGenerator.GenerateCode( _persistentClass, _interfaces );
				}
				else
				{
					return _interfaceProxyGenerator.GenerateCode( _interfaces, typeof( object ) );
				}
			}
			catch( Exception e )
			{
				log.Error( "Castle Dynamic Class Generator failed", e );
				throw new HibernateException( "Castle Dynamic Class Generator failed", e );
			}
		}

		private void EnsureProxyTypeGenerated()
		{
			lock( _lockObject )
			{
				if( _generatedProxyType == null )
				{
					_generatedProxyType = GenerateProxyType();
				}
			}
		}

		/// <summary>
		/// Build a proxy using the Castle.DynamicProxy library.
		/// </summary>
		/// <param name="id">The value for the Id.</param>
		/// <param name="session">The Session the proxy is in.</param>
		/// <returns>A fully built <c>INHibernateProxy</c>.</returns>
		public INHibernateProxy GetProxy( object id, ISessionImplementor session )
		{
			EnsureProxyTypeGenerated();

			try
			{
				object generatedProxy = null;
				CastleLazyInitializer initializer = new CastleLazyInitializer( _persistentClass, id,
					_getIdentifierMethod, _setIdentifierMethod, session );

				if( IsClassProxy )
				{
					generatedProxy = Activator.CreateInstance( _generatedProxyType, new object[ ] { initializer } );
				}
				else
				{
					generatedProxy = Activator.CreateInstance( _generatedProxyType, new object[ ] { initializer, new object() } );
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