using System;
using System.Collections;

namespace NHibernate.Util {
	
	public sealed class ArrayHelper {
		private ArrayHelper() {}
		
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
	}
}
