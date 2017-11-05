using System;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Util
{
	[Serializable]
	internal class SerializableSystemType : ISerializable, IEquatable<SerializableSystemType>
	{
		[NonSerialized]
		private readonly System.Type _type;

		private AssemblyQualifiedTypeName _typeName;

		protected AssemblyQualifiedTypeName TypeName => _typeName;

		/// <summary>
		/// Creates a new instance of <see cref="SerializableSystemType"/> if
		/// <paramref name="type"/> is not null, otherwise returns <c>null</c>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> being wrapped for serialization.</param>
		/// <returns>New instance of <see cref="SerializableSystemType"/> or <c>null</c>.</returns>
		public static SerializableSystemType Wrap(System.Type type)
		{
			return type == null ? null : new SerializableSystemType(type);
		}

		/// <summary>
		/// Creates a new <see cref="SerializableSystemType"/>
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> being wrapped for serialization.</param>
		protected SerializableSystemType(System.Type type)
		{
			_type = type ?? throw new ArgumentNullException(nameof(type));
		}

		protected SerializableSystemType(SerializationInfo info, StreamingContext context)
		{
			_typeName = info.GetValue<AssemblyQualifiedTypeName>("_typeName");
			if (_typeName == null)
				throw new InvalidOperationException("_typeName was null after deserialization");
			_type = _typeName.TypeFromAssembly(false);
		}

		/// <summary>
		/// Returns the wrapped type. Will throw if it was unable to load it after deserialization.
		/// </summary>
		/// <returns>The type that this class was initialized with or initialized after deserialization.</returns>
		public System.Type GetSystemType() => _type ?? throw new TypeLoadException("Could not load type " + _typeName + ".");

		/// <summary>
		/// Returns the wrapped type. Will return null if it was unable to load it after deserialization.
		/// </summary>
		/// <returns>The type that this class was initialized with, the type initialized after deserialization, or null if unable to load.</returns>
		public System.Type TryGetSystemType() => _type;

		public string FullName => _type?.FullName ?? _typeName.Type;

		public string AssemblyQualifiedName => _type?.AssemblyQualifiedName ?? _typeName.ToString();

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (_typeName == null)
			{
				_typeName = new AssemblyQualifiedTypeName(_type.FullName, _type.Assembly.FullName);
			}

			info.AddValue("_typeName", _typeName);
		}

		public bool Equals(SerializableSystemType other)
		{
			return other != null &&
				(_type == null || other._type == null
					? Equals(_typeName, other._typeName)
					: Equals(_type, other._type));
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is SerializableSystemType type && Equals(type);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (FullName.GetHashCode() * 397) ^ (AssemblyQualifiedName?.GetHashCode() ?? 0);
			}
		}
	}

	[Serializable]
	internal sealed class ObjectReferenceSystemType : SerializableSystemType, IObjectReference
	{
		private readonly bool _throwOnDeserializationError;

		/// <summary>
		/// Creates a new instance of <see cref="SerializableSystemType"/> if
		/// <paramref name="type"/> is not null, otherwise returns <c>null</c>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> being wrapped for serialization.</param>
		/// <param name="throwOnDeserializationError"><see langword="true" /> for failing if unable to load the type
		/// in <see cref="IObjectReference.GetRealObject" />, <see langword="false" /> to yield
		/// <see langword="null" /> instead.</param>
		/// <returns>New instance of <see cref="SerializableSystemType"/> or <c>null</c>.</returns>
		public static ObjectReferenceSystemType Wrap(System.Type type, bool throwOnDeserializationError)
		{
			return type == null ? null : new ObjectReferenceSystemType(type, throwOnDeserializationError);
		}

		/// <summary>
		/// Creates a new <see cref="SerializableSystemType"/>
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> being wrapped for serialization.</param>
		/// <param name="throwOnDeserializationError"><see langword="true" /> for failing if unable to load the type
		/// in <see cref="IObjectReference.GetRealObject" />, <see langword="false" /> to yield 
		/// <see langword="null" /> instead.</param>
		private ObjectReferenceSystemType(System.Type type, bool throwOnDeserializationError) : base(type)
		{
			_throwOnDeserializationError = throwOnDeserializationError;
		}

		private ObjectReferenceSystemType(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_throwOnDeserializationError = info.GetBoolean("_throwOnDeserializationError");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("_throwOnDeserializationError", _throwOnDeserializationError);
		}

		[SecurityCritical]
		object IObjectReference.GetRealObject(StreamingContext context)
		{
			return TypeName.TypeFromAssembly(_throwOnDeserializationError);
		}
	}
}
