using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Util
{
	[Serializable]
	internal sealed class SerializableFieldInfo : ISerializable, IEquatable<SerializableFieldInfo>
	{
		[NonSerialized]
		private readonly FieldInfo _fieldInfo;

		/// <summary>
		/// Creates a new instance of <see cref="SerializableFieldInfo"/> if 
		/// <paramref name="fieldInfo"/> is not null, otherwise returns <c>null</c>.
		/// </summary>
		/// <param name="fieldInfo">The <see cref="FieldInfo"/> being wrapped for serialization.</param>
		/// <returns>New instance of <see cref="SerializableFieldInfo"/> or <c>null</c>.</returns>
		public static SerializableFieldInfo Wrap(FieldInfo fieldInfo)
		{
			return fieldInfo == null ? null : new SerializableFieldInfo(fieldInfo);
		}

		/// <summary>
		/// Creates a new <see cref="SerializableFieldInfo"/>
		/// </summary>
		/// <param name="fieldInfo">The <see cref="FieldInfo"/> being wrapped for serialization.</param>
		private SerializableFieldInfo(FieldInfo fieldInfo)
		{
			_fieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
			if (fieldInfo.IsStatic) throw new ArgumentException("Only for instance fields", nameof(fieldInfo));
			if (fieldInfo.DeclaringType == null) throw new ArgumentException("FieldInfo must have non-null DeclaringType", nameof(fieldInfo));
		}

		private SerializableFieldInfo(SerializationInfo info, StreamingContext context)
		{
			System.Type declaringType = info.GetValue<SerializableSystemType>("declaringType").GetSystemType();
			string fieldName = info.GetString("fieldName");

			_fieldInfo = declaringType.GetField(
				fieldName,
				BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			if (_fieldInfo == null) throw new MissingFieldException(declaringType.FullName, fieldName);
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("declaringType", SerializableSystemType.Wrap(_fieldInfo.DeclaringType));
			info.AddValue("fieldName", _fieldInfo.Name);
		}

		public FieldInfo Value => _fieldInfo;

		public bool Equals(SerializableFieldInfo other)
		{
			return other != null && Equals(_fieldInfo, other._fieldInfo);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj is SerializableFieldInfo && Equals((SerializableFieldInfo) obj);
		}

		public override int GetHashCode()
		{
			return (_fieldInfo != null ? _fieldInfo.GetHashCode() : 0);
		}
	}
}
