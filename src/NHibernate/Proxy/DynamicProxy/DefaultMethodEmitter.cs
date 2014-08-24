#region Credits

// This work is based on LinFu.DynamicProxy framework (c) Philip Laureano who has donated it to NHibernate project.
// The license is the same of NHibernate license (LGPL Version 2.1, February 1999).
// The source was then modified to be the default DynamicProxy of NHibernate project.

#endregion

using System;
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

		private static readonly ConstructorInfo infoConstructor = typeof (InvocationInfo).GetConstructor(new[]
			{
				typeof (object),
				typeof (MethodInfo),
				typeof (MethodInfo),
				typeof (StackTrace),
				typeof (System.Type[]),
				typeof (object[])
			});

		private static readonly PropertyInfo interceptorProperty = typeof (IProxy).GetProperty("Interceptor");

		private static readonly ConstructorInfo notImplementedConstructor = typeof(NotImplementedException).GetConstructor(new System.Type[0]);

		private readonly IArgumentHandler _argumentHandler;

		static DefaultMethodEmitter()
		{
			getInterceptor = interceptorProperty.GetGetMethod();
		}

		public DefaultMethodEmitter() : this(new DefaultArgumentHandler()) {}

		public DefaultMethodEmitter(IArgumentHandler argumentHandler)
		{
			_argumentHandler = argumentHandler;
		}

		public void EmitMethodBody(MethodBuilder proxyMethod, MethodBuilder callbackMethod, MethodInfo method, FieldInfo field)
		{
			EmitBaseMethodCall(callbackMethod.GetILGenerator(), method);

			var IL = proxyMethod.GetILGenerator();

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

			EmitBaseMethodCall(IL, method);

			IL.MarkLabel(skipBaseCall);

			// Push arguments for InvocationInfo constructor.
			IL.Emit(OpCodes.Ldarg_0);  // 'this' pointer
			PushTargetMethodInfo(IL, proxyMethod, method);
			PushTargetMethodInfo(IL, callbackMethod, callbackMethod);
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

		private static void EmitBaseMethodCall(ILGenerator IL, MethodInfo method)
		{
			IL.Emit(OpCodes.Ldarg_0);

			for (int i = 0; i < method.GetParameters().Length; i++)
				IL.Emit(OpCodes.Ldarg_S, (sbyte) (i + 1));

			IL.Emit(OpCodes.Call, method);
			IL.Emit(OpCodes.Ret);
		}

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

		private static void PushTargetMethodInfo(ILGenerator IL, MethodBuilder generatedMethod, MethodInfo method)
		{
			if (method.IsGenericMethodDefinition)
			{
				// We want the generated code to load a MethodInfo instantiated with the
				// current generic type parameters. I.e.:
				// MethodInfo methodInfo = methodof(TheClass.TheMethod<T>(...)
				//
				// We need to instantiate the open generic method definition with the type
				// arguments from the generated method. Using the open method definition
				// directly works on .Net 2.0, which might be FW bug, but fails on 4.0.
				//
				// Equivalent pseudo-code:
				// MethodInfo mi = methodof(TheClass.TheMethod<>)
				// versus
				// MethodInfo mi = methodof(TheClass.TheMethod<T0>)
				var instantiatedMethod = method.MakeGenericMethod(generatedMethod.GetGenericArguments());
				IL.Emit(OpCodes.Ldtoken, instantiatedMethod);
			}
			else
			{
				IL.Emit(OpCodes.Ldtoken, method);
			}

			System.Type declaringType = method.DeclaringType;
			if (declaringType.IsGenericType)
			{
				IL.Emit(OpCodes.Ldtoken, declaringType);
				IL.Emit(OpCodes.Call, getGenericMethodFromHandle);
			}
			else
			{
				IL.Emit(OpCodes.Call, getMethodFromHandle);
			}

			IL.Emit(OpCodes.Castclass, typeof(MethodInfo));
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