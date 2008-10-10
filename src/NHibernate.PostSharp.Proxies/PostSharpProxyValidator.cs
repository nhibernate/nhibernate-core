using System.Collections.Generic;
using NHibernate.Mapping;
using NHibernate.Proxy;

namespace NHibernate.PostSharp.Proxies
{
    public class PostSharpProxyValidator : IProxyTypeValidator
    {
        public ICollection<string> ValidateType(PersistentClass persistentClass)
        {
            // TODO: check public fields       
            return null;
        }

        public void LogPropertyAccessorsErrors(PersistentClass persistentClass)
        {
            
        }
    }
}