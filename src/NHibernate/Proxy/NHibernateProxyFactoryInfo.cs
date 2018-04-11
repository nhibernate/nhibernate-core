using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	[Serializable]
	public sealed class NHibernateProxyFactoryInfo : ISerializable
	{
		private readonly string _entityName;

		[NonSerialized]
		private readonly System.Type _persistentClass;

		[NonSerialized]
		private readonly System.Type[] _interfaces;

		[NonSerialized]
		private readonly MethodInfo _getIdentifierMethod;

		[NonSerialized]
		private readonly MethodInfo _setIdentifierMethod;

		private readonly IAbstractComponentType _componentIdType;

		internal NHibernateProxyFactoryInfo(string entityName, System.Type persistentClass, System.Type[] interfaces, MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod, IAbstractComponentType componentIdType)
		{
			_entityName = entityName;
			_persistentClass = persistentClass;
			_interfaces = interfaces;
			_getIdentifierMethod = getIdentifierMethod;
			_setIdentifierMethod = setIdentifierMethod;
			_componentIdType = componentIdType;
		}

		public IProxyFactory CreateProxyFactory()
		{
			var factory = new StaticProxyFactory();
			factory.PostInstantiate(_entityName, _persistentClass, new HashSet<System.Type>(_interfaces), _getIdentifierMethod, _setIdentifierMethod, _componentIdType);
			return factory;
		}

		private NHibernateProxyFactoryInfo(SerializationInfo info, StreamingContext context)
		{
			_entityName = (string) info.GetValue(nameof(_entityName), typeof(string));
			_persistentClass = info.GetValue<SerializableSystemType>(nameof(_persistentClass))?.GetSystemType();
			_interfaces = info.GetValue<SerializableSystemType[]>(nameof(_interfaces))?.Select(x => x?.GetSystemType()).ToArray();
			_getIdentifierMethod = info.GetValue<SerializableMethodInfo>(nameof(_getIdentifierMethod))?.Value;
			_setIdentifierMethod = info.GetValue<SerializableMethodInfo>(nameof(_setIdentifierMethod))?.Value;
			_componentIdType = (IAbstractComponentType) info.GetValue(nameof(_componentIdType), typeof(IAbstractComponentType));
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(_entityName), _entityName);
			info.AddValue(nameof(_persistentClass), SerializableSystemType.Wrap(_persistentClass));
			info.AddValue(nameof(_interfaces), _interfaces?.Select(SerializableSystemType.Wrap).ToArray());
			info.AddValue(nameof(_getIdentifierMethod), SerializableMethodInfo.Wrap(_getIdentifierMethod));
			info.AddValue(nameof(_setIdentifierMethod), SerializableMethodInfo.Wrap(_setIdentifierMethod));
			info.AddValue(nameof(_componentIdType), _componentIdType);
		}
	}
}
