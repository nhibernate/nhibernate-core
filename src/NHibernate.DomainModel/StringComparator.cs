using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public class StringComparator : IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			return ( (String) x ).ToLower().CompareTo( ( (String) y ).ToLower() );
		}

		#endregion
	}
}
