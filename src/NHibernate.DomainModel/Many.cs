using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Many.
	/// </summary>
	public class Many
	{
		private long _key;
		private One _one;
		private int _x;
		private int _v;

		public long Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public One One
		{
			get { return _one; }
			set { _one = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}

		public int V
		{
			get { return _v; }
			set { _v = value; }
		}
	}
}