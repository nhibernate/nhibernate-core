using System;
using System.Reflection;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.PostSharp.Proxies
{
    public class PostSharpProxyFactory : IProxyFactory
    {
        private System.Type _persistentClass;
        private System.Type[] _interfaces;
        private MethodInfo _getIdentifierMethod;
        private MethodInfo _setIdentifierMethod;
        private string _entityName;
        private IAbstractComponentType _componentIdType;


        public void PostInstantiate(string entityName, System.Type persistentClass, ISet<System.Type> interfaces, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType)
        {
            _entityName = entityName;
            _persistentClass = persistentClass;
            _interfaces = new System.Type[interfaces.Count];
            interfaces.CopyTo(_interfaces, 0);
            _getIdentifierMethod = getIdentifierMethod;
            _setIdentifierMethod = setIdentifierMethod;
            _componentIdType = componentIdType;
        }

        public INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            IPostSharpNHibernateProxy instance = (IPostSharpNHibernateProxy)Activator.CreateInstance(_persistentClass);
            PostSharpInitializer initializer = new PostSharpInitializer(_entityName, _persistentClass, id, _getIdentifierMethod,
                                                                        _getIdentifierMethod, session);
            instance.SetInterceptor(initializer);
            return instance;
        }
    }
}