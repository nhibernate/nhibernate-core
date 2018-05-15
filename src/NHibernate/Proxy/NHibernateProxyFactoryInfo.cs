using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Type;

namespace NHibernate.Proxy
{
	[Serializable]
	public sealed class NHibernateProxyFactoryInfo : ISerializable
	{
		private readonly string _entityName;
		private readonly System.Type _persistentClass;
		private readonly System.Type[] _interfaces;
		private readonly MethodInfo _getIdentifierMethod;
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
			_persistentClass = (System.Type) info.GetValue(nameof(_persistentClass), typeof(System.Type));
			_interfaces = (System.Type[]) info.GetValue(nameof(_interfaces), typeof(System.Type[]));
			_getIdentifierMethod = (MethodInfo) info.GetValue(nameof(_getIdentifierMethod), typeof(MethodInfo));
			_setIdentifierMethod = (MethodInfo) info.GetValue(nameof(_setIdentifierMethod), typeof(MethodInfo));
			_componentIdType = (IAbstractComponentType) info.GetValue(nameof(_componentIdType), typeof(IAbstractComponentType));
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(_entityName), _entityName);
			info.AddValue(nameof(_persistentClass), _persistentClass);
			info.AddValue(nameof(_interfaces), _interfaces);
			info.AddValue(nameof(_getIdentifierMethod), _getIdentifierMethod);
			info.AddValue(nameof(_setIdentifierMethod), _setIdentifierMethod);
			info.AddValue(nameof(_componentIdType), _componentIdType);
		}
	}
}
