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
		private IDictionary _zeds;
		
		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public IDictionary Zeds
		{
			get { return _zeds; }
			set { _zeds = value; }
		}

	}
}
