using System;

namespace NHibernate.Test.JoinedSubclass
{
	/// <summary>
	/// Summary description for Person.
	/// </summary>
	public class Person
	{
		private int _id = 0;
		private string _name;
		private char _sex = 'M';
		private Address _address = new Address();

		public Person()
		{
		}

		public virtual int Id 
		{
			get { return _id; }
		}

		public virtual string Name 
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual char Sex 
		{
			get { return _sex; }
			set { _sex = value; }
		}

		public virtual Address Address 
		{
			get { return _address; }
			set { _address = value; }
		}
	}
}
