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

		private static MethodInfo MapInterfaceMethodToImplementationIfNecessary(MethodInfo methodInfo,
		                                                                        System.Type implementingType)
		{
			MethodInfo concreteMethodInfo = methodInfo;

			if (methodInfo!= null && methodInfo.DeclaringType.IsInterface)
			{
				InterfaceMapping interfaceMapping = implementingType.GetInterfaceMap(methodInfo.DeclaringType);
				int methodIndex = Array.IndexOf(interfaceMapping.InterfaceMethods, methodInfo);
				concreteMethodInfo = interfaceMapping.TargetMethods[methodIndex];
			}

			return concreteMethodInfo;
		}

		public LazyInitializer(string entityName, System.Type persistentClass, object id, MethodInfo getIdentifierMethod,
		                       MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType,
		                       ISessionImplementor session)
			: base(
				entityName, persistentClass, id, MapInterfaceMethodToImplementationIfNecessary(getIdentifierMethod, persistentClass),
				MapInterfaceMethodToImplementationIfNecessary(setIdentifierMethod, persistentClass), componentIdType, session)
		{
			InterceptCalls = true;
		}

		public bool InterceptCalls { get; set; }

		#region Implementation of IInterceptor

		public object Invoke(IMethodInvocation info)
		{
			object returnValue;
			try
			{
				var methodInfo = info.Method;
				returnValue = base.Invoke(methodInfo, info.Arguments, info.Proxy);

				if (returnValue != InvokeImplementation)
				{
					return returnValue;
				}
				if (InterceptCalls)
				{
					var method = new SafeMethod(methodInfo);
					return method.Invoke(GetImplementation(), info.Arguments);
				}
			}
			catch (TargetInvocationException ex)
			{
				exceptionInternalPreserveStackTrace.Invoke(ex.InnerException, new Object[] { });
				throw ex.InnerException;
			}

			return returnValue;
		}

		#endregion

		#region Implementation of ITargetSource

		object ITargetSource.GetTarget()
		{
			return Target ?? this;
		}

		void ITargetSource.ReleaseTarget(object target)
		{
			//throw new System.NotImplementedException();
		}

		System.Type ITargetSource.TargetType
		{
			get { return base.PersistentClass; }
		}

		bool ITargetSource.IsStatic
		{
			get { return false; }
		}

		#endregion
	}
}