using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	public sealed class ReverseSortComparer<T> : IComparer<T> where T : IComparable<T>
	{
		#region IComparer<T> Members

		public int Compare(T x, T y)
		{
			return y.CompareTo(x);
		}

		#endregion
	}
}