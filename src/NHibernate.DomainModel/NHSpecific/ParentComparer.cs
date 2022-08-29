using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for ParentComparer.
	/// </summary>
	[Serializable]
	public class ParentComparer : IComparer<Parent>
	{
		public ParentComparer()
		{
		}

		#region IComparer Members

		public int Compare(Parent x, Parent y)
		{
			if (x == null && y == null) return 0;
			//TODO: don't know if my logic is good, but good enough for compile
			if (x == null && y != null) return -1;
			if (x != null && y == null) return 1;
			return x.Id - y.Id;
		}

		#endregion
	}
}
