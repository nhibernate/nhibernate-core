using System;
using NHibernate.Engine;

namespace NHibernate.Proxy {
	
	public sealed class HibernateProxyHelper {
		
		public static object Unproxy(object obj, ISessionImplementor session) {
			HibernateProxy proxy = obj as HibernateProxy;
			if (proxy != null) {
				proxy.Session = session;
				return proxy.Implementation;
			} else {
				return obj;
			}
		}

	}
}
