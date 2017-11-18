using System;
using System.Reflection;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Util;

namespace NHibernate.Intercept
{
	[Serializable]
	public class DefaultDynamicLazyFieldInterceptor : IFieldInterceptorAccessor, Proxy.DynamicProxy.IInterceptor
	{
		public IFieldInterceptor FieldInterceptor { get; set; }

		public object Intercept(InvocationInfo info)
		{
			var methodName = info.TargetMethod.Name;
			if (FieldInterceptor != null)
			{
				if (ReflectHelper.IsPropertyGet(info.TargetMethod))
				{
					if (IsGetFieldInterceptorCall(methodName, info.TargetMethod))
					{
						return FieldInterceptor;
					}

					object propValue = info.InvokeMethodOnTarget();

					var result = FieldInterceptor.Intercept(info.Target, ReflectHelper.GetPropertyName(info.TargetMethod), propValue);

					if (result != AbstractFieldInterceptor.InvokeImplementation)
					{
						return result;
					}
				}
				else if (ReflectHelper.IsPropertySet(info.TargetMethod))
				{
					if (IsSetFieldInterceptorCall(methodName, info.TargetMethod))
					{
						FieldInterceptor = (IFieldInterceptor)info.Arguments[0];
						return null;
					}
					FieldInterceptor.MarkDirty();
					FieldInterceptor.Intercept(info.Target, ReflectHelper.GetPropertyName(info.TargetMethod), info.Arguments[0]);
				}
			}
			else
			{
				if (IsSetFieldInterceptorCall(methodName, info.TargetMethod))
				{
					FieldInterceptor = (IFieldInterceptor)info.Arguments[0];
					return null;
				}
			}

			return info.InvokeMethodOnTarget();
		}

		private static bool IsGetFieldInterceptorCall(string methodName, MethodInfo targetMethod)
		{
			return "get_FieldInterceptor".Equals(methodName) && targetMethod.DeclaringType == typeof(IFieldInterceptorAccessor);
		}

		private static bool IsSetFieldInterceptorCall(string methodName, MethodInfo targetMethod)
		{
			return "set_FieldInterceptor".Equals(methodName) && targetMethod.DeclaringType == typeof(IFieldInterceptorAccessor);
		}
	}
}
