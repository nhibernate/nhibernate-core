using System;
using System.Data;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NHibernate.Util;

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
		private static readonly IDictionary<string, SqlType> SqlTypes = 
			new ThreadSafeDictionary<string, SqlType>(new Dictionary<string, SqlType>(128));

		public static readonly SqlType Guid = new SqlType(DbType.Guid);
		public static readonly SqlType Boolean = new SqlType(DbType.Boolean);
		public static readonly SqlType Byte = new SqlType(DbType.Byte);
		public static readonly SqlType Currency = new SqlType(DbType.Currency);
		public static readonly SqlType Date = new SqlType(DbType.Date);
		public static readonly SqlType DateTime = new SqlType(DbType.DateTime);
		public static readonly SqlType DateTime2 = new SqlType(DbType.DateTime2);
		public static readonly SqlType DateTimeOffSet = new SqlType(DbType.DateTimeOffset);
		public static readonly SqlType Decimal = new SqlType(DbType.Decimal);
		public static readonly SqlType Double = new SqlType(DbType.Double);
		public static readonly SqlType Int16 = new SqlType(DbType.Int16);
		public static readonly SqlType Int32 = new SqlType(DbType.Int32);
		public static readonly SqlType Int64 = new SqlType(DbType.Int64);
		public static readonly SqlType SByte = new SqlType(DbType.SByte);
		public static readonly SqlType Single = new SqlType(DbType.Single);
		public static readonly SqlType Time = new SqlType(DbType.Time);
		public static readonly SqlType UInt16 = new SqlType(DbType.UInt16);
		public static readonly SqlType UInt32 = new SqlType(DbType.UInt32);
		public static readonly SqlType UInt64 = new SqlType(DbType.UInt64);

		public static readonly SqlType[] NoTypes = new SqlType[0];

		private delegate SqlType TypeWithLenCreateDelegate(int length); // Func<int, T>

		private static T GetTypeWithLen<T>(int length, TypeWithLenCreateDelegate createDelegate) where T : SqlType
		{
			string key = GetKeyForLengthBased(typeof (T).Name, length);
			SqlType result;
			if (!SqlTypes.TryGetValue(key, out result))
			{
				lock(SqlTypes)
				{
					if (!SqlTypes.TryGetValue(key, out result))
					{
						result = createDelegate(length);
						SqlTypes.Add(key, result);
					}
				}
			}
			return (T) result;
		}

		private static SqlType GetTypeWithPrecision(DbType dbType, byte precision, byte scale)
		{
			string key = GetKeyForPrecisionScaleBased(dbType.ToString(), precision, scale);
			SqlType result;
			if (!SqlTypes.TryGetValue(key, out result))
			{
				result = new SqlType(dbType, precision, scale);
				SqlTypes.Add(key, result);
			}
			return result;
		}

		public static AnsiStringSqlType GetAnsiString(int length)
		{
			return GetTypeWithLen<AnsiStringSqlType>(length, l => new AnsiStringSqlType(l));
		}

		public static BinarySqlType GetBinary(int length)
		{
			return GetTypeWithLen<BinarySqlType>(length, l => new BinarySqlType(l));
		}

		public static BinaryBlobSqlType GetBinaryBlob(int length)
		{
			return GetTypeWithLen<BinaryBlobSqlType>(length, l => new BinaryBlobSqlType(l));
		}

		public static StringSqlType GetString(int length)
		{
			return GetTypeWithLen<StringSqlType>(length, l => new StringSqlType(l));
		}

		public static StringClobSqlType GetStringClob(int length)
		{
			return GetTypeWithLen<StringClobSqlType>(length, l => new StringClobSqlType(l));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static SqlType GetSqlType(DbType dbType, byte precision, byte scale)
		{
			return GetTypeWithPrecision(dbType, precision, scale);
		}

		private static string GetKeyForLengthBased(string name, int length)
		{
			return name + "(" + length + ")";
		}

		private static string GetKeyForPrecisionScaleBased(string name, byte precision, byte scale)
		{
			return name + "(" + precision + ", " + scale + ")";
		}
	}
}