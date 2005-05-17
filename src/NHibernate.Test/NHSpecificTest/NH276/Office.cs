using System;

namespace NHibernate.Test.NHSpecificTest.NH276
{
	public class Office
	{
		private int _id;
		private string _worker;
		private Building _location;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Worker
		{
			get { return _worker; }
			set { _worker = value; }
		}

		public Building Location
		{
			get { return _location; }
			set { _location = value; }
		}

	}
}
