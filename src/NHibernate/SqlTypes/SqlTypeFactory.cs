using System;
using System.Collections.Concurrent;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// SqlTypeFactory provides Singleton access to the SqlTypes.
	/// </summary>
	[Serializable]
	public static class SqlTypeFactory
	{
		// key = typeof(sqlType).Name : ie - BinarySqlType(l), BooleanSqlType, DecimalSqlType(p,s)
		// value = SqlType
		private static readonly ConcurrentDictionary<string, SqlType> SqlTypes =
			new ConcurrentDictionary<string, SqlType>(4 * Environment.ProcessorCount, 128);

		public static readonly SqlType Guid = new SqlType(DbType.Guid);
		public static readonly SqlType Boolean = new SqlType(DbType.Boolean);
		public static readonly SqlType Byte = new SqlType(DbType.Byte);
		public static readonly SqlType Currency = new SqlType(DbType.Currency);
		public static readonly SqlType Date = new SqlType(DbType.Date);
		public static readonly SqlType DateTime = new DateTimeSqlType();
		public static readonly SqlType DateTime2 = new DateTime2SqlType();
		public static readonly SqlType DateTimeOffSet = new DateTimeOffsetSqlType();
		public static readonly SqlType Decimal = new SqlType(DbType.Decimal);
		public static readonly SqlType Double = new SqlType(DbType.Double);
		public static readonly SqlType Int16 = new SqlType(DbType.Int16);
		public static readonly SqlType Int32 = new SqlType(DbType.Int32);
		public static readonly SqlType Int64 = new SqlType(DbType.Int64);
		public static readonly SqlType SByte = new SqlType(DbType.SByte);
		public static readonly SqlType Single = new SqlType(DbType.Single);
		public static readonly SqlType Time = new TimeSqlType();
		public static readonly SqlType UInt16 = new SqlType(DbType.UInt16);
		public static readonly SqlType UInt32 = new SqlType(DbType.UInt32);
		public static readonly SqlType UInt64 = new SqlType(DbType.UInt64);

		public static readonly SqlType[] NoTypes = Array.Empty<SqlType>();

		private delegate T TypeWithLenOrScaleCreateDelegate<out T, in TDim>(TDim lengthOrScale); // Func<int, T>

		private static T GetTypeWithLenOrScale<T, TDim>(TDim lengthOrScale, TypeWithLenOrScaleCreateDelegate<T, TDim> createDelegate) where T : SqlType
		{
			var key = GetKeyForLengthOrScaleBased(typeof(T).Name, lengthOrScale);
			var result = SqlTypes.GetOrAdd(key, k => createDelegate(lengthOrScale));
			return (T) result;
		}

		private static SqlType GetTypeWithPrecision(DbType dbType, byte precision, byte scale)
		{
			string key = GetKeyForPrecisionScaleBased(dbType.ToString(), precision, scale);
			SqlType result = SqlTypes.GetOrAdd(key, k => new SqlType(dbType, precision, scale));
			return result;
		}

		public static AnsiStringSqlType GetAnsiString(int length)
		{
			return GetTypeWithLenOrScale(length, l => new AnsiStringSqlType(l));
		}

		public static BinarySqlType GetBinary(int length)
		{
			return GetTypeWithLenOrScale(length, l => new BinarySqlType(l));
		}

		public static BinaryBlobSqlType GetBinaryBlob(int length)
		{
			return GetTypeWithLenOrScale(length, l => new BinaryBlobSqlType(l));
		}

		public static StringSqlType GetString(int length)
		{
			return GetTypeWithLenOrScale(length, l => new StringSqlType(l));
		}

		public static StringClobSqlType GetStringClob(int length)
		{
			return GetTypeWithLenOrScale(length, l => new StringClobSqlType(l));
		}

		public static DateTimeSqlType GetDateTime(byte fractionalSecondsPrecision)
		{
			return GetTypeWithLenOrScale(fractionalSecondsPrecision, l => new DateTimeSqlType(l));
		}

		public static DateTime2SqlType GetDateTime2(byte fractionalSecondsPrecision)
		{
			return GetTypeWithLenOrScale(fractionalSecondsPrecision, l => new DateTime2SqlType(l));
		}

		public static DateTimeOffsetSqlType GetDateTimeOffset(byte fractionalSecondsPrecision)
		{
			return GetTypeWithLenOrScale(fractionalSecondsPrecision, l => new DateTimeOffsetSqlType(l));
		}

		public static TimeSqlType GetTime(byte fractionalSecondsPrecision)
		{
			return GetTypeWithLenOrScale(fractionalSecondsPrecision, l => new TimeSqlType(l));
		}

		public static SqlType GetSqlType(DbType dbType, byte precision, byte scale)
		{
			return GetTypeWithPrecision(dbType, precision, scale);
		}

		private static string GetKeyForLengthOrScaleBased<T>(string name, T lengthOrScale)
		{
			return name + "(" + lengthOrScale + ")";
		}

		private static string GetKeyForPrecisionScaleBased(string name, byte precision, byte scale)
		{
			return name + "(" + precision + ", " + scale + ")";
		}
	}
}
