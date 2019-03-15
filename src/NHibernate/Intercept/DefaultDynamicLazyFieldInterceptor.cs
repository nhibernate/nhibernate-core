using System;
using System.Reflection;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Util;

namespace NHibernate.Intercept
{
	[Serializable]
	// Since v5.2
	[Obsolete("Dynamic proxy has been obsoleted, use static proxies instead (see StaticProxyFactory)")]
	public class DefaultDynamicLazyFieldInterceptor : IFieldInterceptorAccessor, Proxy.DynamicProxy.IInterceptor
	{
		public IFieldInterceptor FieldInterceptor { get; set; }

		public object Intercept(InvocationInfo info)
		{
			if (ReflectHelper.IsPropertyGet(info.TargetMethod))
			{
				if (IsGetFieldInterceptorCall(info.TargetMethod))
				{
					return FieldInterceptor;
				}

				if (FieldInterceptor != null)
				{
					object propValue = info.InvokeMethodOnTarget();

					var result = FieldInterceptor.Intercept(info.Target, ReflectHelper.GetPropertyName(info.TargetMethod), propValue);

					if (result != AbstractFieldInterceptor.InvokeImplementation)
					{
						return result;
					}
				}
			}
			else if (ReflectHelper.IsPropertySet(info.TargetMethod))
			{
				if (IsSetFieldInterceptorCall(info.TargetMethod))
				{
					FieldInterceptor = (IFieldInterceptor) info.Arguments[0];
					return null;
				}

				if (FieldInterceptor != null)
				{
					FieldInterceptor.MarkDirty();
					FieldInterceptor.Intercept(info.Target, ReflectHelper.GetPropertyName(info.TargetMethod), info.Arguments[0], true);
				}
			}

			return info.InvokeMethodOnTarget();
		}

		private static bool IsGetFieldInterceptorCall(MethodInfo targetMethod)
		{
			return string.Equals("get_FieldInterceptor", targetMethod.Name, StringComparison.Ordinal) && targetMethod.DeclaringType == typeof(IFieldInterceptorAccessor);
		}

		private static bool IsSetFieldInterceptorCall(MethodInfo targetMethod)
		{
			return string.Equals("set_FieldInterceptor", targetMethod.Name, StringComparison.Ordinal) && targetMethod.DeclaringType == typeof(IFieldInterceptorAccessor);
		}
	}
}
