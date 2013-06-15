#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	public interface IMethodBodyEmitter
	{
		void EmitMethodBody(MethodBuilder proxyMethod, MethodBuilder callbackMethod, MethodInfo method, FieldInfo field);
	}
}