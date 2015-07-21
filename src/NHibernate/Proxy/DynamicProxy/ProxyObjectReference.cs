#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Proxy.DynamicProxy
{
	[Serializable]
	public class ProxyObjectReference : IObjectReference, ISerializable
	{
		private readonly System.Type _baseType;
		private readonly IProxy _proxy;

		protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			// Deserialize the base type using its assembly qualified name
			string qualifiedName = info.GetString("__baseType");
			_baseType = System.Type.GetType(qualifiedName, true, false);

			// Rebuild the list of interfaces
			var interfaceList = new List<System.Type>();
			int interfaceCount = info.GetInt32("__baseInterfaceCount");
			for (int i = 0; i < interfaceCount; i++)
			{
				string keyName = string.Format("__baseInterface{0}", i);
				string currentQualifiedName = info.GetString(keyName);
				System.Type interfaceType = System.Type.GetType(currentQualifiedName, true, false);

				interfaceList.Add(interfaceType);
			}

			// Reconstruct the proxy
			var factory = new ProxyFactory();
			System.Type proxyType = factory.CreateProxyType(_baseType, interfaceList.ToArray());

			// Initialize the proxy with the deserialized data
			var args = new object[] {info, context};
			_proxy = (IProxy) Activator.CreateInstance(proxyType, args);
		}

		#region IObjectReference Members

#if NET_4_0
		[SecurityCritical]
#endif
		public object GetRealObject(StreamingContext context)
		{
			return _proxy;
		}

		#endregion

		#region ISerializable Members

#if NET_4_0
		[SecurityCritical]
#endif
		public void GetObjectData(SerializationInfo info, StreamingContext context) {}

		#endregion
	}
}