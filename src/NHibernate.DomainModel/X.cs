using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for X.
	/// </summary>
	public class X
	{
		private long _id;
		private Y _y;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public Y Y
		{
			get { return _y; }
			set { _y = value; }
		}
	}
}