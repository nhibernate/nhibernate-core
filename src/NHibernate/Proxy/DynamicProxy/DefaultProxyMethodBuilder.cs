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

		#region IProxyMethodBuilder Members

		public void CreateProxiedMethod(FieldInfo field, MethodInfo method, TypeBuilder typeBuilder)
		{
			const MethodAttributes methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig |
																								MethodAttributes.Virtual;
			ParameterInfo[] parameters = method.GetParameters();

			MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes,
																   CallingConventions.HasThis, method.ReturnType,
																   parameters.Select(param => param.ParameterType).ToArray());

			System.Type[] typeArgs = method.GetGenericArguments();

			if (typeArgs.Length > 0)
			{
				var typeNames = new List<string>();

				for (int index = 0; index < typeArgs.Length; index++)
				{
					typeNames.Add(string.Format("T{0}", index));
				}

				var typeArgsBuilder = methodBuilder.DefineGenericParameters(typeNames.ToArray());

				for (int index = 0; index < typeArgs.Length; index++)
				{
					// Copy generic parameter attributes (Covariant, Contravariant, ReferenceTypeConstraint,
					// NotNullableValueTypeConstraint, DefaultConstructorConstraint).
					typeArgsBuilder[index].SetGenericParameterAttributes(typeArgs[index].GenericParameterAttributes);

					System.Type[] typeConstraints = typeArgs[index].GetGenericParameterConstraints();

					if (typeConstraints.Length > 0)
					{
						// Copy generic parameter constraints (class and interfaces).
						System.Type baseTypeConstraint = typeConstraints.SingleOrDefault(x => x.IsClass);
						System.Type[] interfaceTypeConstraints = typeConstraints.Where(x => !x.IsClass).ToArray();

						if (baseTypeConstraint != null)
						{
							typeArgsBuilder[index].SetBaseTypeConstraint(baseTypeConstraint);
						}

						if (interfaceTypeConstraints.Length > 0)
						{
							typeArgsBuilder[index].SetInterfaceConstraints(interfaceTypeConstraints);
						}
					}
				}
			}
			

			Debug.Assert(MethodBodyEmitter != null);
			MethodBodyEmitter.EmitMethodBody(methodBuilder, method, field);
		}

		#endregion
	}
}