using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Test.DynamicEntity
{
	public class ProxyHelper
	{
		private static readonly ProxyFactory proxyGenerator = new ProxyFactory();

		private static T NewProxy<T>(object id)
		{
			return (T)proxyGenerator.CreateProxy(typeof(T), new DataProxyHandler(typeof (T).FullName, id),
			                                     new[] {typeof (IProxyMarker), typeof (T)});

		}

		public static Person NewPersonProxy()
		{
			return NewProxy<Person>(0L);
		}

		public static Person NewPersonProxy(object id)
		{
			return NewProxy<Person>(id);
		}

		public static Customer NewCustomerProxy()
		{
			return NewProxy<Customer>(0L);
		}

		public static Customer NewCustomerProxy(object id)
		{
			return NewProxy<Customer>(id);
		}

		public static Company NewCompanyProxy()
		{
			return NewProxy<Company>(0L);
		}

		public static Company NewCompanyProxy(object id)
		{
			return NewProxy<Company>(id);
		}

		public static Address NewAddressProxy()
		{
			return NewProxy<Address>(0L);
		}

		public static Address NewAddressProxy(object id)
		{
			return NewProxy<Address>(id);
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