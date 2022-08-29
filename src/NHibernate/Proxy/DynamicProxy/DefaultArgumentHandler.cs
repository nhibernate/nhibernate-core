#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	// Since v5.2
	[Obsolete("DynamicProxy namespace has been obsoleted, use static proxies instead (see StaticProxyFactory)")]
	internal class DefaultArgumentHandler : IArgumentHandler
	{
		#region IArgumentHandler Members

		public void PushArguments(ParameterInfo[] methodParameters, ILGenerator IL, bool isStatic)
		{
			ParameterInfo[] parameters = methodParameters ?? Array.Empty<ParameterInfo>();
			int parameterCount = parameters.Length;

			// object[] args = new object[size];
			IL.Emit(OpCodes.Ldc_I4, parameterCount);
			IL.Emit(OpCodes.Newarr, typeof(object));
			IL.Emit(OpCodes.Stloc_S, 0);

			if (parameterCount == 0)
			{
				IL.Emit(OpCodes.Ldloc_S, 0);
				return;
			}

			// Populate the object array with the list of arguments
			int index = 0;
			int argumentPosition = 1;
			foreach (ParameterInfo param in parameters)
			{
				System.Type parameterType = param.ParameterType.IsByRef ? param.ParameterType.GetElementType() : param.ParameterType;
				// args[N] = argumentN (pseudocode)
				IL.Emit(OpCodes.Ldloc_S, 0);
				IL.Emit(OpCodes.Ldc_I4, index);

				// Zero out the [out] parameters
				if (param.IsOut)
				{
					IL.Emit(OpCodes.Ldnull);
					IL.Emit(OpCodes.Stelem_Ref);
					argumentPosition++;
					index++;
					continue;
				}

				IL.Emit(OpCodes.Ldarg, argumentPosition);

				if (param.ParameterType.IsByRef)
				{
					var unboxedType = param.ParameterType.GetElementType();
					if (Nullable.GetUnderlyingType(unboxedType) != null)
					{
						IL.Emit(OpCodes.Ldobj, unboxedType);
					}
					else if (OpCodesMap.TryGetLdindOpCode(unboxedType, out var ldind))
					{
						IL.Emit(ldind);
					}
					else
					{
						IL.Emit(OpCodes.Ldind_Ref);
					}
				}

				if (parameterType.IsValueType || param.ParameterType.IsByRef || parameterType.IsGenericParameter)
				{
					IL.Emit(OpCodes.Box, parameterType);
				}

				IL.Emit(OpCodes.Stelem_Ref);

				index++;
				argumentPosition++;
			}
			IL.Emit(OpCodes.Ldloc_S, 0);
		}

		#endregion
	}
}
