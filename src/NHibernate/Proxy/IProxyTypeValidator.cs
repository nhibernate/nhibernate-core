using System.Collections.Generic;
using NHibernate.Mapping;

namespace NHibernate.Proxy
{
    public interface IProxyTypeValidator
    {
        ICollection<string> ValidateType(PersistentClass persistentClass);

        void LogPropertyAccessorsErrors(PersistentClass persistentClass);
    }
}