using System;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Proxy.Poco;
using NHibernate.Type;

namespace NHibernate.Proxy
{
	[Serializable]
	public class DefaultLazyInitializer : BasicLazyInitializer, DynamicProxy.IInterceptor
	{
		[NonSerialized]
		private static readonly MethodInfo exceptionInternalPreserveStackTrace =
			typeof (Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

		public DefaultLazyInitializer(string entityName, System.Type persistentClass, object id, MethodInfo getIdentifierMethod,
							   MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType,
							   ISessionImplementor session)
			: base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session) {}

		#region Implementation of IInterceptor

		public object Intercept(InvocationInfo info)
		{
			object returnValue;
			try
			{
				returnValue = base.Invoke(info.TargetMethod, info.Arguments, info.Target);

				// Avoid invoking the actual implementation, if possible
				if (returnValue != InvokeImplementation)
				{
					return returnValue;
				}

				returnValue = info.TargetMethod.Invoke(GetImplementation(), info.Arguments);
			}
			catch (TargetInvocationException ex)
			{
				exceptionInternalPreserveStackTrace.Invoke(ex.InnerException, new Object[] {});
				throw ex.InnerException;
			}

			return returnValue;
		}

		#endregion
	}
}