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
			var targetMethod = info.TargetMethod;
			if (targetMethod.IsGenericMethodDefinition && info.TypeArguments != null && info.TypeArguments.Length > 0)
			{
				targetMethod = targetMethod.MakeGenericMethod(info.TypeArguments);
			}
			return targetMethod.Invoke(targetInstance, info.Arguments);
		}
	}
}