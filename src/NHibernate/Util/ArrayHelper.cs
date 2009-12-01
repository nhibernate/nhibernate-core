using System;
using System.Collections;
using System.Data;
using System.Text;
using NHibernate.SqlTypes;
using NHibernate.Type;
using System.Collections.Generic;

namespace NHibernate.Util
{
	/// <summary>
	/// Helper class that contains common array functions and 
	/// data structures used through out NHibernate.
	/// </summary>
	public static class ArrayHelper
	{
		public static readonly object[] EmptyObjectArray = new object[0];
		public static readonly IType[] EmptyTypeArray = new IType[0];
		public static readonly int[] EmptyIntArray = new int[0];
		public static readonly bool[] EmptyBoolArray = new bool[0];

		public static readonly bool[] True = new bool[] { true };
		public static readonly bool[] False = new bool[] { false };

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

		public static void Fill<T>(T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = value;
			}
		}

		public static int[] ToIntArray(ICollection coll)
		{
			return (int[]) ToArray(coll, typeof(int));
		}

		public static bool[] ToBooleanArray(ICollection col)
		{
			return (bool[])ToArray(col, typeof(bool));
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

		public static T[] Join<T>(T[] x, T[] y, bool[] use)
		{
			List<T> l = new List<T>(x);
			for (int i = 0; i < y.Length; i++)
			{
				if (use[i])
					l.Add(y[i]);
			}
			return l.ToArray();
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
			var result = new string[ coll.Count ][];
			int i = 0;
			foreach (object row in coll)
			{
				var rowAsCollection = row as ICollection;
				if (rowAsCollection != null)
				{
					result[i] = new string[rowAsCollection.Count];
					int j = 0;
					foreach (object cell in rowAsCollection)
					{
						result[i][j++] = cell == null ? null : (string) cell;
					}
				}
				else
				{
					result[i] = new string[1];
					result[i][0] = row == null ? null : (string) row;
				}
				i++;
			}

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
				// we want to override the values from to if they exists
				to[de.Key] = de.Value;
			}
		}

		// NH-specific
		public static void AddAll<TKey, TValue>(IDictionary<TKey, TValue> to, IDictionary<TKey, TValue> from)
		{
			foreach (KeyValuePair<TKey, TValue> de in from)
			{
				// we want to override the values from to if they exists
				to[de.Key] = de.Value;
			}
		}

		public static IDictionary<TKey, TValue> AddOrOverride<TKey, TValue>(this IDictionary<TKey, TValue> destination, IDictionary<TKey, TValue> sourceOverride)
		{
			foreach (KeyValuePair<TKey, TValue> de in sourceOverride)
			{
				// we want to override the values from to if they exists
				destination[de.Key] = de.Value;
			}
			return destination;
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

		public static string[] ToStringArray(ICollection<string> coll)
		{
			return new List<string>(coll).ToArray();
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

		public static bool ArrayEquals(SqlType[] a, SqlType[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}

			for(int i = 0; i < a.Length; i++)
			{
				if (!Equals(a[i], b[i]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
