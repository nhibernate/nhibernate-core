#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	internal class DefaultMethodEmitter : IMethodBodyEmitter
	{
		private static readonly MethodInfo getInterceptor;

		private static readonly MethodInfo getGenericMethodFromHandle = typeof (MethodBase).GetMethod("GetMethodFromHandle",
		                                                                                              BindingFlags.Public | BindingFlags.Static, null,
		                                                                                              new[] {typeof (RuntimeMethodHandle), typeof (RuntimeTypeHandle)}, null);

		private static readonly MethodInfo getMethodFromHandle = typeof (MethodBase).GetMethod("GetMethodFromHandle", new[] {typeof (RuntimeMethodHandle)});
		private static readonly MethodInfo getTypeFromHandle = typeof(System.Type).GetMethod("GetTypeFromHandle");
		private static readonly MethodInfo handlerMethod = typeof (IInterceptor).GetMethod("Intercept");
		private static readonly ConstructorInfo infoConstructor;
		private static readonly PropertyInfo interceptorProperty = typeof (IProxy).GetProperty("Interceptor");

		private static readonly ConstructorInfo notImplementedConstructor = typeof(NotImplementedException).GetConstructor(new System.Type[0]);

		private readonly IArgumentHandler _argumentHandler;

		static DefaultMethodEmitter()
		{
			getInterceptor = interceptorProperty.GetGetMethod();
			var constructorTypes = new[]
			                       {
			                       	typeof (object), typeof (MethodInfo),
			                       	typeof (StackTrace), typeof (System.Type[]), typeof (object[])
			                       };

			infoConstructor = typeof (InvocationInfo).GetConstructor(constructorTypes);
		}

		public DefaultMethodEmitter() : this(new DefaultArgumentHandler()) {}

		public DefaultMethodEmitter(IArgumentHandler argumentHandler)
		{
			_argumentHandler = argumentHandler;
		}

		#region IMethodBodyEmitter Members

		public void EmitMethodBody(ILGenerator IL, MethodInfo method, FieldInfo field)
		{
			ParameterInfo[] parameters = method.GetParameters();
			IL.DeclareLocal(typeof (object[]));
			IL.DeclareLocal(typeof (InvocationInfo));
			IL.DeclareLocal(typeof(System.Type[]));

			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Callvirt, getInterceptor);

			// if (interceptor == null)
			// 		return base.method(...);

			Label skipBaseCall = IL.DefineLabel();

			IL.Emit(OpCodes.Ldnull);
			IL.Emit(OpCodes.Bne_Un, skipBaseCall);

			IL.Emit(OpCodes.Ldarg_0);

			for(int i=0; i<method.GetParameters().Length; i++)
				IL.Emit(OpCodes.Ldarg_S, (sbyte)(i + 1));

			IL.Emit(OpCodes.Call, method);
			IL.Emit(OpCodes.Ret);

			IL.MarkLabel(skipBaseCall);

			// Push the 'this' pointer onto the stack
			IL.Emit(OpCodes.Ldarg_0);

			// Push the MethodInfo onto the stack            
			System.Type declaringType = method.DeclaringType;

			IL.Emit(OpCodes.Ldtoken, method);
			if (declaringType.IsGenericType)
			{
				IL.Emit(OpCodes.Ldtoken, declaringType);
				IL.Emit(OpCodes.Call, getGenericMethodFromHandle);
			}
			else
			{
				IL.Emit(OpCodes.Call, getMethodFromHandle);
			}

			IL.Emit(OpCodes.Castclass, typeof (MethodInfo));

			PushStackTrace(IL);
			PushGenericArguments(method, IL);
			_argumentHandler.PushArguments(parameters, IL, false);

			// InvocationInfo info = new InvocationInfo(...);

			IL.Emit(OpCodes.Newobj, infoConstructor);
			IL.Emit(OpCodes.Stloc_1);

			// this.Interceptor.Intercept(info);
			IL.Emit(OpCodes.Ldarg_0);
			IL.Emit(OpCodes.Callvirt, getInterceptor);
			IL.Emit(OpCodes.Ldloc_1);
			IL.Emit(OpCodes.Callvirt, handlerMethod);

			PackageReturnType(method, IL);
			SaveRefArguments(IL, parameters);

			IL.Emit(OpCodes.Ret);
		}

		#endregion

		private static void SaveRefArguments(ILGenerator IL, ParameterInfo[] parameters)
		{
			// Save the arguments returned from the handler method
			MethodInfo getArguments = typeof (InvocationInfo).GetMethod("get_Arguments");
			IL.Emit(OpCodes.Ldloc_1);
			IL.Emit(OpCodes.Call, getArguments);
			IL.Emit(OpCodes.Stloc_0);

			foreach (ParameterInfo param in parameters)
			{
				string typeName = param.ParameterType.Name;

				bool isRef = param.ParameterType.IsByRef && typeName.EndsWith("&");
				if (!isRef)
				{
					continue;
				}

				// Load the destination address
				IL.Emit(OpCodes.Ldarg, param.Position + 1);

				// Load the argument value
				IL.Emit(OpCodes.Ldloc_0);
				IL.Emit(OpCodes.Ldc_I4, param.Position);
				IL.Emit(OpCodes.Ldelem_Ref);

				System.Type unboxedType = param.ParameterType.GetElementType();

				IL.Emit(OpCodes.Unbox_Any, unboxedType);

				OpCode stind = GetStindInstruction(param.ParameterType);
				IL.Emit(stind);
			}
		}

		private static OpCode GetStindInstruction(System.Type parameterType)
		{
			if (parameterType.IsByRef)
			{
				OpCode stindOpCode;
				if(OpCodesMap.TryGetStindOpCode(parameterType.GetElementType(), out stindOpCode))
				{
					return stindOpCode;
				}
			}

			return OpCodes.Stind_Ref;
		}

		private void PushStackTrace(ILGenerator IL)
		{
			// NOTE: The stack trace has been disabled for performance reasons
			IL.Emit(OpCodes.Ldnull);
		}

		private void PushGenericArguments(MethodInfo method, ILGenerator IL)
		{
			System.Type[] typeParameters = method.GetGenericArguments();

			// If this is a generic method, we need to store
			// the generic method arguments
			int genericTypeCount = typeParameters.Length;

			// Type[] genericTypeArgs = new Type[genericTypeCount];
			IL.Emit(OpCodes.Ldc_I4, genericTypeCount);
			IL.Emit(OpCodes.Newarr, typeof(System.Type));

			if (genericTypeCount == 0)
			{
				return;
			}

			for (int index = 0; index < genericTypeCount; index++)
			{
				System.Type currentType = typeParameters[index];

				IL.Emit(OpCodes.Dup);
				IL.Emit(OpCodes.Ldc_I4, index);
				IL.Emit(OpCodes.Ldtoken, currentType);
				IL.Emit(OpCodes.Call, getTypeFromHandle);
				IL.Emit(OpCodes.Stelem_Ref);
			}
		}

		private void PackageReturnType(MethodInfo method, ILGenerator IL)
		{
			System.Type returnType = method.ReturnType;
			// Unbox the return value if necessary
			if (returnType == typeof (void))
			{
				IL.Emit(OpCodes.Pop);
				return;
			}

			IL.Emit(OpCodes.Unbox_Any, returnType);
		}
	}
}