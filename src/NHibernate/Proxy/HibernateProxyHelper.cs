using System;
using NHibernate.Engine;

namespace NHibernate.Proxy {
	
	public sealed class HibernateProxyHelper {

		private HibernateProxyHelper() {
			//can't instantiate
		}

		public static LazyInitializer GetLazyInitializer(HibernateProxy proxy) {
			return null;
			//TODO: return (LazyInitializer) Enhancer.getMethodInterceptor(proxy);
		}

		public static System.Type GetClass(object obj) {
			if (obj is HibernateProxy) {
				HibernateProxy proxy = (HibernateProxy) obj;
					LazyInitializer li = GetLazyInitializer(proxy);
				return li.PersistentClass;
			}
			else {
				return obj.GetType();
			}
		}
		/*
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
*/
	}
}
