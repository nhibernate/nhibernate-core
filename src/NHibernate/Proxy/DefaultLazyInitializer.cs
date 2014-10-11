using System;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Proxy.Poco;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	[Serializable]
	public class DefaultLazyInitializer : BasicLazyInitializer, DynamicProxy.IInterceptor
	{
		public DefaultLazyInitializer(string entityName, System.Type persistentClass, object id, MethodInfo getIdentifierMethod,
							   MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType,
							   ISessionImplementor session, bool overridesEquals)
			: base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session, overridesEquals) {}

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
				throw ReflectHelper.UnwrapTargetInvocationException(ex);
			}

			return returnValue;
		}
	}
}