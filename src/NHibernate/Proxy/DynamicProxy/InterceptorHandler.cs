#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Diagnostics;
using System.Reflection;

namespace NHibernate.Proxy.DynamicProxy
{
	public delegate object InterceptorHandler(object proxy, MethodInfo targetMethod,
																						StackTrace trace, System.Type[] genericTypeArgs, object[] args);
}