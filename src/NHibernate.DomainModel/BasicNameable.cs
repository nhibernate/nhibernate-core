using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for BasicNameable.
	/// </summary>
	public class BasicNameable : INameable
	{
		private string _name;
		private long _id;

		#region INameable Members
		
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

		#endregion
	}
}
