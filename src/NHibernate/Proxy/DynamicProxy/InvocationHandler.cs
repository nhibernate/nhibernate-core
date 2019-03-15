#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;

namespace NHibernate.Proxy.DynamicProxy
{
	// Since v5.2
	[Obsolete("DynamicProxy namespace has been obsoleted, use static proxies instead (see StaticProxyFactory)")]
	public delegate object InvocationHandler(InvocationInfo info);
}
