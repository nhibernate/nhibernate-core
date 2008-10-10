using NHibernate.Proxy;

namespace NHibernate.PostSharp.Proxies
{
    public class PostSharpProxyAdapter : IPostSharpNHibernateProxy
    {
        private static int creations;

        public PostSharpProxyAdapter()
        {
            creations += 1;
        }
        private ILazyInitializer interceptor;

        public ILazyInitializer HibernateLazyInitializer
        {
            get
            {
                return interceptor;
            }
        }

        public void SetInterceptor(PostSharpInitializer postSharpInitializer)
        {
            interceptor = postSharpInitializer;
        }
    }
}