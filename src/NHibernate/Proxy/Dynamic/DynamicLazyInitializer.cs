using NHibernate.Engine;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace NHibernate.Proxy.Dynamic
{
    [Serializable]
    public class DynamicLazyInitializer : AbstractLazyInitializer
    {
        public DynamicLazyInitializer(string entityName, object id, ISessionImplementor session)
            : base(entityName, id, session) { }

        public IDynamicMetaObjectProvider Dynamic
        {
            get { return (IDynamicMetaObjectProvider)GetImplementation(); }
        }

        public override System.Type PersistentClass
        {
            get
            {
                throw new NotSupportedException("dynamic entity representation");
            }
        }
    }
}
