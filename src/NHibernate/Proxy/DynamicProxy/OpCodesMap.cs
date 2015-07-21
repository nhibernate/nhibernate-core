using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace NHibernate.Proxy.DynamicProxy
{
	public static class OpCodesMap
	{
		private static readonly Dictionary<System.Type, OpCode> LdindMap = new Dictionary<System.Type, OpCode>
		                                                                   {
		                                                                   	{typeof (Boolean), OpCodes.Ldind_I1},
		                                                                   	{typeof (SByte), OpCodes.Ldind_I1},
		                                                                   	{typeof (Byte), OpCodes.Ldind_U1},
		                                                                   	{typeof (Char), OpCodes.Ldind_I2},
		                                                                   	{typeof (Int16), OpCodes.Ldind_I2},
		                                                                   	{typeof (Int32), OpCodes.Ldind_I4},
		                                                                   	{typeof (Int64), OpCodes.Ldind_I8},
		                                                                   	{typeof (UInt16), OpCodes.Ldind_U2},
		                                                                   	{typeof (UInt32), OpCodes.Ldind_U4},
		                                                                   	{typeof (UInt64), OpCodes.Ldind_I8},
		                                                                   	{typeof (Single), OpCodes.Ldind_R4},
		                                                                   	{typeof (Double), OpCodes.Ldind_R8},
		                                                                   };
		private static readonly Dictionary<System.Type, OpCode> StindMap = new Dictionary<System.Type, OpCode>
		                                                                   {
		                                                                   	{typeof (Boolean), OpCodes.Stind_I1},
		                                                                   	{typeof (SByte), OpCodes.Stind_I1},
		                                                                   	{typeof (Byte), OpCodes.Stind_I1},
		                                                                   	{typeof (Char), OpCodes.Stind_I2},
		                                                                   	{typeof (Int16), OpCodes.Stind_I2},
		                                                                   	{typeof (Int32), OpCodes.Stind_I4},
		                                                                   	{typeof (Int64), OpCodes.Stind_I8},
		                                                                   	{typeof (UInt16), OpCodes.Stind_I2},
		                                                                   	{typeof (UInt32), OpCodes.Stind_I4},
		                                                                   	{typeof (UInt64), OpCodes.Stind_I8},
		                                                                   	{typeof (Single), OpCodes.Stind_R4},
		                                                                   	{typeof (Double), OpCodes.Stind_R8},
		                                                                   };
		
		public static bool TryGetLdindOpCode(System.Type valueType, out OpCode opCode)
		{
			return LdindMap.TryGetValue(valueType, out opCode);
		}

		public static bool TryGetStindOpCode(System.Type valueType, out OpCode opCode)
		{
			return StindMap.TryGetValue(valueType, out opCode);
		}
	}
}