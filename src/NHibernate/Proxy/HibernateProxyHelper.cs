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

		public static HibernateProxy GetHibernateProxy(object obj) {
			//TODO: reflect into the object, and pull out the real proxy
			return null;
		}


		public static bool IsProxy(object obj) {
			//TODO: determine if the object is a proxy object
			// can we just see if it's an instance of HibernateProxy?
			return false;
		}

	}
}
