using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for SubDetail.
	/// </summary>
	public class SubDetail
	{
		private string _name;
		private long _id;

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}