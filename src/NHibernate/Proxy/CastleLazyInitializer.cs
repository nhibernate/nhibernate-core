using System;
using System.Reflection;
using Castle.DynamicProxy;
using log4net;
using NHibernate.Engine;

namespace NHibernate.Proxy
{
	/// <summary>
	/// A <see cref="LazyInitializer"/> for use with the Castle Dynamic Class Generator.
	/// </summary>
	[Serializable]
	[CLSCompliant( false )]
	public class CastleLazyInitializer : LazyInitializer, Castle.DynamicProxy.IInterceptor //, ISerializable
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( CastleLazyInitializer ) );

		private System.Type _concreteProxy; // not used !?!
		private System.Type[ ] _interfaces; // not used !?!

		/// <summary>
		/// Initializes a new <see cref="CastleLazyInitializer"/> object.
		/// </summary>
		/// <param name="persistentClass">The Class to Proxy.</param>
		/// <param name="concreteProxy">The <see cref="System.Type"/> to use as the Proxy.</param>
		/// <param name="interfaces">An array of <see cref="System.Type"/> interfaces that the Proxy should implement.</param>
		/// <param name="id">The Id of the Object we are Proxying.</param>
		/// <param name="identifierPropertyInfo">The PropertyInfo for the &lt;id&gt; property.</param>
		/// <param name="session">The ISession this Proxy is in.</param>
		internal CastleLazyInitializer( System.Type persistentClass, System.Type concreteProxy, System.Type[ ] interfaces, object id, PropertyInfo identifierPropertyInfo, ISessionImplementor session )
			: base( persistentClass, id, identifierPropertyInfo, session )
		{
			_concreteProxy = concreteProxy;
			_interfaces = interfaces;
		}

		#region Castle.DynamicProxy.IInterceptor Members

		/// <summary>
		/// Invoke the actual Property/Method using the Proxy or instantiate the actual
		/// object and use it when the Proxy can't handle the method. 
		/// </summary>
		/// <param name="invocation">The <see cref="IInvocation"/> from the generated Castle.DynamicProxy.</param>
		/// <param name="args">The parameters for the Method/Property</param>
		/// <returns>The result just like the actual object was called.</returns>
		public object Intercept( IInvocation invocation, params object[ ] args )
		{
			// let the generic LazyInitializer figure out if this can be handled
			// with the proxy or if the real class needs to be initialized
			object result = base.Invoke( invocation.Method, args );

			// the base LazyInitializer could not handle it so we need to Invoke
			// the method/property against the real class
			if( result == InvokeImplementation )
			{
				invocation.InvocationTarget = GetImplementation();
				return invocation.Proceed( args );
			}
			else
			{
				return result;
			}
		}

		#endregion
	}
}