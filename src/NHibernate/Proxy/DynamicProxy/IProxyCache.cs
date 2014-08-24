#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;

namespace NHibernate.Proxy.DynamicProxy
{
	public interface IProxyCache
	{
		bool Contains(System.Type baseType, params System.Type[] baseInterfaces);
		System.Type GetProxyType(System.Type baseType, params System.Type[] baseInterfaces);

		bool TryGetProxyType(System.Type baseType, System.Type[] baseInterfaces, out System.Type proxyType);

		void StoreProxyType(System.Type result, System.Type baseType, params System.Type[] baseInterfaces);
	}
}