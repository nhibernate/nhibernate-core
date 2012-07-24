using System;
using System.Reflection;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Util;

namespace NHibernate.Intercept
{
	[Serializable]
	public class DefaultDynamicLazyFieldInterceptor : IFieldInterceptorAccessor, Proxy.DynamicProxy.IInterceptor
	{
		public DefaultDynamicLazyFieldInterceptor(object targetInstance)
		{
			if (targetInstance == null)
			{
				throw new ArgumentNullException("targetInstance");
			}
			TargetInstance = targetInstance;
		}

		public IFieldInterceptor FieldInterceptor { get; set; }
		public object TargetInstance { get; private set; }

		public object Intercept(InvocationInfo info)
		{
			var methodName = info.TargetMethod.Name;
			if (FieldInterceptor != null)
			{
				if (ReflectHelper.IsPropertyGet(info.TargetMethod))
				{
					if("get_FieldInterceptor".Equals(methodName))
					{
						return FieldInterceptor;
					}
					object propValue = info.TargetMethod.Invoke(TargetInstance, info.Arguments);

					var result = FieldInterceptor.Intercept(info.Target, ReflectHelper.GetPropertyName(info.TargetMethod), propValue);

					if (result != AbstractFieldInterceptor.InvokeImplementation)
					{
						return result;
					}
				}
				else if (ReflectHelper.IsPropertySet(info.TargetMethod))
				{
					if ("set_FieldInterceptor".Equals(methodName))
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
				if ("set_FieldInterceptor".Equals(methodName))
				{
					FieldInterceptor = (IFieldInterceptor)info.Arguments[0];
					return null;
				}
			}

			MethodInfo targetMethod = info.TargetMethod;
			if (info.TypeArguments != null && info.TypeArguments.Length != 0)
			{
				targetMethod = targetMethod.MakeGenericMethod(info.TypeArguments);
			}
			return targetMethod.Invoke(TargetInstance, info.Arguments);
		}
	}
}