using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace NHibernate.Proxy
{
	/// <summary>
	/// Serializable "container" for a NHibernate Proxy.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A NHibernate Proxy is not actually Serialized from Server to Client because
	/// it is a dynamically generated Type that does not exist in the Client AppDomain.
	/// </para>
	/// <para>
	/// This Class holds all of the information needed to rebuild the NHibernate Proxy
	/// during Deserialization.
	/// </para>
	/// </remarks>
	[Serializable]
	public class SerializableProxy : IObjectReference
	{
		private System.Type _persistentClass;
		private System.Type[] _interfaces;
		private object _id;
		private PropertyInfo _identifierPropertyInfo;

		/// <summary>
		/// Construct a container that can recreate the Proxy after Serialization.
		/// </summary>
		/// <param name="persistentClass">The PersistentClass to rebuild the Proxy for.</param>
		/// <param name="interfaces">The Interfaces for the Proxy.</param>
		/// <param name="id">The value of the id for the Proxy.</param>
		/// <param name="identifierPropertyInfo">The Identifier Property.</param>
		public SerializableProxy(System.Type persistentClass, System.Type[] interfaces, object id, PropertyInfo identifierPropertyInfo)
		{
			_persistentClass = persistentClass;
			_interfaces = interfaces;
			_id = id;
			_identifierPropertyInfo = identifierPropertyInfo;
			// don't need to do anything fancy here - in java a java.lang.reflect.Method is not serializable
			// so it had to be converted into a Class and Method Name to rebuild the Method object, but in
			// .NET the PropertyInfo is marked as serializable...
		}

		
		#region IObjectReference Members

		/// <summary>
		/// Converts a Serializable Proxy into the dynamically generated Proxy on the Client side.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		/// <remarks>
		/// This is called after the Deserialization ctor of this Class has been called.
		/// </remarks>
		public object GetRealObject(StreamingContext context)
		{
			IProxyGenerator generator = ProxyGeneratorFactory.GetProxyGenerator();

			object proxy = generator.GetProxy(_persistentClass, _interfaces, _identifierPropertyInfo, _id, null);

			return proxy;
		}

		#endregion

		
	}
}
