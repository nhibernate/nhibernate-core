using System;

namespace NHibernate.Sql {
	/// <summary>
	/// Generic SQL types
	/// </summary>
	/// <remarks>We have to adjust this enum and its implementations in Dialect, making NHibernate.Type class</remarks>
	public enum Types {
		Array,
		BigInt,
		Binary,
		Bit,
		Blob,
		Char,
		Clob,
		Date,
		Decimal,
		Double,
		Float,
		Integer,
		LongVarBinary,
		LongVarChar,
		Null,
		Numeric,
		Real,
		SmallInt,
		Struct,
		Time,
		TimeStamp,
		TinyInt,
		VarBinary,
		VarChar
	}
}
