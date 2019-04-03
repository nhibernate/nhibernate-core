using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Proxy.Map
{
	public class MapProxyFactory : IProxyFactory
	{
		//6.0 TODO: make readonly
		private string entityName;

		//Since v5.3
		[Obsolete("Please use constructor accepting entityName instead.")]
		public MapProxyFactory()
		{
		}

		public MapProxyFactory(string entityName)
		{
			this.entityName = entityName;
		}

		//Since v5.3
		[Obsolete("Please use constructor accepting entityName instead.")]
		public void PostInstantiate(string entityName, System.Type persistentClass, ISet<System.Type> interfaces,
																MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
																IAbstractComponentType componentIdType)
		{
			//6.0 TODO: throw NotSupportedException in the new override
			this.entityName = entityName;
		}

		public INHibernateProxy GetProxy(object id, ISessionImplementor session)
		{
			return new MapProxy(new MapLazyInitializer(entityName, id, session));
		}

		public object GetFieldInterceptionProxy(object getInstance)
		{
			throw new NotSupportedException();
		}
	}
}
