using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace NHibernate.Bytecode
{
	public class EmitUtil
	{
		private EmitUtil()
		{
		}

		/// <summary>
		/// Emits an <c>ldc.i4</c> opcode using the fastest available opcode choice.
		/// </summary>
		public static void EmitFastInt(ILGenerator il, int value)
		{
			switch (value)
			{
				case -1:
					il.Emit(OpCodes.Ldc_I4_M1);
					return;
				case 0:
					il.Emit(OpCodes.Ldc_I4_0);
					return;
				case 1:
					il.Emit(OpCodes.Ldc_I4_1);
					return;
				case 2:
					il.Emit(OpCodes.Ldc_I4_2);
					return;
				case 3:
					il.Emit(OpCodes.Ldc_I4_3);
					return;
				case 4:
					il.Emit(OpCodes.Ldc_I4_4);
					return;
				case 5:
					il.Emit(OpCodes.Ldc_I4_5);
					return;
				case 6:
					il.Emit(OpCodes.Ldc_I4_6);
					return;
				case 7:
					il.Emit(OpCodes.Ldc_I4_7);
					return;
				case 8:
					il.Emit(OpCodes.Ldc_I4_8);
					return;
			}

			if (value > -129 && value < 128)
			{
				il.Emit(OpCodes.Ldc_I4_S, (SByte) value);
			}
			else
			{
				il.Emit(OpCodes.Ldc_I4, value);
			}
		}

		public static void EmitBoxIfNeeded(ILGenerator il, System.Type type)
		{
			if (type.IsValueType)
			{
				il.Emit(OpCodes.Box, type);
			}
		}

		private static Dictionary<System.Type, OpCode> typeToOpcode;

		static EmitUtil()
		{
			typeToOpcode = new Dictionary<System.Type, OpCode>(12);

			typeToOpcode[typeof(bool)] = OpCodes.Ldind_I1;
			typeToOpcode[typeof(sbyte)] = OpCodes.Ldind_I1;
			typeToOpcode[typeof(byte)] = OpCodes.Ldind_U1;

			typeToOpcode[typeof(char)] = OpCodes.Ldind_U2;
			typeToOpcode[typeof(short)] = OpCodes.Ldind_I2;
			typeToOpcode[typeof(ushort)] = OpCodes.Ldind_U2;

			typeToOpcode[typeof(int)] = OpCodes.Ldind_I4;
			typeToOpcode[typeof(uint)] = OpCodes.Ldind_U4;

			typeToOpcode[typeof(long)] = OpCodes.Ldind_I8;
			typeToOpcode[typeof(ulong)] = OpCodes.Ldind_I8;

			typeToOpcode[typeof(float)] = OpCodes.Ldind_R4;

			typeToOpcode[typeof(double)] = OpCodes.Ldind_R8;
		}

		/// <summary>
		/// Emits IL to unbox a value type and if null, create a new instance of the value type.
		/// </summary>
		/// <remarks>
		/// This does not work if the value type doesn't have a default constructor - we delegate
		/// that to the ISetter.
		/// </remarks>
		public static void PreparePropertyForSet(ILGenerator il, System.Type propertyType)
		{
			// If this is a value type, we need to unbox it
			if (propertyType.IsValueType)
			{
				// if (object[i] == null), create a new instance 
				Label notNullLabel = il.DefineLabel();
				Label nullDoneLabel = il.DefineLabel();
				LocalBuilder localNew = il.DeclareLocal(propertyType);

				il.Emit(OpCodes.Dup);
				il.Emit(OpCodes.Brtrue_S, notNullLabel);

				il.Emit(OpCodes.Pop);
				il.Emit(OpCodes.Ldloca, localNew);
				il.Emit(OpCodes.Initobj, propertyType);
				il.Emit(OpCodes.Ldloc, localNew);
				il.Emit(OpCodes.Br_S, nullDoneLabel);

				il.MarkLabel(notNullLabel);

				il.Emit(OpCodes.Unbox, propertyType);

				// Load the value indirectly, using ldobj or a specific opcode
				OpCode specificOpCode;
				if (typeToOpcode.TryGetValue(propertyType, out specificOpCode))
				{
					il.Emit(specificOpCode);
				}
				else
				{
					il.Emit(OpCodes.Ldobj, propertyType);
				}

				il.MarkLabel(nullDoneLabel);
			}
			else
			{
				if (propertyType != typeof(object))
				{
					il.Emit(OpCodes.Castclass, propertyType);
				}
			}
		}

		/// <summary>
		/// Defines a new delegate type.
		/// </summary>
		public static System.Type DefineDelegateType(
			string fullTypeName,
			ModuleBuilder moduleBuilder,
			System.Type returnType,
			System.Type[] parameterTypes)
		{
			TypeBuilder delegateBuilder =
				moduleBuilder.DefineType(
					fullTypeName,
					TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass |
					TypeAttributes.AutoClass, typeof(MulticastDelegate));

			// Define a special constructor
			ConstructorBuilder constructorBuilder =
				delegateBuilder.DefineConstructor(
					MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
					CallingConventions.Standard, new System.Type[] {typeof(object), typeof(IntPtr)});

			constructorBuilder.SetImplementationFlags(
				MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

			// Define the Invoke method for the delegate

			MethodBuilder methodBuilder = delegateBuilder.DefineMethod("Invoke",
			                                                           MethodAttributes.Public | MethodAttributes.HideBySig
			                                                           | MethodAttributes.NewSlot | MethodAttributes.Virtual,
			                                                           returnType, parameterTypes);

			methodBuilder.SetImplementationFlags(
				MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

			return delegateBuilder.CreateType();
		}

		public static void EmitLoadType(ILGenerator il, System.Type type)
		{
			il.Emit(OpCodes.Ldtoken, type);
			il.Emit(OpCodes.Call, typeof(System.Type).GetMethod("GetTypeFromHandle"));
		}

		public static void EmitLoadMethodInfo(ILGenerator il, MethodInfo methodInfo)
		{
			il.Emit(OpCodes.Ldtoken, methodInfo);
			il.Emit(
				OpCodes.Call,
				typeof(MethodBase).GetMethod(
					"GetMethodFromHandle", new System.Type[] {typeof(RuntimeMethodHandle)}));
			il.Emit(OpCodes.Castclass, typeof(MethodInfo));
		}

		public static void EmitCreateDelegateInstance(ILGenerator il, System.Type delegateType, MethodInfo methodInfo)
		{
			MethodInfo createDelegate = typeof(Delegate).GetMethod(
				"CreateDelegate", BindingFlags.Static | BindingFlags.Public | BindingFlags.ExactBinding, null,
				new System.Type[] {typeof(System.Type), typeof(MethodInfo)}, null);

			EmitLoadType(il, delegateType);
			EmitLoadMethodInfo(il, methodInfo);
			il.EmitCall(OpCodes.Call, createDelegate, null);
			il.Emit(OpCodes.Castclass, delegateType);
		}
	}
}