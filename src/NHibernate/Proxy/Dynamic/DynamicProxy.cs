using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace NHibernate.Proxy.Dynamic
{
    [Serializable]
    public class DynamicProxy : INHibernateProxy, IDynamicMetaObjectProvider
    {
        private readonly DynamicLazyInitializer li;
        private DynamicMetaObject dyn;
		internal DynamicProxy(DynamicLazyInitializer li)
		{
			this.li = li;
		}

        public ILazyInitializer HibernateLazyInitializer
        {
            get { return li; }
        }

        public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return  li.Dynamic.GetMetaObject(parameter);
        }
    }
}
