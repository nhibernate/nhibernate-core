using System;
using System.Reflection;
using Iesi.Collections.Generic;
using log4net;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.ByteCode.LinFu
{
	public class ProxyFactory : IProxyFactory
	{
		private static readonly global::LinFu.DynamicProxy.ProxyFactory factory = new global::LinFu.DynamicProxy.ProxyFactory();
		protected static readonly ILog log = LogManager.GetLogger(typeof (ProxyFactory));

		protected System.Type PersistentClass { get; private set; }

		protected System.Type[] Interfaces { get; private set; }

		protected MethodInfo GetIdentifierMethod { get; private set; }

		public MethodInfo SetIdentifierMethod { get; private set; }

		protected IAbstractComponentType ComponentIdType { get; private set; }

		protected string EntityName { get; private set; }

		protected bool IsClassProxy
		{
			get { return Interfaces.Length == 1; }
		}

		#region IProxyFactory Members

		public void PostInstantiate(string entityName, System.Type persistentClass, ISet<System.Type> interfaces,
		                            MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
		                            IAbstractComponentType componentIdType)
		{
			EntityName = entityName;
			PersistentClass = persistentClass;
			Interfaces = new System.Type[interfaces.Count];

			if (interfaces.Count > 0)
			{
				interfaces.CopyTo(Interfaces, 0);
			}

			GetIdentifierMethod = getIdentifierMethod;
			SetIdentifierMethod = setIdentifierMethod;
			ComponentIdType = componentIdType;
		}

		public INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				var initializer = new LazyInitializer(EntityName, PersistentClass, id, GetIdentifierMethod, SetIdentifierMethod,
				                                      ComponentIdType, session);

				object proxyInstance = IsClassProxy ? factory.CreateProxy(PersistentClass, initializer, Interfaces)
					: factory.CreateProxy(Interfaces[0], initializer, Interfaces);

				return (INHibernateProxy) proxyInstance;
			}
			catch (Exception ex)
			{
				log.Error("Creating a proxy instance failed", ex);
				throw new HibernateException("Creating a proxy instance failed", ex);
			}
		}

		#endregion
	}
}