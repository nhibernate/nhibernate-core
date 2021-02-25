using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Type;

namespace NHibernate.Proxy
{
	public sealed class StaticProxyFactory : AbstractProxyFactory
	{
		private static readonly ConcurrentDictionary<ProxyCacheEntry, Func<ILazyInitializer, NHibernateProxyFactoryInfo, INHibernateProxy>>
			Cache = new ConcurrentDictionary<ProxyCacheEntry, Func<ILazyInitializer, NHibernateProxyFactoryInfo, INHibernateProxy>>();
		private static readonly ConcurrentDictionary<System.Type, Func<NHibernateProxyFactoryInfo, IFieldInterceptorAccessor>>
			FieldInterceptorCache = new ConcurrentDictionary<System.Type, Func<NHibernateProxyFactoryInfo, IFieldInterceptorAccessor>>();

		private static readonly INHibernateLogger Log = NHibernateLogger.For(typeof(StaticProxyFactory));

		private NHibernateProxyFactoryInfo _proxyFactoryInfo;
		private ProxyCacheEntry _cacheEntry;

		public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			try
			{
				var proxyActivator = Cache.GetOrAdd(_cacheEntry, pke => CreateProxyActivator(pke));
				return proxyActivator(
					new LiteLazyInitializer(EntityName, id, session, PersistentClass),
					_proxyFactoryInfo);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Creating a proxy instance failed");
				throw new HibernateException("Creating a proxy instance failed", ex);
			}
		}

		public override void PostInstantiate(
			string entityName,
			System.Type persistentClass,
			ISet<System.Type> interfaces,
			MethodInfo getIdentifierMethod,
			MethodInfo setIdentifierMethod,
			IAbstractComponentType componentIdType,
			bool isClassProxy)
		{
			base.PostInstantiate(entityName, persistentClass, interfaces, getIdentifierMethod, setIdentifierMethod, componentIdType, isClassProxy);

			_proxyFactoryInfo = new NHibernateProxyFactoryInfo(
				EntityName,
				PersistentClass,
				Interfaces,
				GetIdentifierMethod,
				SetIdentifierMethod,
				ComponentIdType,
				IsClassProxy);
			_cacheEntry = new ProxyCacheEntry(IsClassProxy ? PersistentClass : typeof(object), Interfaces);
		}

		private Func<ILazyInitializer, NHibernateProxyFactoryInfo, INHibernateProxy> CreateProxyActivator(ProxyCacheEntry pke)
		{
			var proxyBuilder = new NHibernateProxyBuilder(GetIdentifierMethod, SetIdentifierMethod, ComponentIdType, OverridesEquals);
			var type = proxyBuilder.CreateProxyType(pke.BaseType, pke.Interfaces);
			var ctor = type.GetConstructor(new[] {typeof(ILazyInitializer), typeof(NHibernateProxyFactoryInfo)});
			var li = Expression.Parameter(typeof(ILazyInitializer));
			var pf = Expression.Parameter(typeof(NHibernateProxyFactoryInfo));
			return Expression.Lambda<Func<ILazyInitializer, NHibernateProxyFactoryInfo, INHibernateProxy>>(Expression.New(ctor, li, pf), li, pf).Compile();
		}

		// Since 5.3
		[Obsolete("Use ProxyFactoryExtensions.GetFieldInterceptionProxy extension method instead.")]
		public override object GetFieldInterceptionProxy(object instanceToWrap)
		{
			return GetFieldInterceptionProxy();
		}

		public object GetFieldInterceptionProxy()
		{
			var proxyActivator = FieldInterceptorCache.GetOrAdd(PersistentClass, CreateFieldInterceptionProxyActivator);
			return proxyActivator(_proxyFactoryInfo);
		}

		private Func<NHibernateProxyFactoryInfo, IFieldInterceptorAccessor> CreateFieldInterceptionProxyActivator(System.Type baseType)
		{
			var type = FieldInterceptorProxyBuilder.CreateProxyType(baseType);
			var ctor = type.GetConstructor(new[] { typeof(NHibernateProxyFactoryInfo) });
			var pf = Expression.Parameter(typeof(NHibernateProxyFactoryInfo));
			return Expression.Lambda<Func<NHibernateProxyFactoryInfo, IFieldInterceptorAccessor>>(Expression.New(ctor, pf), pf).Compile();
		}
	}
}
