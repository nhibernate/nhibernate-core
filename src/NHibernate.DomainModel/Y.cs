using System;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for Y.
	/// </summary>
	public class Y 
	{
		private long _id;
		private string _x;
		private X _theX;
		
		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string X
		{
			get { return _x; }
			set { _x = value; }
		}

		public X TheX
		{
			get { return _theX; }
			set { _theX = value; }
		}

	}
}
