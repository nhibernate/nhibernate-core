#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	// Since v5.2
	[Obsolete("DynamicProxy namespace has been obsoleted, use static proxies instead (see StaticProxyFactory)")]
	internal class DefaultyProxyMethodBuilder : IProxyMethodBuilder
	{
		public DefaultyProxyMethodBuilder() : this(new DefaultMethodEmitter()) { }

		public DefaultyProxyMethodBuilder(IMethodBodyEmitter emitter)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			MethodBodyEmitter = emitter;
		}

		public IMethodBodyEmitter MethodBodyEmitter { get; private set; }

		public void CreateProxiedMethod(FieldInfo field, MethodInfo method, TypeBuilder typeBuilder)
		{
			var callbackMethod = ProxyBuilderHelper.GenerateMethodSignature(method.Name + "_callback", method, typeBuilder);
			var proxyMethod = ProxyBuilderHelper.GenerateMethodSignature(method.Name, method, typeBuilder);

			MethodBodyEmitter.EmitMethodBody(proxyMethod, callbackMethod, method, field);

			typeBuilder.DefineMethodOverride(proxyMethod, method);
		}
	}
}
