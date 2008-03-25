using System;
using System.Reflection;
using Castle.DynamicProxy;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Proxy.Poco.Castle
{
	public class CastleProxyFactory : IProxyFactory
	{
		protected static readonly ILog log = LogManager.GetLogger(typeof(CastleProxyFactory));
		private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

		private System.Type _persistentClass;
		private System.Type[] _interfaces;
		private MethodInfo _getIdentifierMethod;
		private MethodInfo _setIdentifierMethod;
		private string _entityName;
		private IAbstractComponentType _componentIdType;

		public virtual void PostInstantiate(string entityName, System.Type persistentClass, ISet<System.Type> interfaces,
		                                    MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
		                                    IAbstractComponentType componentIdType)
		{
			_entityName = entityName;
			_persistentClass = persistentClass;
			_interfaces = new System.Type[interfaces.Count];
			interfaces.CopyTo(_interfaces, 0);
			_getIdentifierMethod = getIdentifierMethod;
			_setIdentifierMethod = setIdentifierMethod;
			_componentIdType = componentIdType;
		}

		protected static ProxyGenerator DefaultProxyGenerator
		{
			get	{return _proxyGenerator;}
		}

		protected System.Type PersistentClass
		{
			get { return _persistentClass; }
		}

		protected System.Type[] Interfaces
		{
			get { return _interfaces; }
		}

		protected MethodInfo GetIdentifierMethod
		{
			get { return _getIdentifierMethod; }
		}

		protected MethodInfo SetIdentifierMethod
		{
			get { return _setIdentifierMethod; }
		}

		protected bool IsClassProxy
		{
			get { return _interfaces.Length == 1; }
		}

		public string EntityName
		{
			get { return _entityName; }
		}

		public IAbstractComponentType ComponentIdType
		{
			get { return _componentIdType; }
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
				CastleLazyInitializer initializer =
					new CastleLazyInitializer(EntityName, _persistentClass, id, _getIdentifierMethod, _setIdentifierMethod,
					                          ComponentIdType, session);

				object generatedProxy;

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