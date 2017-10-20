using System.Reflection.Emit;
using System.Collections.Generic;

namespace NHibernate.Bytecode
{
	internal static class EmitUtil
	{
		public static void EmitBoxIfNeeded(ILGenerator il, System.Type type)
		{
			if (type.IsValueType)
			{
				il.Emit(OpCodes.Box, type);
			}
		}

		private static readonly Dictionary<System.Type, OpCode> typeToOpcode;

		static EmitUtil()
		{
			typeToOpcode = new Dictionary<System.Type, OpCode>
			{
				{typeof(bool), OpCodes.Ldind_I1},
				{typeof(sbyte), OpCodes.Ldind_I1},
				{typeof(byte), OpCodes.Ldind_U1},
				{typeof(char), OpCodes.Ldind_U2},
				{typeof(short), OpCodes.Ldind_I2},
				{typeof(ushort), OpCodes.Ldind_U2},
				{typeof(int), OpCodes.Ldind_I4},
				{typeof(uint), OpCodes.Ldind_U4},
				{typeof(long), OpCodes.Ldind_I8},
				{typeof(ulong), OpCodes.Ldind_I8},
				{typeof(float), OpCodes.Ldind_R4},
				{typeof(double), OpCodes.Ldind_R8},
			};
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
	}
}