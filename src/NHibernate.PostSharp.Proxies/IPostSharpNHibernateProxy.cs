using NHibernate.Engine;
using NHibernate.Proxy;

namespace NHibernate.PostSharp.Proxies
{
    public interface IPostSharpNHibernateProxy : INHibernateProxy
    {
        void SetInterceptor(PostSharpInitializer postSharpInitializer);
    }
}