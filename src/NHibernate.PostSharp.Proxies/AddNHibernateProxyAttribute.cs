using System;
using NHibernate.Proxy;
using PostSharp.Laos;

namespace NHibernate.PostSharp.Proxies
{
    [Serializable]
    public class AddNHibernateProxyAttribute : CompositionAspect
    {
        public override object CreateImplementationObject(InstanceBoundLaosEventArgs eventArgs)
        {
            return new PostSharpProxyAdapter();
        }

        public override System.Type GetPublicInterface(System.Type containerType)
        {
            return typeof (IPostSharpNHibernateProxy);
        }
    }
}