using Castle.Core.Interceptor;
using NHibernate.Intercept;
using NHibernate.Util;

namespace NHibernate.ByteCode.Castle
{
	public class LazyFieldInterceptor : IFieldInterceptorAccessor, global::Castle.Core.Interceptor.IInterceptor
	{
		public IFieldInterceptor FieldInterceptor
		{
			get;
			set;
		}

		public void Intercept(IInvocation invocation)
		{
			if (FieldInterceptor != null)
			{
				if (ReflectHelper.IsPropertyGet(invocation.Method))
				{
					var result = FieldInterceptor.Intercept(invocation.InvocationTarget, ReflectHelper.GetPropertyName(invocation.Method));
					if (result == AbstractFieldInterceptor.InvokeImplementation)
					{
						invocation.Proceed();
					}
					else
					{
						invocation.ReturnValue = result;
					}
				}
				else if (ReflectHelper.IsPropertySet(invocation.Method))
				{
					FieldInterceptor.MarkDirty();
					FieldInterceptor.Intercept(invocation.InvocationTarget, ReflectHelper.GetPropertyName(invocation.Method));
					invocation.Proceed();
				}
			}
			else
			{
				invocation.Proceed();
			}
		}
	}
}