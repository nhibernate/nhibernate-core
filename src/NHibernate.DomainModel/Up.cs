using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Up.
	/// </summary>
	public class Up
	{
		private string id1;
		private long id2;

		public Up()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public string Id1
		{
			get { return id1; }
			set { id1 = value; }
		}

		public long Id2
		{
			get { return id2; }
			set { id2 = value; }
		}

		public int GetHashcode()
		{
			return id1.GetHashCode();
		}
	}
}
