using System;
using System.Collections.Generic;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for W.
	/// </summary>
	public class W
	{
		private long _id;
		// <set> mapping

		public virtual long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual ISet<Z> Zeds { get; set; }
	}
}