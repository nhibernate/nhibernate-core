using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public class ReverseComparator : IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			return - ((IComparable)x).CompareTo(y);
		}

		#endregion
	}
}







