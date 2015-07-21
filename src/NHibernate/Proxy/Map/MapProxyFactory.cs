using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Proxy.Map
{
	public class MapProxyFactory : IProxyFactory
	{
		private string entityName;

		#region IProxyFactory Members

		public void PostInstantiate(string entityName, System.Type persistentClass, ISet<System.Type> interfaces,
																MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
																IAbstractComponentType componentIdType)
		{
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

		#endregion
	}
}
