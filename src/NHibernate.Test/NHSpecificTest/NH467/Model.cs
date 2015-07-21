using System;

namespace NHibernate.Test.NHSpecificTest.NH467
{
	public class User
	{
		private int _id;
		private string _name;
		private short _isActive;

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual short IsActive
		{
			get { return _isActive; }
			set { _isActive = value; }
		}
	}

	public class Employee
	{
		private int _id;
		private User _user;

		public virtual int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public virtual User User
		{
			get { return _user; }
			set { _user = value; }
		}
	}
}