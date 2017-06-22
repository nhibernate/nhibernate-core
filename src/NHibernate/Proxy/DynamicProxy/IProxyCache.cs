#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Reflection;

namespace NHibernate.Proxy.DynamicProxy
{
	public interface IProxyCache
	{
		bool Contains(System.Type baseType, params System.Type[] baseInterfaces);
		TypeInfo GetProxyType(System.Type baseType, params System.Type[] baseInterfaces);

		bool TryGetProxyType(System.Type baseType, System.Type[] baseInterfaces, out TypeInfo proxyType);

		void StoreProxyType(TypeInfo result, System.Type baseType, params System.Type[] baseInterfaces);
	}
}