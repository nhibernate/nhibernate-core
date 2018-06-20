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
using System.Runtime.Serialization;
using System.Security;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	internal static class ProxyBuilderHelper
	{
		private static readonly ConstructorInfo ObjectConstructor = typeof(object).GetConstructor(System.Type.EmptyTypes);
		private static readonly ConstructorInfo SecurityCriticalAttributeConstructor = typeof(SecurityCriticalAttribute).GetConstructor(System.Type.EmptyTypes);

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

		internal static IEnumerable<MethodInfo> GetProxiableMethods(System.Type type, IEnumerable<System.Type> interfaces)
		{
			const BindingFlags candidateMethodsBindingFlags =
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			return
				type.GetMethods(candidateMethodsBindingFlags)
				    .Where(method => method.IsProxiable())
				    .Concat(interfaces.SelectMany(interfaceType => interfaceType.GetMethods()))
				    .Distinct();
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
			//TODO: Should we use attributes of base method?
			var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (method.IsSpecialName)
				methodAttributes |= MethodAttributes.SpecialName;

			var parameters = method.GetParameters();

			var methodBuilder = typeBuilder.DefineMethod(
				name,
				methodAttributes,
				CallingConventions.HasThis,
				method.ReturnType,
				parameters.Select(param => param.ParameterType).ToArray());

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
					                             .Select(x => ResolveTypeConstraint(method, x))
					                             .ToArray();

					var baseTypeConstraint = typeConstraints.SingleOrDefault(x => x.IsClass);
					typeArgBuilder.SetBaseTypeConstraint(baseTypeConstraint);

					var interfaceTypeConstraints = typeConstraints.Where(x => !x.IsClass).ToArray();
					typeArgBuilder.SetInterfaceConstraints(interfaceTypeConstraints);
				}
			}

			return methodBuilder;
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
	}
}
