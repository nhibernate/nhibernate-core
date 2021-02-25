#if !NETFX
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace NHibernate.Util
{
	public static partial class SerializationHelper
	{
		private sealed class TypeSerializationSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				info.SetType(typeof(TypeReference));
				new TypeReference((System.Type) obj).GetObjectData(info, context);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) =>
				throw new NotSupportedException();
		}

		[Serializable]
		private sealed class TypeReference : IObjectReference, ISerializable
		{
			private readonly string _assemblyName;
			private readonly string _fullName;

			public TypeReference(System.Type type)
			{
				if (type == null)
					throw new ArgumentNullException(nameof(type));

				_assemblyName = type.Assembly.FullName;
				_fullName = type.FullName;
			}

			private TypeReference(SerializationInfo info, StreamingContext context)
			{
				_assemblyName = info.GetString("AssemblyName");
				_fullName = info.GetString("FullName");
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("AssemblyName", _assemblyName);
				info.AddValue("FullName", _fullName);
			}

			public object GetRealObject(StreamingContext context) =>
				Assembly.Load(_assemblyName).GetType(_fullName, true);
		}

		private sealed class MemberInfoSerializationSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				info.SetType(typeof(MemberInfoReference));
				new MemberInfoReference((MemberInfo) obj).GetObjectData(info, context);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) =>
				throw new NotSupportedException();
		}

		[Serializable]
		private sealed class MemberInfoReference : IObjectReference, ISerializable
		{
			private readonly System.Type _declaringType;
			private readonly string _name;
			private readonly MemberTypes _memberType;
			private readonly BindingFlags _bindingFlags;
			private readonly System.Type[] _genericArguments;
			private readonly System.Type[] _parameterTypes;

			public MemberInfoReference(MemberInfo member)
			{
				if (member == null)
					throw new ArgumentNullException(nameof(member));

				_declaringType = member.DeclaringType;
				_name = member.Name;
				_memberType = member.MemberType;
				_bindingFlags = GetBindingFlags(member);
				if (member is MethodBase method)
				{
					_genericArguments = method.IsGenericMethod ? method.GetGenericArguments() : System.Type.EmptyTypes;
					_parameterTypes = method.GetParameters().ToArray(p => p.ParameterType);
				}
			}

			private MemberInfoReference(SerializationInfo info, StreamingContext context)
			{
				_declaringType = info.GetValue<System.Type>("DeclaringType");
				_name = info.GetString("Name");
				_memberType = info.GetValue<MemberTypes>("MemberType");
				_bindingFlags = info.GetValue<BindingFlags>("BindingFlags");
				_genericArguments = info.GetValueArray<System.Type>("GenericArguments");
				_parameterTypes = info.GetValueArray<System.Type>("ParameterTypes");
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValue("DeclaringType", _declaringType);
				info.AddValue("Name", _name);
				info.AddValue("MemberType", _memberType);
				info.AddValue("BindingFlags", _bindingFlags);
				info.AddValueArray("GenericArguments", _genericArguments);
				info.AddValueArray("ParameterTypes", _parameterTypes);
			}

			public object GetRealObject(StreamingContext context)
			{
				var members = _declaringType.GetMember(_name, _memberType, _bindingFlags | BindingFlags.DeclaredOnly);
				if (members.Length == 0) throw new MissingMemberException(_declaringType.FullName, _name);

				if (_memberType == MemberTypes.Method || _memberType == MemberTypes.Constructor)
				{
					try
					{
						return members.Cast<MethodBase>().First(MatchMethodSignature);
					}
					catch (InvalidOperationException)
					{
						throw new MissingMethodException(_declaringType.FullName, _name);
					}
				}

				if (members.Length > 1)
					throw new AmbiguousMatchException($"Found multiple \"{_name}\" in \"{_declaringType}\".");

				return members[0];
			}

			private bool MatchMethodSignature(MethodBase method)
			{
				var gpa = method.IsGenericMethod ? method.GetGenericArguments() : System.Type.EmptyTypes;
				if (gpa.Length != _genericArguments.Length) return false;
				var pa = method.GetParameters();
				if (pa.Length != _parameterTypes.Length) return false;
				if (gpa.Length > 0)
				{
					var genericMethod = ((MethodInfo) method).MakeGenericMethod(_genericArguments);
					pa = genericMethod.GetParameters();
				}

				for (int i = 0; i < pa.Length; i++)
				{
					if (pa[i].ParameterType != _parameterTypes[i])
					{
						return false;
					}
				}

				return true;
			}

			private static BindingFlags GetBindingFlags(MemberInfo member)
			{
				if (member == null) throw new ArgumentNullException(nameof(member));
				var bindingFlags = BindingFlags.Default;
				switch (member)
				{
					case MethodBase method:
						bindingFlags |= method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
						bindingFlags |= method.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
						break;

					case FieldInfo field:
						bindingFlags |= field.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
						bindingFlags |= field.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
						break;

					default:
						return BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
				}

				return bindingFlags;
			}
		}

		private sealed class DelegateSerializationSurrogate : ISerializationSurrogate
		{
			public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
			{
				info.SetType(typeof(DelegateReference));
				new DelegateReference((Delegate) obj).GetObjectData(info, context);
			}

			public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) =>
				throw new NotSupportedException();
		}

		[Serializable]
		private sealed class DelegateReference : IObjectReference, ISerializable
		{
			private readonly System.Type[] _types;
			private readonly MethodInfo[] _methods;
			private readonly object[] _targets;

			public DelegateReference(Delegate @delegate)
			{
				if (@delegate == null)
					throw new ArgumentNullException(nameof(@delegate));

				var invocations = @delegate.GetInvocationList();
				_types = new System.Type[invocations.Length];
				_methods = new MethodInfo[invocations.Length];
				_targets = new object[invocations.Length];
				for (var i = 0; i < invocations.Length; i++)
				{
					var invocation = invocations[i];
					_types[i] = invocation.GetType();
					_methods[i] = invocation.Method;
					_targets[i] = invocation.Target;
				}
			}

			private DelegateReference(SerializationInfo info, StreamingContext context)
			{
				_types = GetValueArray<System.Type>(info, "Types");
				_methods = info.GetValueArray<MethodInfo>("Methods");
				_targets = info.GetValueArray<object>("Targets");
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				info.AddValueArray("Types", _types);
				info.AddValueArray("Methods", _methods);
				info.AddValueArray("Targets", _targets);
			}

			public object GetRealObject(StreamingContext context)
			{
				var delegates = new Delegate[_types.Length];
				for (var i = 0; i < delegates.Length; i++)
					delegates[i] = Delegate.CreateDelegate(_types[i], _targets[i], _methods[i]);

				return delegates.Length == 0
					? delegates[0]
					: Delegate.Combine(delegates);
			}
		}

		public sealed class SurrogateSelector : System.Runtime.Serialization.SurrogateSelector
		{
			private static readonly bool SystemTypeIsNotSerializable = !typeof(System.Type).IsSerializable;
			private static readonly bool MemberInfoIsNotSerializable = !typeof(MemberInfo).IsSerializable;
			private static readonly bool DelegateIsNotSerializable = !GetDelegateIsSerializable();

			private static bool GetDelegateIsSerializable()
			{
				//In .NET Standard & .NET Core Delegate implements ISerializable,
				//but throws SerializationException
				if (!typeof(Delegate).IsSerializable)
					return false;
				try
				{
					System.Action a = () => { };
					a.GetObjectData(
						new SerializationInfo(typeof(Delegate), new FormatterConverter()),
						new StreamingContext(StreamingContextStates.All));
					return true;
				}
				catch (PlatformNotSupportedException)
				{
					return false;
				}
				catch (SerializationException)
				{
					return false;
				}
			}

			public override ISerializationSurrogate GetSurrogate(
				System.Type type,
				StreamingContext context,
				out ISurrogateSelector selector)
			{
				if (SystemTypeIsNotSerializable && typeof(System.Type).IsAssignableFrom(type))
				{
					selector = this;
					return new TypeSerializationSurrogate();
				}

				if (MemberInfoIsNotSerializable && typeof(MemberInfo).IsAssignableFrom(type))
				{
					selector = this;
					return new MemberInfoSerializationSurrogate();
				}

				if (DelegateIsNotSerializable && typeof(Delegate).IsAssignableFrom(type))
				{
					selector = this;
					return new DelegateSerializationSurrogate();
				}

				return base.GetSurrogate(type, context, out selector);
			}

			public override void AddSurrogate(System.Type type, StreamingContext context, ISerializationSurrogate surrogate) =>
				throw new NotSupportedException("This is a static SurrogateSelector");

			public override void RemoveSurrogate(System.Type type, StreamingContext context) =>
				throw new NotSupportedException("This is a static SurrogateSelector");
		}
	}
}
#endif
