using Castle.DynamicProxy;
using NHibernate.Intercept;
using NHibernate.Util;

namespace NHibernate.ByteCode.Castle
{
	public class LazyFieldInterceptor : IFieldInterceptorAccessor, global::Castle.DynamicProxy.IInterceptor
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
					invocation.Proceed(); // get the existing value
					
					var result = FieldInterceptor.Intercept(
						invocation.InvocationTarget, 
						ReflectHelper.GetPropertyName(invocation.Method), 
						invocation.ReturnValue);

					if (result != AbstractFieldInterceptor.InvokeImplementation)
					{
						invocation.ReturnValue = result;
					}
				}
				else if (ReflectHelper.IsPropertySet(invocation.Method))
				{
					FieldInterceptor.MarkDirty();
					FieldInterceptor.Intercept(invocation.InvocationTarget, ReflectHelper.GetPropertyName(invocation.Method), null);
					invocation.Proceed();
				}
				else
				{
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