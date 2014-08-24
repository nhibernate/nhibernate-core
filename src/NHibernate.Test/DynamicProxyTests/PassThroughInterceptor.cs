using System;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Test.DynamicProxyTests
{
	public class PassThroughInterceptor : NHibernate.Proxy.DynamicProxy.IInterceptor
	{
		private readonly object targetInstance;

		public PassThroughInterceptor(object targetInstance)
		{
			this.targetInstance = targetInstance;
		}

		public object Intercept(InvocationInfo info)
		{
			return info.TargetMethod.Invoke(targetInstance, info.Arguments);
		}
	}
}