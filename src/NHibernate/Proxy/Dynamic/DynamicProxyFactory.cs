using NHibernate.Engine;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NHibernate.Proxy.Dynamic
{
    public class DynamicProxyFactory : IProxyFactory
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
            return new DynamicProxy(new DynamicLazyInitializer(entityName, id, session));
        }

        public object GetFieldInterceptionProxy(object getInstance)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
