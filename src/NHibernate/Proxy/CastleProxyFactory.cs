using System;
using System.Reflection;
using Castle.DynamicProxy;
using Iesi.Collections;
using log4net;
using NHibernate.Engine;

namespace NHibernate.Proxy
{
	public class CastleProxyFactory : IProxyFactory
	{
		protected static readonly ILog log = LogManager.GetLogger(typeof(CastleProxyFactory));
		protected static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

		protected System.Type _persistentClass;
		protected System.Type[] _interfaces;
		protected MethodInfo _getIdentifierMethod;
		protected MethodInfo _setIdentifierMethod;

		public virtual void PostInstantiate(System.Type persistentClass, ISet interfaces,
		                            MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod)
		{
			_persistentClass = persistentClass;
			_interfaces = new System.Type[interfaces.Count];
			interfaces.CopyTo(_interfaces, 0);
			_getIdentifierMethod = getIdentifierMethod;
			_setIdentifierMethod = setIdentifierMethod;
		}

		protected bool IsClassProxy
		{
			get { return _interfaces.Length == 1; }
		}

		/// <summary>
		/// Build a proxy using the Castle.DynamicProxy library.
		/// </summary>
		/// <param name="id">The value for the Id.</param>
		/// <param name="session">The Session the proxy is in.</param>
		/// <returns>A fully built <c>INHibernateProxy</c>.</returns>
		public virtual INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				CastleLazyInitializer initializer = new CastleLazyInitializer(_persistentClass, id,
				                                                              _getIdentifierMethod, _setIdentifierMethod, session);

				object generatedProxy = null;

				if (IsClassProxy)
				{
					generatedProxy = _proxyGenerator.CreateClassProxy(_persistentClass, _interfaces, initializer);
					//generatedProxy = _proxyGenerator.CreateClassProxy(_persistentClass, _interfaces, initializer, false);
				}
				else
				{
					generatedProxy = _proxyGenerator.CreateInterfaceProxyWithoutTarget(_interfaces[0], _interfaces, initializer);
					//generatedProxy = _proxyGenerator.CreateProxy(_interfaces, initializer, new object());
				}

				initializer._constructed = true;
				return (INHibernateProxy) generatedProxy;
			}
			catch (Exception e)
			{
				log.Error("Creating a proxy instance failed", e);
				throw new HibernateException("Creating a proxy instance failed", e);
			}
		}
	}
}
