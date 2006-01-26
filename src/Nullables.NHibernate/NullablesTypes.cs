using System;

using NHibernate.Type;

namespace Nullables.NHibernate
{
	/// <summary>
	/// Summary description for NullablesTypes.
	/// </summary>
	public sealed class NullablesTypes
	{
		/// <summary>
		/// Nullables.NHibernate.NullableBoolean type
		/// </summary>
		public static readonly NullableType NullableBoolean = new NullableBooleanType();
	
		/// <summary>
		/// Nullables.NHibernate.NullableByte type
		/// </summary>
		public static readonly NullableType NullableByte = new NullableByteType();
	
		/// <summary>
		/// Nullables.NHibernate.NullableDouble type
		/// </summary>
		public static readonly NullableType NullableDouble = new NullableDoubleType();
	
		/// <summary>
		/// Nullables.NHibernate.NullableInt16 type
		/// </summary>
		public static readonly NullableType NullableInt16 = new NullableInt16Type();
	
		/// <summary>
		/// Nullables.NHibernate.NullableInt32 type
		/// </summary>
		public static readonly NullableType NullableInt32 = new NullableInt32Type();
	
		/// <summary>
		/// Nullables.NHibernate.NullableInt64 type
		/// </summary>
		public static readonly NullableType NullableInt64 = new NullableInt64Type();
	
		/// <summary>
		/// Nullables.NHibernate.NullableDecimal type
		/// </summary>
		public static readonly NullableType NullableDecimal = new NullableDecimalType();
	
		/// <summary>
		/// Nullables.NHibernate.NullableDateTime type
		/// </summary>
		public static readonly NullableType NullableDateTime = new NullableDateTimeType();

//		/// <summary>
//		/// Nullables.NHibernate.NullableSByte type
//		/// </summary>
//		public static readonly NullableType NullableSByte = new NullableSByteType();
	
		/// <summary>
		/// Nullables.NHibernate.NullableSingle type
		/// </summary>
		public static readonly NullableType NullableSingle = new NullableSingleType();
	
		/// <summary>
		/// Nullables.NHibernate.NullableGuid type
		/// </summary>
		public static readonly NullableType NullableGuid = new NullableGuidType();
	}
}
