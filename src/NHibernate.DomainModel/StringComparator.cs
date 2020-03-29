using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class StringComparator : IComparer<string>
	{
		#region IComparer Members

		public int Compare(string x, string y)
		{
			if (x == null && y == null)
			{
				return 0;
			}

			if (x == null)
			{
				return -1;
			}

			return ((String) x).ToLower().CompareTo(((String) y).ToLower());
		}

		#endregion
	}
}
