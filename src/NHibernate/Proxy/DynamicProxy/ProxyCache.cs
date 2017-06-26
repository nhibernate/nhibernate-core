#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System.Collections.Concurrent;
using System.Reflection;

namespace NHibernate.Proxy.DynamicProxy
{
	public class ProxyCache : IProxyCache
	{
		private static readonly ConcurrentDictionary<ProxyCacheEntry, TypeInfo> cache = new ConcurrentDictionary<ProxyCacheEntry, TypeInfo>();

		#region IProxyCache Members

		public bool Contains(System.Type baseType, params System.Type[] baseInterfaces)
		{
			if (baseType == null)
			{
				return false;
			}

			var entry = new ProxyCacheEntry(baseType, baseInterfaces);
			return cache.ContainsKey(entry);
		}

		public TypeInfo GetProxyType(System.Type baseType, params System.Type[] baseInterfaces)
		{
			var entry = new ProxyCacheEntry(baseType, baseInterfaces);
			return cache[entry];
		}

		public bool TryGetProxyType(System.Type baseType, System.Type[] baseInterfaces, out TypeInfo proxyType)
		{
			proxyType = null;

			if (baseType == null)
				return false;

			var entry = new ProxyCacheEntry(baseType, baseInterfaces);
			return cache.TryGetValue(entry, out proxyType);
		}

		public void StoreProxyType(TypeInfo result, System.Type baseType, params System.Type[] baseInterfaces)
		{
			var entry = new ProxyCacheEntry(baseType, baseInterfaces);
			cache[entry] = result;
		}

		#endregion
	}
}
