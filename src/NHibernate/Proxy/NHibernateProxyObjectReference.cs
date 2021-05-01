using System;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Proxy
{
	[Serializable]
	public sealed class NHibernateProxyObjectReference : IObjectReference, ISerializable
	{
		private readonly NHibernateProxyFactoryInfo _proxyFactoryInfo;
		private readonly object _identifier;
		private readonly object _implementation;

		[Obsolete("Use overload taking an implementation parameter")]
		public NHibernateProxyObjectReference(NHibernateProxyFactoryInfo proxyFactoryInfo, object identifier)
			: this (proxyFactoryInfo, identifier, null)
		{}

		public NHibernateProxyObjectReference(NHibernateProxyFactoryInfo proxyFactoryInfo, object identifier, object implementation)
		{
			_proxyFactoryInfo = proxyFactoryInfo;
			_identifier = identifier;
			_implementation = implementation;
		}

		private NHibernateProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			_proxyFactoryInfo = (NHibernateProxyFactoryInfo) info.GetValue(nameof(_proxyFactoryInfo), typeof(NHibernateProxyFactoryInfo));
			_identifier = info.GetValue(nameof(_identifier), typeof(object));
			// 6.0 TODO: simplify with info.GetValue(nameof(_implementation), typeof(object));
			foreach (var entry in info)
			{
				if (entry.Name == nameof(_implementation))
				{
					_implementation = entry.Value;
				}
			}
		}

		[SecurityCritical]
		public object GetRealObject(StreamingContext context)
		{
			var proxy = _proxyFactoryInfo.CreateProxyFactory().GetProxy(_identifier, null);

			if (_implementation != null)
				proxy.HibernateLazyInitializer.SetImplementation(_implementation);

			return proxy;
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//Save a copy as it seems IObjectReference deserialization can't properly handle multiple objects with the same reference 
			info.AddValue(nameof(_proxyFactoryInfo), _proxyFactoryInfo.Clone());
			info.AddValue(nameof(_identifier), _identifier);
			info.AddValue(nameof(_implementation), _implementation);
		}
	}
}
