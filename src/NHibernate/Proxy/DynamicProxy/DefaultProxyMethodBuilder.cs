#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	internal class DefaultyProxyMethodBuilder : IProxyMethodBuilder
	{
		public DefaultyProxyMethodBuilder() : this(new DefaultMethodEmitter()) { }

		public DefaultyProxyMethodBuilder(IMethodBodyEmitter emitter)
		{
			if (emitter == null)
			{
				throw new ArgumentNullException("emitter");
			}
			MethodBodyEmitter = emitter;
		}

		public IMethodBodyEmitter MethodBodyEmitter { get; private set; }

		private static MethodBuilder GenerateMethodSignature(string name, MethodInfo method, TypeBuilder typeBuilder)
		{
			//TODO: Should we use attributes of base method?
			var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;

			if (method.IsSpecialName)
				methodAttributes |= MethodAttributes.SpecialName;

			ParameterInfo[] parameters = method.GetParameters();

			MethodBuilder methodBuilder = typeBuilder.DefineMethod(name,
																   methodAttributes,
																   CallingConventions.HasThis,
																   method.ReturnType,
																   parameters.Select(param => param.ParameterType).ToArray());

			System.Type[] typeArgs = method.GetGenericArguments();

			if (typeArgs.Length > 0)
			{
				var typeNames = GenerateTypeNames(typeArgs.Length);
				var typeArgBuilders = methodBuilder.DefineGenericParameters(typeNames);

				for (int index = 0; index < typeArgs.Length; index++)
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
			for (int index = 0; index < constraintGenericArguments.Length; index++)
			{
				var genericArgument = constraintGenericArguments[index];
				System.Type result;
				if (parametersMap.TryGetValue(genericArgument, out result))
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
			for (int index = 0; index < count; index++)
			{
				result[index] = string.Format("T{0}", index); 
			}
			return result;
		}

		public void CreateProxiedMethod(FieldInfo field, MethodInfo method, TypeBuilder typeBuilder)
		{
			var callbackMethod = GenerateMethodSignature(method.Name + "_callback", method, typeBuilder);
			var proxyMethod = GenerateMethodSignature(method.Name, method, typeBuilder);

			MethodBodyEmitter.EmitMethodBody(proxyMethod, callbackMethod, method, field);
		}
	}
}