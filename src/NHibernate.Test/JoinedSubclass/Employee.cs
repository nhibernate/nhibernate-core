using System;

namespace NHibernate.Test.JoinedSubclass
{
	/// <summary>
	/// Summary description for Employee.
	/// </summary>
	public class Employee : Person
	{
		private string _title;
		private Decimal _salary;
		private Employee _manager;

		public Employee()
		{
		}

		public virtual string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public virtual Decimal Salary
		{
			get { return _salary; }
			set { _salary = value; }
		}

		public virtual Employee Manager
		{
			get { return _manager; }
			set { _manager = value; }
		}
	}
}