using System;
using System.Collections;

namespace NHibernate.DomainModel
{
	public class StringComparator : IComparer
	{
		#region IComparer Members

		public int Compare(object x, object y)
		{
			if(x==null && y==null) 
			{
				return 0;
			}
			
			if(x==null) 
			{
				return -1;
			}


			return ( (String) x ).ToLower().CompareTo( ( (String) y ).ToLower() );
		}

		#endregion
	}
}
