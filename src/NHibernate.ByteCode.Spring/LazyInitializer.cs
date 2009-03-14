using System;
using System.Reflection;
using AopAlliance.Intercept;
using NHibernate.Engine;
using NHibernate.Proxy.Poco;
using NHibernate.Type;
using Spring.Aop;
using Spring.Reflection.Dynamic;

namespace NHibernate.ByteCode.Spring
{
	[Serializable]
	public class LazyInitializer : BasicLazyInitializer, IMethodInterceptor, ITargetSource
	{
		private static readonly MethodInfo exceptionInternalPreserveStackTrace =
			typeof (Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

		public LazyInitializer(string entityName, System.Type persistentClass, object id, MethodInfo getIdentifierMethod,
		                       MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType,
		                       ISessionImplementor session)
			: base(
				entityName, persistentClass.IsInterface ? typeof (object) : persistentClass, id, getIdentifierMethod,
				setIdentifierMethod, componentIdType, session) {}

		#region Implementation of IInterceptor

		public object Invoke(IMethodInvocation info)
		{
			try
			{
				MethodInfo methodInfo = info.Method;
				object returnValue = base.Invoke(methodInfo, info.Arguments, info.Proxy);

				if (returnValue != InvokeImplementation)
				{
					return returnValue;
				}

				var method = new SafeMethod(methodInfo);
				return method.Invoke(GetImplementation(), info.Arguments);
			}
			catch (TargetInvocationException ex)
			{
				exceptionInternalPreserveStackTrace.Invoke(ex.InnerException, new Object[] {});
				throw ex.InnerException;
			}
		}

		#endregion

		#region Implementation of ITargetSource

		object ITargetSource.GetTarget()
		{
			return Target;
		}

		void ITargetSource.ReleaseTarget(object target) {}

		System.Type ITargetSource.TargetType
		{
			get { return PersistentClass; }
		}

		bool ITargetSource.IsStatic
		{
			get { return false; }
		}

		#endregion
	}
}