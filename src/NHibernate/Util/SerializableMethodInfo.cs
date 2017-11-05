using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace NHibernate.Util
{
	[Serializable]
	internal sealed class SerializableMethodInfo : ISerializable, IEquatable<SerializableMethodInfo>
	{
		[NonSerialized]
		private readonly MethodInfo _methodInfo;

		/// <summary>
		/// Creates a new instance of <see cref="SerializableMethodInfo"/> if 
		/// <paramref name="methodInfo"/> is not null, otherwise returns <c>null</c>.
		/// </summary>
		/// <param name="methodInfo">The <see cref="MethodInfo"/> being wrapped for serialization.</param>
		/// <returns>New instance of <see cref="SerializableMethodInfo"/> or <c>null</c>.</returns>
		public static SerializableMethodInfo Wrap(MethodInfo methodInfo)
		{
			return methodInfo == null ? null : new SerializableMethodInfo(methodInfo);
		}

		/// <summary>
		/// Creates a new <see cref="SerializableMethodInfo"/>
		/// </summary>
		/// <param name="methodInfo">The <see cref="MethodInfo"/> being wrapped for serialization.</param>
		private SerializableMethodInfo(MethodInfo methodInfo)
		{
			_methodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
			if (methodInfo.IsStatic) throw new ArgumentException("Only for instance fields", nameof(methodInfo));
			if (methodInfo.DeclaringType == null) throw new ArgumentException("MethodInfo must have non-null DeclaringType", nameof(methodInfo));
		}

		private SerializableMethodInfo(SerializationInfo info, StreamingContext context)
		{
			System.Type declaringType = info.GetValue<SerializableSystemType>("declaringType").GetSystemType();
			string methodName = info.GetString("methodName");
			SerializableSystemType[] parameterSystemTypes = info.GetValue<SerializableSystemType[]>("parameterTypesHelper");

			System.Type[] parameterTypes = parameterSystemTypes?.Select(x => x.GetSystemType()).ToArray() ?? new System.Type[0];
			_methodInfo = declaringType.GetMethod(
				methodName,
				BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null);

			if (_methodInfo == null) throw new MissingMethodException(declaringType.FullName, methodName);
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			SerializableSystemType[] parameterSystemTypes =
				_methodInfo.GetParameters()
				           .Select(x => SerializableSystemType.Wrap(x.ParameterType))
				           .ToArray();

			info.AddValue("declaringType", SerializableSystemType.Wrap(_methodInfo.DeclaringType));
			info.AddValue("methodName", _methodInfo.Name);
			info.AddValue("parameterTypesHelper", parameterSystemTypes);
		}

		public MethodInfo Value => _methodInfo;

		public bool Equals(SerializableMethodInfo other)
		{
			return other != null && Equals(_methodInfo, other._methodInfo);
		}
	}
}
