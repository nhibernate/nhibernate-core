using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Util
{
	[Serializable]
	internal sealed class SerializablePropertyInfo : ISerializable, IEquatable<SerializablePropertyInfo>
	{
		[NonSerialized]
		private readonly PropertyInfo _propertyInfo;

		/// <summary>
		/// Creates a new instance of <see cref="SerializablePropertyInfo"/> if 
		/// <paramref name="propertyInfo"/> is not null, otherwise returns <c>null</c>.
		/// </summary>
		/// <param name="propertyInfo">The <see cref="PropertyInfo"/> being wrapped for serialization.</param>
		/// <returns>New instance of <see cref="SerializablePropertyInfo"/> or <c>null</c>.</returns>
		public static SerializablePropertyInfo Wrap(PropertyInfo propertyInfo)
		{
			return propertyInfo == null ? null : new SerializablePropertyInfo(propertyInfo);
		}

		/// <summary>
		/// Creates a new <see cref="SerializablePropertyInfo"/>
		/// </summary>
		/// <param name="propertyInfo">The <see cref="PropertyInfo"/> being wrapped for serialization.</param>
		private SerializablePropertyInfo(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
			if (propertyInfo.DeclaringType == null) throw new ArgumentException("PropertyInfo must have non-null DeclaringType", nameof(propertyInfo));
			if (propertyInfo.GetIndexParameters().Length > 0) throw new ArgumentException("PropertyInfo not supported with IndexParameters", nameof(propertyInfo));
		}

		private SerializablePropertyInfo(SerializationInfo info, StreamingContext context)
		{
			System.Type declaringType = info.GetValue<SerializableSystemType>("declaringType").GetSystemType();
			string propertyName = info.GetString("propertyName");

			_propertyInfo = declaringType.GetProperty(
				propertyName,
				BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (_propertyInfo == null) throw new MissingMethodException(declaringType.FullName, propertyName);
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("declaringType", SerializableSystemType.Wrap(_propertyInfo.DeclaringType));
			info.AddValue("propertyName", _propertyInfo.Name);
		}

		public PropertyInfo Value => _propertyInfo;

		public bool Equals(SerializablePropertyInfo other)
		{
			return other != null && Equals(_propertyInfo, other._propertyInfo);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is SerializablePropertyInfo && Equals((SerializablePropertyInfo) obj);
		}

		public override int GetHashCode()
		{
			return (_propertyInfo != null ? _propertyInfo.GetHashCode() : 0);
		}
	}
}
