using System;
using System.Collections;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for W.
	/// </summary>
	public class W 
	{
		private long _id;
		// <set> mapping
		private Iesi.Collections.ISet _zeds;
		
		public virtual long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual Iesi.Collections.ISet Zeds
		{
			get { return _zeds; }
			set { _zeds = value; }
		}

	}
}
