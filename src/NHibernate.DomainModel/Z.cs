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
		
		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public W W
		{
			get { return _w; }
			set { _w = value; }
		}

	}
}
