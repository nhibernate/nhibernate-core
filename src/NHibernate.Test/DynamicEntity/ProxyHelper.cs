using Castle.DynamicProxy;

namespace NHibernate.Test.DynamicEntity
{
	public class ProxyHelper
	{
		private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

		public static Person NewPersonProxy()
		{
			return NewPersonProxy(0L);
		}

		public static Person NewPersonProxy(object id)
		{
			return
				(Person)
				proxyGenerator.CreateInterfaceProxyWithoutTarget(typeof (Person),
				                                                 new System.Type[] {typeof (IProxyMarker), typeof (Person)},
				                                                 new DataProxyHandler(typeof (Person).FullName, id));
		}

		public static Customer NewCustomerProxy()
		{
			return NewCustomerProxy(0L);
		}

		public static Customer NewCustomerProxy(object id)
		{
			return
				(Customer)
				proxyGenerator.CreateInterfaceProxyWithoutTarget(typeof (Customer),
				                                                 new System.Type[] {typeof (IProxyMarker), typeof (Customer)},
				                                                 new DataProxyHandler(typeof (Customer).FullName, id));
		}

		public static Company NewCompanyProxy()
		{
			return NewCompanyProxy(0L);
		}

		public static Company NewCompanyProxy(object id)
		{
			return
				(Company)
				proxyGenerator.CreateInterfaceProxyWithoutTarget(typeof (Company),
				                                                 new System.Type[] {typeof (IProxyMarker), typeof (Company)},
				                                                 new DataProxyHandler(typeof (Company).FullName, id));
		}

		public static Address NewAddressProxy()
		{
			return NewAddressProxy(0L);
		}

		public static Address NewAddressProxy(object id)
		{
			return
				(Address)
				proxyGenerator.CreateInterfaceProxyWithoutTarget(typeof (Address),
				                                                 new System.Type[] {typeof (IProxyMarker), typeof (Address)},
				                                                 new DataProxyHandler(typeof (Address).FullName, id));
		}

		public static string ExtractEntityName(object obj)
		{
			// Our custom Proxy instances actually bundle
			// their appropriate entity name, so we simply extract it from there
			// if this represents one of our proxies; otherwise, we return null
			IProxyMarker pm = obj as IProxyMarker;
			if (pm != null)
			{
				DataProxyHandler myHandler = pm.DataHandler;
				return myHandler.EntityName;
			}
			return null;
		}
	}
}