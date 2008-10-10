using System;
using System.Reflection;
using NHibernate.Proxy;
using PostSharp.Laos;

namespace NHibernate.PostSharp.Proxies
{
    [Serializable]
    public class NHibernateLazyLoadingSupportAttribute : OnMethodInvocationAspect
    {
        public override void OnInvocation(MethodInvocationEventArgs eventArgs)
        {
            INHibernateProxy proxy = eventArgs.Delegate.Target as INHibernateProxy;
            if (proxy != null)
            {
                ILazyInitializer initializer = proxy.HibernateLazyInitializer;
                if (initializer != null)
                {
                    var interceptor = ((PostSharpInitializer) initializer);

                    interceptor.Intercept(eventArgs);
                    return;
                }
            }
            base.OnInvocation(eventArgs);
        }


    }
}
