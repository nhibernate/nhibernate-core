using System;
using System.Data;
using System.Collections;

using NHibernate.SqlTypes;

namespace NHibernate.Util {
	
	public sealed class ArrayHelper {
		private ArrayHelper() {}
		
		/// <summary>
		/// Compares two byte[] arrays for equality.
		/// </summary>
		/// <param name="lhs">The byte[] array on the Left Hand Side </param>
		/// <param name="rhs">The byte[] array on the Right Hand Side</param>
		/// <returns>true if they contain the same items</returns>
		public static bool Equals(byte[] lhs, byte[] rhs) 
		{
			// just for luck, check for reference equality
			if(lhs==rhs) return true;

			// if they don't have the same reference and one of them
			// is null, then they are not Equal
			if(lhs==null || rhs==null) return false;

			// if they don't have the same length they are not equal
			if(lhs.Length != rhs.Length) return false;

			// move through every object in the array and hope that it 
			// implements the Equals method correctly
			for(int i = 0; i < lhs.Length; i++)
			{
				if(lhs[i].Equals(rhs[i])==false) return false;
			}

			return true;
		}

		/// <summary>
		/// Compares two object arrays for equality.
		/// </summary>
		/// <param name="lhs">The object array on the Left Hand Side </param>
		/// <param name="rhs">The object array on the Right Hand Side</param>
		/// <returns>true if they contain the same items</returns>
		/// <remarks>
		/// This relies on the objects in the array implementing the method Equals(object)
		/// correctly.
		/// </remarks>
		public static bool Equals(Array lhs, Array rhs) 
		{
			// just for luck, check for reference equality
			if(lhs==rhs) return true;

			// if they don't have the same reference and one of them
			// is null, then they are not Equal
			if(lhs==null || rhs==null) return false;

			// if they don't have the same length they are not equal
			if(lhs.Length != rhs.Length) return false;

			// move through every object in the array and hope that it 
			// implements the Equals method correctly
			for(int i = 0; i < lhs.Length; i++)
			{
				if(lhs.GetValue(i).Equals(rhs.GetValue(i))==false) return false;
			}

			return true;

		}

		public static string[] ToStringArray(object[] objects) {
			int length = objects.Length;
			string[] result = new string[length];
			for (int i=0; i<length; i++) {
				result[i] = objects[i].ToString();
			}
			return result;
		}

		public static string[] FillArray(string str, int length) {
			string[] result = new string[length];
			for (int i=0; i<length; i++) {
				result[i] = str;
			}
			return result;
		}

		public static string[] ToStringArray(ICollection coll) {
			string[] result = new string[coll.Count];
			int i=0;
			foreach(object obj in coll) {
				result[i++] = obj.ToString();
			}
			return result;
		}
		public static int[] ToIntArray(ICollection coll) {
			int[] result = new int[coll.Count];
			int i=0;
			foreach(object obj in coll) {
				result[i++] = int.Parse(obj.ToString());
			}
			return result;
		}

		public static string[] Slice(string[] strings, int begin, int length) {
			string[] result = new string[length];
			for (int i=0; i<length; i++) {
				result[i] = strings[begin+i];
			}
			return result;
		}

		public static object[] Slice(object[] objects, int begin, int length) {
			object[] result = new object[length];
			for (int i=0; i<length; i++) {
				result[i] = objects[begin+i];
			}
			return result;
		}

		public static string[] Join(string[] x, string[] y) {
			string[] result = new string[x.Length + y.Length];
			for(int i=0; i<x.Length; i++)
				result[i] = x[i];
			for(int i=0; i<y.Length; i++)
				result[i+x.Length] = y[i];
			return result;
		}

		public static DbType[] Join(DbType[] x, DbType[] y) {
			DbType[] result = new DbType[ x.Length + y.Length ];
			for ( int i=0; i<x.Length; i++ ) result[i] = x[i];
			for ( int i=0; i<y.Length; i++ ) result[i+x.Length] = y[i];
			return result;		
		}

		public static SqlType[] Join(SqlType[] x, SqlType[] y) {
			SqlType[] result = new SqlType[ x.Length + y.Length ];
			for ( int i=0; i<x.Length; i++ ) result[i] = x[i];
			for ( int i=0; i<y.Length; i++ ) result[i+x.Length] = y[i];
			return result;		
		}
	}
}
