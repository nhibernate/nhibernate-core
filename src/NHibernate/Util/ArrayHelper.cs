using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
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
		public static readonly int[] EmptyIntArray = new int[0];
		public static readonly bool[] EmptyBoolArray = new bool[0];

		public static readonly bool[] True = new bool[] { true };
		public static readonly bool[] False = new bool[] { false };

		public static bool IsAllNegative(int[] array)
		{
			return array.All(t => t < 0);
		}

		public static T[] Fill<T>(T value, int length)
		{
			var result = new T[length];
			Fill(result, value);
			return result;
		}

		public static void Fill<T>(T[] array, T value)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = value;
			}
		}

		public static T[] Slice<T>(T[] strings, int begin, int length)
		{
			var result = new T[length];
			Array.Copy(strings, begin, result, 0, length);
			return result;
		}

		public static T[] Join<T>(T[] x, T[] y, bool[] use)
		{
			var l = new List<T>(x);
			for (int i = 0; i < y.Length; i++)
			{
				if (use[i])
					l.Add(y[i]);
			}
			return l.ToArray();
		}

		public static T[] Join<T>(T[] x, T[] y)
		{
			var result = new T[x.Length + y.Length];
			Array.Copy(x, 0, result, 0, x.Length);
			Array.Copy(y, 0, result, x.Length, y.Length);
			return result;
		}

		public static bool IsAllFalse(bool[] array)
		{
			return Array.IndexOf(array, true) < 0;
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


		/// <summary>
		/// Append all elements in the 'from' list to the 'to' list.
		/// </summary>
		/// <param name="to"></param>
		/// <param name="from"></param>
		public static void AddAll(IList to, IList from)
		{
			System.Action addNull = null;
			foreach (object obj in from)
			{
				// There is bug in .NET, before version 4, where adding null to a List<Nullable<T>> through the non-generic IList interface throws an exception.
				// TODO: Everything but the to.Add(obj) should be conditionally compiled only for versions of .NET earlier than 4.
				if (obj == null)
				{
					if (addNull == null)
					{
						var toType = to.GetType();
						if (toType.IsGenericType &&
							toType.GetGenericTypeDefinition() == typeof(List<>) &&
							toType.GetGenericArguments()[0].IsNullable())
						{
							MethodInfo addMethod = toType.GetMethod("Add");
							var addMethodCall =	System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Constant(to),
																		addMethod,
																		System.Linq.Expressions.Expression.Constant(null, toType.GetGenericArguments()[0]));
							System.Linq.Expressions.LambdaExpression addLambda =
								System.Linq.Expressions.Expression.Lambda(addMethodCall);

							addNull = (System.Action)addLambda.Compile();
						}
						else
						{
							addNull = () => to.Add(null);
						}
					}
					addNull();
				}
				else
				{
					to.Add(obj);
				}
			}
		}

		// NH-specific
		public static void AddAll<T>(IList<T> to, IList<T> from)
		{
			foreach (T obj in from)
				to.Add(obj);
		}

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

		public static int CountTrue(bool[] array)
		{
			return array.Count(t => t);
		}

		public static bool ArrayEquals<T>(T[] a, T[] b)
		{
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			if (a.Length != b.Length)
				return false;

			for (int i = 0; i < a.Length; i++)
			{
				if (!Equals(a[i], b[i]))
					return false;
			}

			return true;
		}

		public static bool ArrayEquals(byte[] a, byte[] b)
		{
			if (a == b)
				return true;

			if (a == null || b == null)
				return false;

			if (a.Length != b.Length)
				return false;

			int i = 0;
			int len = a.Length;
			while (i < len)
			{
				if (a[i] != b[i])
					return false;

				i++;
			}
			return true;
		}

		/// <summary>
		/// Calculate a hash code based on the length and contents of the array.
		/// The algorithm is such that if ArrayHelper.ArrayEquals(a,b) returns true,
		/// then ArrayGetHashCode(a) == ArrayGetHashCode(b).
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <returns></returns>
		public static int ArrayGetHashCode<T>(T[] array)
		{
			int hc = array.Length;

			foreach (var e in array)
				hc = unchecked(hc*31 + e.GetHashCode());

			return hc;
		}
	}
}
