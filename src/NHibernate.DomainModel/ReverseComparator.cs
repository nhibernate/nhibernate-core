using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class ReverseComparator : IComparer<string>
	{
		#region IComparer Members

		public int Compare(string x, string y)
		{
			return -(x).CompareTo(y);
		}

		#endregion
	}
}
