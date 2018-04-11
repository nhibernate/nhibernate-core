using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Util
{
	[Serializable]
	internal sealed class SerializableConstructorInfo : ISerializable, IEquatable<SerializableConstructorInfo>
	{
		[NonSerialized]
		private readonly ConstructorInfo _constructorInfo;

		/// <summary>
		/// Creates a new instance of <see cref="SerializableConstructorInfo"/> if 
		/// <paramref name="constructorInfo"/> is not null, otherwise returns <c>null</c>.
		/// </summary>
		/// <param name="constructorInfo">The <see cref="ConstructorInfo"/> being wrapped for serialization.</param>
		/// <returns>New instance of <see cref="SerializableConstructorInfo"/> or <c>null</c>.</returns>
		public static SerializableConstructorInfo Wrap(ConstructorInfo constructorInfo)
		{
			return constructorInfo == null ? null : new SerializableConstructorInfo(constructorInfo);
		}

		/// <summary>
		/// Creates a new <see cref="SerializableConstructorInfo"/>
		/// </summary>
		/// <param name="constructorInfo">The <see cref="ConstructorInfo"/> being wrapped for serialization.</param>
		private SerializableConstructorInfo(ConstructorInfo constructorInfo)
		{
			_constructorInfo = constructorInfo ?? throw new ArgumentNullException(nameof(constructorInfo));
			if (constructorInfo.DeclaringType == null)
			{
				throw new ArgumentException("ConstructorInfo must have non-null DeclaringType", nameof(constructorInfo));
			}
		}

		private SerializableConstructorInfo(SerializationInfo info, StreamingContext context)
		{
			System.Type declaringType = info.GetValue<SerializableSystemType>("declaringType").GetSystemType();
			SerializableSystemType[] parameterSystemTypes = info.GetValue<SerializableSystemType[]>("parameterTypesHelper");

			System.Type[] parameterTypes = parameterSystemTypes?.Select(x => x.GetSystemType()).ToArray() ?? new System.Type[0];
			_constructorInfo = declaringType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, parameterTypes, null);

			if (_constructorInfo == null) throw new MissingMethodException(declaringType.FullName, ".ctor");
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			SerializableSystemType[] parameterSystemTypes =
				_constructorInfo.GetParameters()
				                .Select(x => SerializableSystemType.Wrap(x.ParameterType))
				                .ToArray();

			info.AddValue("declaringType", SerializableSystemType.Wrap(_constructorInfo.DeclaringType));
			info.AddValue("parameterTypesHelper", parameterSystemTypes);
		}

		public ConstructorInfo Value => _constructorInfo;

		public bool Equals(SerializableConstructorInfo other)
		{
			return other != null && Equals(_constructorInfo, other._constructorInfo);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is SerializableConstructorInfo && Equals((SerializableConstructorInfo) obj);
		}

		public override int GetHashCode()
		{
			return (_constructorInfo != null ? _constructorInfo.GetHashCode() : 0);
		}
	}
}
