#region Credits

// Part of this work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Proxy.DynamicProxy;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	internal static class ProxyBuilderHelper
	{
		private static readonly ConstructorInfo ObjectConstructor = typeof(object).GetConstructor(System.Type.EmptyTypes);
		private static readonly ConstructorInfo SecurityCriticalAttributeConstructor = typeof(SecurityCriticalAttribute).GetConstructor(System.Type.EmptyTypes);
		private static readonly ConstructorInfo IgnoresAccessChecksToAttributeConstructor = typeof(IgnoresAccessChecksToAttribute).GetConstructor(new[] {typeof(string)});

		internal static readonly MethodInfo SerializableGetObjectDataMethod = typeof(ISerializable).GetMethod(nameof(ISerializable.GetObjectData));
		internal static readonly MethodInfo SerializationInfoSetTypeMethod = ReflectHelper.GetMethod<SerializationInfo>(si => si.SetType(null));

#if NETFX
		private static bool _saveAssembly;
		private static string _saveAssemblyPath;

		// Called by reflection
		internal static void EnableDynamicAssemblySaving(bool enable, string saveAssemblyPath)
		{
			_saveAssembly = enable;
			_saveAssemblyPath = saveAssemblyPath;
		}
#endif

		internal static AssemblyBuilder DefineDynamicAssembly(AppDomain appDomain, AssemblyName name)
		{
#if NETFX
			var access = _saveAssembly ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run;
			var assemblyBuilder = _saveAssembly && !string.IsNullOrEmpty(_saveAssemblyPath)
				? appDomain.DefineDynamicAssembly(name, access, Path.GetDirectoryName(_saveAssemblyPath))
				: AssemblyBuilder.DefineDynamicAssembly(name, access);
#else
			var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#endif
			return assemblyBuilder;
		}

		internal static ModuleBuilder DefineDynamicModule(AssemblyBuilder assemblyBuilder, string moduleName)
		{
#if NETFX
			var moduleBuilder = _saveAssembly
				? assemblyBuilder.DefineDynamicModule(moduleName, $"{moduleName}.mod", true)
				: assemblyBuilder.DefineDynamicModule(moduleName);
#else
			var moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif
			return moduleBuilder;
		}

		internal static void Save(AssemblyBuilder assemblyBuilder)
		{
#if NETFX
			if (_saveAssembly)
				assemblyBuilder.Save(
					string.IsNullOrEmpty(_saveAssemblyPath)
						? "generatedAssembly.dll"
						: Path.GetFileName(_saveAssemblyPath));
#endif
		}

		internal static void CallDefaultBaseConstructor(ILGenerator il, System.Type parentType)
		{
			var baseConstructor = parentType.GetConstructor(
				BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
				null,
				System.Type.EmptyTypes,
				null);
			// if there is no default constructor, or the default constructor is private/internal, call System.Object constructor
			// this works, but the generated assembly will fail PeVerify (cannot use in medium trust for example)
			if (baseConstructor == null || baseConstructor.IsPrivate || baseConstructor.IsAssembly)
				baseConstructor = ObjectConstructor;

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Call, baseConstructor);
		}

		internal static IEnumerable<MethodInfo> GetProxiableMethods(System.Type type)
		{
			const BindingFlags candidateMethodsBindingFlags =
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

			return type.GetMethods(candidateMethodsBindingFlags).Where(m => m.IsProxiable());
		}

		internal static IEnumerable<MethodInfo> GetProxiableMethods(System.Type type, IEnumerable<System.Type> interfaces)
		{
			if (type.IsInterface || type == typeof(object) || type.GetInterfaces().Length == 0)
			{
				return GetProxiableMethods(type)
					.Concat(interfaces.SelectMany(i => i.GetMethods()))
					.Distinct();
			}

			var proxiableMethods = new HashSet<MethodInfo>(GetProxiableMethods(type), new MethodInfoComparer(type));
			foreach (var interfaceMethod in interfaces.SelectMany(i => i.GetMethods()))
			{
				proxiableMethods.Add(interfaceMethod);
			}

			return proxiableMethods;
		}

		internal static void MakeProxySerializable(TypeBuilder typeBuilder)
		{
			var serializableConstructor = typeof(SerializableAttribute).GetConstructor(System.Type.EmptyTypes);
			var customAttributeBuilder = new CustomAttributeBuilder(serializableConstructor, Array.Empty<object>());
			typeBuilder.SetCustomAttribute(customAttributeBuilder);
		}

		internal static MethodBuilder GetObjectDataMethodBuilder(TypeBuilder typeBuilder)
		{
			const MethodAttributes attributes =
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

			var parameterTypes = new[] { typeof(SerializationInfo), typeof(StreamingContext) };

			var methodBuilder = typeBuilder.DefineMethod(
				nameof(ISerializable.GetObjectData),
				attributes,
				typeof(void),
				parameterTypes);
			methodBuilder.SetImplementationFlags(MethodImplAttributes.IL | MethodImplAttributes.Managed);
			methodBuilder.SetCustomAttribute(
				new CustomAttributeBuilder(SecurityCriticalAttributeConstructor, Array.Empty<object>()));
			return methodBuilder;
		}

		internal static MethodBuilder GenerateMethodSignature(string name, MethodInfo method, TypeBuilder typeBuilder)
		{
			var explicitImplementation = method.DeclaringType.IsInterface;
			if (explicitImplementation &&
				(typeBuilder.BaseType == typeof(object) ||
#pragma warning disable 618
					typeBuilder.BaseType == typeof(ProxyDummy)) &&
#pragma warning restore 618
				(IsEquals(method) || IsGetHashCode(method)))
			{
				// If we are building a proxy for an interface, and it defines an Equals or GetHashCode, they must
				// be implicitly implemented for overriding object methods.
				// (Ideally we should check the method actually comes from the interface declared for the proxy.)
				explicitImplementation = false;
			}

			var methodAttributes = explicitImplementation
				? MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig |
					MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual
				: MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (method.IsSpecialName)
				methodAttributes |= MethodAttributes.SpecialName;

			var parameters = method.GetParameters();
			var implementationName = explicitImplementation
				? $"{method.DeclaringType.FullName}.{name}"
				: name;

			var methodBuilder =
				typeBuilder.DefineMethod(
					implementationName,
					methodAttributes,
					CallingConventions.HasThis,
					method.ReturnType,
					method.ReturnParameter?.GetRequiredCustomModifiers(),
					method.ReturnParameter?.GetOptionalCustomModifiers(),
					parameters.ToArray(p => p.ParameterType),
					parameters.ToArray(p => p.GetRequiredCustomModifiers()),
					parameters.ToArray(p => p.GetOptionalCustomModifiers()));

			var typeArgs = method.GetGenericArguments();
			if (typeArgs.Length > 0)
			{
				var typeNames = GenerateTypeNames(typeArgs.Length);
				var typeArgBuilders = methodBuilder.DefineGenericParameters(typeNames);

				for (var index = 0; index < typeArgs.Length; index++)
				{
					// Copy generic parameter attributes (Covariant, Contravariant, ReferenceTypeConstraint,
					// NotNullableValueTypeConstraint, DefaultConstructorConstraint).
					var typeArgBuilder = typeArgBuilders[index];
					var typeArg = typeArgs[index];

					typeArgBuilder.SetGenericParameterAttributes(typeArg.GenericParameterAttributes);

					// Copy generic parameter constraints (class and interfaces).
					var typeConstraints = typeArg.GetGenericParameterConstraints()
												.ToArray(x => ResolveTypeConstraint(method, x));

					var baseTypeConstraint = typeConstraints.SingleOrDefault(x => x.IsClass);
					typeArgBuilder.SetBaseTypeConstraint(baseTypeConstraint);

					var interfaceTypeConstraints = typeConstraints.Where(x => !x.IsClass).ToArray();
					typeArgBuilder.SetInterfaceConstraints(interfaceTypeConstraints);
				}
			}

			if (explicitImplementation)
				methodBuilder.SetImplementationFlags(MethodImplAttributes.Managed | MethodImplAttributes.IL);

			return methodBuilder;
		}

		private static bool IsGetHashCode(MethodBase method)
		{
			return method.Name == "GetHashCode" && method.GetParameters().Length == 0;
		}

		private static bool IsEquals(MethodBase method)
		{
			if (method.Name != "Equals") return false;
			var parameters = method.GetParameters();
			return parameters.Length == 1 && parameters[0].ParameterType == typeof(object);
		}

		internal static void GenerateInstanceOfIgnoresAccessChecksToAttribute(
			AssemblyBuilder assemblyBuilder,
			string assemblyName)
		{
			// [assembly: System.Runtime.CompilerServices.IgnoresAccessChecksToAttribute(assemblyName)]
			var customAttributeBuilder = new CustomAttributeBuilder(
				IgnoresAccessChecksToAttributeConstructor,
				new object[] {assemblyName});

			assemblyBuilder.SetCustomAttribute(customAttributeBuilder);
		}

		private static System.Type ResolveTypeConstraint(MethodInfo method, System.Type typeConstraint)
		{
			if (typeConstraint != null && typeConstraint.IsGenericType)
			{
				var declaringType = method.DeclaringType;
				if (declaringType != null && declaringType.IsGenericType)
				{
					return BuildTypeConstraint(typeConstraint, declaringType);
				}
			}

			return typeConstraint;
		}

		private static System.Type BuildTypeConstraint(System.Type typeConstraint, System.Type declaringType)
		{
			var constraintGenericArguments = typeConstraint.GetGenericArguments();
			var declaringTypeGenericArguments = declaringType.GetGenericArguments();

			var parametersMap = declaringType
			                    .GetGenericTypeDefinition()
			                    .GetGenericArguments()
			                    .ToDictionary(x => x, x => declaringTypeGenericArguments[x.GenericParameterPosition]);

			var args = new System.Type[constraintGenericArguments.Length];
			var make = false;
			for (var index = 0; index < constraintGenericArguments.Length; index++)
			{
				var genericArgument = constraintGenericArguments[index];
				if (parametersMap.TryGetValue(genericArgument, out var result))
				{
					make = true;
				}
				else
				{
					result = genericArgument;
				}

				args[index] = result;
			}

			if (make)
			{
				return typeConstraint.GetGenericTypeDefinition().MakeGenericType(args);
			}

			return typeConstraint;
		}

		private static string[] GenerateTypeNames(int count)
		{
			var result = new string[count];
			for (var index = 0; index < count; index++)
			{
				result[index] = $"T{index}";
			}

			return result;
		}

		/// <summary>
		/// Method equality for the proxy building purpose: we want to equate an interface method to a base type
		/// method which implements it. This implies the base type method has the same signature and there is no
		/// explicit implementation of the interface method in the base type.
		/// </summary>
		private class MethodInfoComparer : IEqualityComparer<MethodInfo>
		{
			private readonly Dictionary<System.Type, Dictionary<MethodInfo, MethodInfo>> _interfacesMap;

			public MethodInfoComparer(System.Type baseType)
			{
				_interfacesMap = BuildInterfacesMap(baseType);
			}

			private static Dictionary<System.Type, Dictionary<MethodInfo, MethodInfo>> BuildInterfacesMap(System.Type type)
			{
				return type.GetInterfaces()
					.Distinct()
					.ToDictionary(i => i, i => ToDictionary(type.GetInterfaceMap(i)));
			}

			private static Dictionary<MethodInfo, MethodInfo> ToDictionary(InterfaceMapping interfaceMap)
			{
				var map = new Dictionary<MethodInfo, MethodInfo>(interfaceMap.InterfaceMethods.Length);
				for (var i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
				{
					map.Add(interfaceMap.InterfaceMethods[i], interfaceMap.TargetMethods[i]);
				}

				return map;
			}

			public bool Equals(MethodInfo x, MethodInfo y)
			{
				if (x == y)
					return true;
				if (x == null || y == null)
					return false;
				if (x.Name != y.Name)
					return false;

				// If they have the same declaring type, one cannot be the implementation of the other.
				if (x.DeclaringType == y.DeclaringType)
					return false;
				// If they belong to two different interfaces or to two different concrete types, one cannot be the
				// implementation of the other.
				if (x.DeclaringType.IsInterface == y.DeclaringType.IsInterface)
					return false;

				var interfaceMethod = x.DeclaringType.IsInterface ? x : y;
				// If the interface is not implemented by the base type, the method cannot be implemented by the
				// base type method.
				if (!_interfacesMap.TryGetValue(interfaceMethod.DeclaringType, out var map))
					return false;

				var baseMethod = x.DeclaringType.IsInterface ? y : x;
				return map[interfaceMethod] == baseMethod;
			}

			public int GetHashCode(MethodInfo obj)
			{
				// Hashing by name only, putting methods with the same name in the same bucket, in order to keep
				// this method fast.
				return obj.Name.GetHashCode();
			}
		}
	}
}
