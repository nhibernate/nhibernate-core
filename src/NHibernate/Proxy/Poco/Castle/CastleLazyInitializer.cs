using System;
using System.Reflection;
using Castle.Core.Interceptor;
using log4net;
using NHibernate.Engine;

namespace NHibernate.Proxy.Poco.Castle
{
	/// <summary>
	/// A <see cref="ILazyInitializer"/> for use with the Castle Dynamic Class Generator.
	/// </summary>
	[Serializable]
	[CLSCompliant(false)]
	public class CastleLazyInitializer : AbstractLazyInitializer, global::Castle.Core.Interceptor.IInterceptor
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(CastleLazyInitializer));

		#region Instance

		public bool _constructed = false;

		/// <summary>
		/// Initializes a new <see cref="CastleLazyInitializer"/> object.
		/// </summary>
		/// <param name="persistentClass">The Class to Proxy.</param>
		/// <param name="id">The Id of the Object we are Proxying.</param>
		/// <param name="getIdentifierMethod"></param>
		/// <param name="setIdentifierMethod"></param>
		/// <param name="session">The ISession this Proxy is in.</param>
		public CastleLazyInitializer(
			System.Type persistentClass,
			object id,
			MethodInfo getIdentifierMethod,
			MethodInfo setIdentifierMethod,
			ISessionImplementor session)
			: base(persistentClass, id, getIdentifierMethod, setIdentifierMethod, session)
		{
		}

		/// <summary>
		/// Invoke the actual Property/Method using the Proxy or instantiate the actual
		/// object and use it when the Proxy can't handle the method. 
		/// </summary>
		/// <param name="invocation">The <see cref="IInvocation"/> from the generated Castle.DynamicProxy.</param>
		public virtual void Intercept(IInvocation invocation)
		{
			try
			{
				if (_constructed)
				{
					// let the generic LazyInitializer figure out if this can be handled
					// with the proxy or if the real class needs to be initialized
					invocation.ReturnValue = base.Invoke(invocation.Method, invocation.Arguments, invocation.Proxy);

					// the base LazyInitializer could not handle it so we need to Invoke
					// the method/property against the real class
					if (invocation.ReturnValue == InvokeImplementation)
					{
						invocation.ReturnValue = invocation.Method.Invoke(GetImplementation(), invocation.Arguments);
						return;
					}
					else
					{
						return;
					}
				}
				else
				{
					// TODO: Find out equivalent to CGLIB's 'method.invokeSuper'.
					return;
				}
			}
			catch (TargetInvocationException tie)
			{
				// Propagate the inner exception so that the proxy throws the same exception as
				// the real object would (though of course the stack trace will be probably lost).
				throw tie.InnerException;
			}
		}

		#endregion
	}
}