using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Intercept;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	[Serializable]
	public sealed class FieldInterceptorObjectReference : IObjectReference, ISerializable
	{
		private readonly NHibernateProxyFactoryInfo _proxyFactoryInfo;
		private readonly IFieldInterceptor _fieldInterceptor;

		[NonSerialized]
		private readonly object _deserializedProxy;

		private const string HasAdditionalDataName = "proxy$hasAdditionalData";

		public FieldInterceptorObjectReference(NHibernateProxyFactoryInfo proxyFactoryInfo, IFieldInterceptor fieldInterceptorField)
		{
			_proxyFactoryInfo = proxyFactoryInfo;
			_fieldInterceptor = fieldInterceptorField;
		}

		private FieldInterceptorObjectReference(SerializationInfo info, StreamingContext context)
		{
			_proxyFactoryInfo = info.GetValue<NHibernateProxyFactoryInfo>(nameof(_proxyFactoryInfo));
			_fieldInterceptor = info.GetValue<IFieldInterceptor>(nameof(_fieldInterceptor));

			var proxy = _proxyFactoryInfo.CreateProxyFactory().GetFieldInterceptionProxy(null);
			if (info.GetBoolean(HasAdditionalDataName))
			{
				var members = FormatterServices.GetSerializableMembers(_proxyFactoryInfo.PersistentClass, context);
				foreach (var member in members)
				{
					switch (member)
					{
						case FieldInfo field:
							field.SetValue(
								proxy,
								info.GetValue(GetAdditionalMemberName(field), field.FieldType));
							break;
						case PropertyInfo property:
							property.SetValue(
								proxy,
								info.GetValue(GetAdditionalMemberName(property), property.PropertyType));
							break;
						default:
							throw new NotSupportedException(
								$"Deserializing a member of type {member.GetType()} is not supported.");
					}
				}
				_deserializedProxy = proxy;
			}
			else
			{
				// Base type has a custom serialization, we need to call the proxy deserialization for deserializing
				// base type members too.
				var proxyType = proxy.GetType();
				var deserializationConstructor = proxyType.GetConstructor(
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
					null,
					new[] { typeof(SerializationInfo), typeof(StreamingContext) },
					null);
				_deserializedProxy = deserializationConstructor.Invoke(new object[] { info, context });
			}
			((IFieldInterceptorAccessor) _deserializedProxy).FieldInterceptor = _fieldInterceptor;
		}

		[SecurityCritical]
		public object GetRealObject(StreamingContext context)
		{
			return _deserializedProxy;
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(nameof(_proxyFactoryInfo), _proxyFactoryInfo);
			info.AddValue(nameof(_fieldInterceptor), _fieldInterceptor);
		}

		[SecurityCritical]
		public void GetBaseData(SerializationInfo info, StreamingContext context, object proxy, System.Type proxyBaseType)
		{
			if (proxyBaseType == null)
				throw new ArgumentNullException(nameof(proxyBaseType));

			info.AddValue(HasAdditionalDataName, true);

			var members = FormatterServices.GetSerializableMembers(proxyBaseType, context);
			foreach (var member in members)
			{
				switch (member)
				{
					case FieldInfo field:
						info.AddValue(GetAdditionalMemberName(field), field.GetValue(proxy));
						break;
					case PropertyInfo property:
						info.AddValue(GetAdditionalMemberName(property), property.GetValue(proxy));
						break;
					default:
						throw new NotSupportedException($"Serializing a member of type {member.GetType()} is not supported.");
				}
			}
		}

		[SecurityCritical]
		public void SetNoAdditionalData(SerializationInfo info)
		{
			info.AddValue(HasAdditionalDataName, false);
		}

		private static string GetAdditionalMemberName(FieldInfo fieldInfo)
		{
			return $"proxy${fieldInfo.DeclaringType.Name}${fieldInfo.Name}";
		}

		private static string GetAdditionalMemberName(PropertyInfo propertyInfo)
		{
			return $"proxy${propertyInfo.DeclaringType.Name}${propertyInfo.Name}";
		}
	}
}
