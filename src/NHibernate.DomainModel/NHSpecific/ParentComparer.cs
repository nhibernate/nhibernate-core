using System;
using System.Collections;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for ParentComparer.
	/// </summary>
	public class ParentComparer : IComparer {

		public ParentComparer() {
			
		}

		#region IComparer Members

		public int Compare(Parent x, Parent y) {
			if(x==null && y==null) return 0;
			//TODO: don't know if my logic is good, but good enough for compile
			if(x==null && y!=null) return -1;
			if(x!=null && y==null) return 1;
			return x.Id - y.Id;
		}

		int IComparer.Compare(object x, object y) {
			Parent xParent = x as Parent;
			Parent yParent = y as Parent;
			
			return Compare(xParent, yParent);
			
		}

		#endregion
	}
}
