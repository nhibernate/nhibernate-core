using System;

namespace NHibernate.Test.NHSpecificTest.NH266
{
	public class User
	{
		private int _id;
		private string _name;
		private short _isActive;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public short IsActive
		{
			get { return _isActive; }
			set { _isActive = value; }
		}
	}
}
