using System;
using System.Collections;
using System.Data;
using System.Text;
using NHibernate.SqlTypes;
using NHibernate.Type;

namespace NHibernate.Util
{
	/// <summary>
	/// Helper class that contains common array functions and 
	/// data structures used through out NHibernate.
	/// </summary>
	public sealed class ArrayHelper
	{
		public static readonly object[] EmptyObjectArray = new object[0];
		public static readonly IType[] EmptyTypeArray = new IType[0];
		public static readonly int[] EmptyIntArray = new int[0];
		public static readonly bool[] EmptyBoolArray = new bool[0];

		private ArrayHelper()
		{
		}

		public static bool IsAllNegative(int[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] >= 0)
				{
					return false;
				}
			}

			return true;
		}

		public static string[] ToStringArray(object[] objects)
		{
			return (string[]) ToArray(objects, typeof(string));
		}

		public static string[] FillArray(string str, int length)
		{
			string[] result = new string[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = str;
			}
			return result;
		}

		public static LockMode[] FillArray(LockMode lockMode, int length)
		{
			LockMode[] result = new LockMode[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = lockMode;
			}
			return result;
		}

		public static IType[] FillArray(IType type, int length)
		{
			IType[] result = new IType[length];
			for (int i = 0; i < length; i++)
			{
				result[i] = type;
			}
			return result;
		}

		public static int[] ToIntArray(ICollection coll)
		{
			return (int[]) ToArray(coll, typeof(int));
		}

		public static string[] Slice(string[] strings, int begin, int length)
		{
			string[] result = new string[length];
			Array.Copy(strings, begin, result, 0, length);
			return result;
		}

		public static object[] Slice(object[] objects, int begin, int length)
		{
			object[] result = new object[length];
			Array.Copy(objects, begin, result, 0, length);
			return result;
		}

		public static string[] Join(string[] x, string[] y)
		{
			string[] result = new string[x.Length + y.Length];
			Array.Copy(x, 0, result, 0, x.Length);
			Array.Copy(y, 0, result, x.Length, y.Length);
			return result;
		}

		public static DbType[] Join(DbType[] x, DbType[] y)
		{
			DbType[] result = new DbType[x.Length + y.Length];
			Array.Copy(x, 0, result, 0, x.Length);
			Array.Copy(y, 0, result, x.Length, y.Length);
			return result;
		}

		public static SqlType[] Join(SqlType[] x, SqlType[] y)
		{
			SqlType[] result = new SqlType[x.Length + y.Length];
			Array.Copy(x, 0, result, 0, x.Length);
			Array.Copy(y, 0, result, x.Length, y.Length);
			return result;
		}

		public static object[] Join(object[] x, object[] y)
		{
			object[] result = new object[x.Length + y.Length];
			Array.Copy(x, 0, result, 0, x.Length);
			Array.Copy(y, 0, result, x.Length, y.Length);
			return result;
		}

		public static bool IsAllFalse(bool[] array)
		{
			return Array.IndexOf(array, true) < 0;
		}

		public static string[][] To2DStringArray(ICollection coll)
		{
			string[][] result = new string[coll.Count][];
			coll.CopyTo(result, 0);
			return result;
		}

		public static string ToString(object[] array)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			for (int i = 0; i < array.Length; i++)
			{
				sb.Append(array[i]);
				if (i < array.Length - 1)
				{
					sb.Append(",");
				}
			}
			sb.Append("]");
			return sb.ToString();
		}

		// NH-specific
		public static void AddAll(IList to, IList from)
		{
			foreach (object obj in from)
			{
				to.Add(obj);
			}
		}

		// NH-specific
		public static void AddAll(IDictionary to, IDictionary from)
		{
			foreach (DictionaryEntry de in from)
			{
				to.Add(de.Key, de.Value);
			}
		}

		public static int[] GetBatchSizes(int maxBatchSize)
		{
			int batchSize = maxBatchSize;
			int n = 1;

			while (batchSize > 1)
			{
				batchSize = GetNextBatchSize(batchSize);
				n++;
			}

			int[] result = new int[n];
			batchSize = maxBatchSize;

			for (int i = 0; i < n; i++)
			{
				result[i] = batchSize;
				batchSize = GetNextBatchSize(batchSize);
			}

			return result;
		}

		private static int GetNextBatchSize(int batchSize)
		{
			if (batchSize <= 10)
			{
				return batchSize - 1; // allow 9,8,7,6,5,4,3,2,1
			}
			else if (batchSize / 2 < 10)
			{
				return 10;
			}
			else
			{
				return batchSize / 2;
			}
		}

		public static IType[] ToTypeArray(IList list)
		{
			IType[] result = new IType[list.Count];
			list.CopyTo(result, 0);
			return result;
		}

		private static void ExpandWithNulls(IList list, int requiredLength)
		{
			while (list.Count < requiredLength)
			{
				list.Add(null);
			}
		}

		/// <summary>
		/// Sets <paramref name="list" /> item at position <paramref name="index" /> to <paramref name="value" />.
		/// Expands the list by adding <see langword="null" /> values, if needed.
		/// </summary>
		public static void SafeSetValue(IList list, int index, object value)
		{
			ExpandWithNulls(list, index + 1);
			list[index] = value;
		}

		public static string[] ToStringArray(ICollection coll)
		{
			return (string[]) ToArray(coll, typeof(string));
		}

		public static SqlType[] ToSqlTypeArray(ICollection coll)
		{
			return (SqlType[]) ToArray(coll, typeof(SqlType));
		}

		public static Array ToArray(ICollection coll, System.Type elementType)
		{
			Array result = Array.CreateInstance(elementType, coll.Count);
			coll.CopyTo(result, 0);
			return result;
		}

		public static int CountTrue(bool[] array)
		{
			int result = 0;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i]) result++;
			}
			return result;
		}
	}
}