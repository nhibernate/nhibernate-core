using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for BasicNameable.
	/// </summary>
	public class BasicNameable
	{
		private string _name;
		private long _id;

		public string Name 
		{
			get { return _name; }
			set { _name = value; }
		}

		public long Key 
		{
			get { return _id; }
			set { _id = value; }
		}

	}
}
