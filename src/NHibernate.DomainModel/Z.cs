using System;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Z.
	/// </summary>
	public class Z 
	{
		private long _id;
		private W _w;
		
		public virtual long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual W W
		{
			get { return _w; }
			set { _w = value; }
		}

	}
}
