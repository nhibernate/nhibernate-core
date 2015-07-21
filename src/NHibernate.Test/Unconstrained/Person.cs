using System;

namespace NHibernate.Test.Unconstrained
{
	public class Person
	{
		private string _name;
		private string _employeeId;
		private Employee _employee;
		// Just a property that can be changed to force an update of the object.
		private int _unrelated;

		public Person()
		{
		}

		public Person(string name) : this()
		{
			_name = name;
		}

		public virtual string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public virtual Employee Employee
		{
			get { return _employee; }
			set { _employee = value; }
		}

		public virtual string EmployeeId
		{
			get { return _employeeId; }
			set { _employeeId = value; }
		}

		public virtual int Unrelated
		{
			get { return _unrelated; }
			set { _unrelated = value; }
		}
	}
}