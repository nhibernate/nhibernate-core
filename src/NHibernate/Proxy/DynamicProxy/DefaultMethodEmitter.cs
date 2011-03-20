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

		private static readonly Dictionary<string, OpCode> stindMap = new Dictionary<string, OpCode>();
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


			stindMap["Bool&"] = OpCodes.Stind_I1;
			stindMap["Int8&"] = OpCodes.Stind_I1;
			stindMap["Uint8&"] = OpCodes.Stind_I1;

			stindMap["Int16&"] = OpCodes.Stind_I2;
			stindMap["Uint16&"] = OpCodes.Stind_I2;

			stindMap["Uint32&"] = OpCodes.Stind_I4;
			stindMap["Int32&"] = OpCodes.Stind_I4;

			stindMap["IntPtr"] = OpCodes.Stind_I4;
			stindMap["Uint64&"] = OpCodes.Stind_I8;
			stindMap["Int64&"] = OpCodes.Stind_I8;
			stindMap["Float32&"] = OpCodes.Stind_R4;
			stindMap["Float64&"] = OpCodes.Stind_R8;
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
			// 		throw new NullReferenceException();

			Label skipThrow = IL.DefineLabel();

			IL.Emit(OpCodes.Dup);
			IL.Emit(OpCodes.Ldnull);
			IL.Emit(OpCodes.Bne_Un, skipThrow);

			IL.Emit(OpCodes.Newobj, notImplementedConstructor);
			IL.Emit(OpCodes.Throw);

			IL.MarkLabel(skipThrow);
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
			IL.Emit(OpCodes.Ldloc_1);
			IL.Emit(OpCodes.Callvirt, handlerMethod);

			SaveRefArguments(IL, parameters);
			PackageReturnType(method, IL);

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

				typeName = typeName.Replace("&", "");
				System.Type unboxedType = System.Type.GetType(typeName);

				IL.Emit(OpCodes.Unbox_Any, unboxedType);

				OpCode stind = GetStindInstruction(param.ParameterType);
				IL.Emit(stind);
			}
		}

		private static OpCode GetStindInstruction(System.Type parameterType)
		{
			if (parameterType.IsClass && !parameterType.Name.EndsWith("&"))
			{
				return OpCodes.Stind_Ref;
			}


			string typeName = parameterType.Name;

			if (!stindMap.ContainsKey(typeName) && parameterType.IsByRef)
			{
				return OpCodes.Stind_Ref;
			}

			Debug.Assert(stindMap.ContainsKey(typeName));
			OpCode result = stindMap[typeName];

			return result;
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