using System;
using System.Collections;

namespace NHibernate.Shards.Strategy.Exit
{
	public class ExitOperationUtils
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="results"></param>
		/// <returns></returns>
		public static IList GetNonNullList(IList results)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="results"></param>
		/// <param name="fromIndex"></param>
		/// <param name="toIndex"></param>
		/// <returns></returns>
		public static IList GetSubList(IList results, int fromIndex, int toIndex)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Compare the properties of 2 objects. The name of the property must be provided.
		/// </summary>
		public class PropertyComparer : System.Collections.Generic.IComparer<object>, IComparer
		{
			private readonly string propertyName;

			public PropertyComparer(string propertyName)
			{
				this.propertyName = propertyName;
			}

			///<summary>
			///Compares two properties of objects and returns a value indicating whether one is less than, equal to, or greater than the other.
			///</summary>
			///
			///<returns>
			///Value Condition Less than zero x is less than y. Zero x equals y. Greater than zero x is greater than y. 
			///</returns>
			///
			///<param name="y">The second object to compare. </param>
			///<param name="x">The first object to compare. </param>
			///<exception cref="T:System.ArgumentException">Neither x nor y implements the <see cref="T:System.IComparable"></see> interface.-or- x and y are of different types and neither one can handle comparisons with the other. </exception><filterpriority>2</filterpriority>
			public int Compare(object x, object y)
			{
				if (x.Equals(y)) return 0;

				IComparable xValue = GetPropertyValue(x, propertyName);
				IComparable yValue = GetPropertyValue(y, propertyName);

				if (xValue == null) return -1;

				return xValue.CompareTo(yValue);
			}
		}


		public static IComparable GetPropertyValue(object obj, string propertyName)
		{
			throw new NotImplementedException();
		}
	}
}