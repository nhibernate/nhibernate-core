using System;
using System.Collections;
using System.Data;

namespace NHibernate.SqlTypes
{
	/// <summary>
	/// SqlTypeFactory provides Singleton access to the SqlTypes.
	/// </summary>
	[Serializable]
	public sealed class SqlTypeFactory
	{
		// key = typeof(sqlType).Name : ie - BinarySqlType(l), BooleanSqlType, DecimalSqlType(p,s)
		// value = SqlType
		private static Hashtable sqlTypes = Hashtable.Synchronized(new Hashtable(41));

		private SqlTypeFactory()
		{
		}

		public static readonly SqlType Guid = new SqlType(DbType.Guid);

		public static readonly SqlType Boolean = new SqlType(DbType.Boolean);
		public static readonly SqlType Byte = new SqlType(DbType.Byte);
		public static readonly SqlType Currency = new SqlType(DbType.Currency);
		public static readonly SqlType Date = new SqlType(DbType.Date);
		public static readonly SqlType DateTime = new SqlType(DbType.DateTime);
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

		public static AnsiStringSqlType GetAnsiString(int length)
		{
			string key = GetKeyForLengthBased(typeof(AnsiStringSqlType).Name, length);
			AnsiStringSqlType returnSqlType = (AnsiStringSqlType) sqlTypes[key];
			if (returnSqlType == null)
			{
				returnSqlType = new AnsiStringSqlType(length);
				sqlTypes.Add(key, returnSqlType);
			}
			return returnSqlType;
		}

		public static BinarySqlType GetBinary(int length)
		{
			string key = GetKeyForLengthBased(typeof(BinarySqlType).Name, length);
			BinarySqlType returnSqlType = (BinarySqlType) sqlTypes[key];
			if (returnSqlType == null)
			{
				returnSqlType = new BinarySqlType(length);
				sqlTypes.Add(key, returnSqlType);
			}
			return returnSqlType;
		}

		public static SqlType GetDecimal(byte precision, byte scale)
		{
			return new SqlType(DbType.Decimal, precision, scale);
		}

		public static StringSqlType GetString(int length)
		{
			string key = GetKeyForLengthBased(typeof(StringSqlType).Name, length);

			StringSqlType returnSqlType = (StringSqlType) sqlTypes[key];
			if (returnSqlType == null)
			{
				returnSqlType = new StringSqlType(length);
				sqlTypes.Add(key, returnSqlType);
			}

			return returnSqlType;
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