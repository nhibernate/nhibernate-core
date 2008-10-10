using NHibernate.Bytecode;
using NHibernate.Proxy;

namespace NHibernate.PostSharp.Proxies
{
    public class PostSharpProxyFactoryFactory : IProxyFactoryFactory
    {
        public IProxyFactory BuildProxyFactory()
        {
            return new PostSharpProxyFactory();
        }

        public IProxyTypeValidator Validator
        {
            get { return new PostSharpProxyValidator(); }
        }
    }
}