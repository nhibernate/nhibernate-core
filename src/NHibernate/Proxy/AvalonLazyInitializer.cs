using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

using Apache.Avalon.DynamicProxy;

using NHibernate.Engine;

namespace NHibernate.Proxy
{
	/// <summary>
	/// A <see cref="LazyInitiliazer"/> for use with Avalon's Dynamic Class Generator.
	/// </summary>
	public class AvalonLazyInitializer : LazyInitializer, IInvocationHandler
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger( typeof(AvalonLazyInitializer) );

		private System.Type _concreteProxy;
		private System.Type[] _interfaces;

		/// <summary>
		/// Initializes a new <see cref="AvalonLazyInitializer"/> object.
		/// </summary>
		/// <param name="persistentClass">The Class to Proxy.</param>
		/// <param name="concreteProxy">The <see cref="System.Type"/> to use as the Proxy.</param>
		/// <param name="interfaces">An array of <see cref="System.Type"/> interfaces that the Proxy should implement.</param>
		/// <param name="id">The Id of the Object we are Proxying.</param>
		/// <param name="identifierPropertyInfo">The PropertyInfo for the &lt;id&gt; property.</param>
		/// <param name="session">The ISession this Proxy is in.</param>
		internal AvalonLazyInitializer(System.Type persistentClass, System.Type concreteProxy, System.Type[] interfaces, object id, PropertyInfo identifierPropertyInfo, ISessionImplementor session)
			: base (persistentClass, id, identifierPropertyInfo,  session)
		{
			_concreteProxy = concreteProxy;
			_interfaces = interfaces;
		}


		protected override void AddSerializationInfo(SerializationInfo info)
		{
			// the AvalonProxyDeserializer will be the Type that is actually serialized for this
			// proxy.  
			info.SetType( typeof(AvalonProxyDeserializer) );
					
			info.AddValue( "_target", _target );
			info.AddValue( "_persistentClass", _persistentClass ); 
			info.AddValue( "_concreteProxy", _concreteProxy );
			info.AddValue( "_interfaces", _interfaces );
			info.AddValue( "_identifierPropertyInfo", _identifierPropertyInfo );
			info.AddValue( "_id", _id );
		}

		#region Apache.Avalon.DynamicProxy.IInvocationHandler Members

		/// <summary>
		/// Invoke the actual Property/Method using the Proxy or instantiate the actual
		/// object and use it when the Proxy can't handle the method. 
		/// </summary>
		/// <param name="methodName">The name of the Method/Property to Invoke.</param>
		/// <param name="args">The parameters for the Method/Property</param>
		/// <returns>The result just like the actual object was called.</returns>
		public object Invoke(object proxy, MethodInfo method, params object[] arguments)
		{
			object result = base.Invoke( method, arguments );

			// the base LazyInitializer could not handle it so we need to Invoke
			// the method/property against the real class.
			if(result==InvokeImplementation) 
			{
				return method.Invoke( GetImplementation(), arguments );
			}
			else 
			{
				return result;
			}
		}

		#endregion
	}
}
