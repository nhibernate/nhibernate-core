using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace NHibernate.Proxy
{
	/// <summary>
	/// Deserializes an Avalon generated Proxy into either another Proxy that is regenerated 
	/// or into the target that the generated Proxy was proxying.
	/// </summary>
	[Serializable]
	public class AvalonProxyDeserializer : IObjectReference
	{
		// these fields are set during the GetObjectData call in AvalonLazyInitializer.
		private object _target = null;
		private System.Type _persistentClass = null;
		private System.Type _concreteProxy = null;
		private System.Type[] _interfaces = null;
		private PropertyInfo _identifierPropertyInfo = null;
		private object _id = null;
		
		#region IObjectReference Members

		/// <summary>
		/// Converts a SerializableProxy into the dynamically generated Proxy on the Client side
		/// or just returns the object that was the target behind the Proxy if it was loaded.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public object GetRealObject(StreamingContext context)
		{
			if( _target==null )
			{

				IProxyGenerator generator = ProxyGeneratorFactory.GetProxyGenerator();
				
				object proxy = generator.GetProxy( 
					_persistentClass
					, _concreteProxy
					, _interfaces
					, _identifierPropertyInfo
					, _id
					, null );
				
				return proxy;
			}
			else 
			{
				// this object was not serialized as a proxy but instead as the real object
				// there is nothing to do with it other than return it and let the framework
				// put it into the deserialized object stream.
				return _target;
			}
			
		}

		#endregion

	}
}
