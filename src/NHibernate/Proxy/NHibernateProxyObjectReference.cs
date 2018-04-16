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

		public NHibernateProxyObjectReference(NHibernateProxyFactoryInfo proxyFactoryInfo, object identifier)
		{
			_proxyFactoryInfo = proxyFactoryInfo;
			_identifier = identifier;
		}

		private NHibernateProxyObjectReference(SerializationInfo info, StreamingContext context)
		{
			_proxyFactoryInfo = (NHibernateProxyFactoryInfo) info.GetValue(nameof(_proxyFactoryInfo), typeof(NHibernateProxyFactoryInfo));
			_identifier = info.GetValue(nameof(_identifier), typeof(object));
		}

		[SecurityCritical]
		public object GetRealObject(StreamingContext context)
		{
			return _proxyFactoryInfo.CreateProxyFactory().GetProxy(_identifier, null);
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(_proxyFactoryInfo), _proxyFactoryInfo);
			info.AddValue(nameof(_identifier), _identifier);
		}
	}
}
