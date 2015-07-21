using System;
using System.Collections.Generic;

namespace NHibernate.Test.Join
{
	public class Employee : Person
	{
		public Employee()
		{
			Meetings = new List<Meeting>();
		}
		private string _Title;
		public virtual string Title
		{
			get { return _Title; }
			set { _Title = value; }
		}

		private Employee _Manager;
		public virtual Employee Manager
		{
			get { return _Manager; }
			set { _Manager = value; }
		}

		private decimal _Salary;
		public virtual decimal Salary
		{
			get { return _Salary; }
			set { _Salary = value; }
		}

		public virtual IList<Meeting> Meetings { get; set; }
	}

	public class Meeting
	{
		public virtual int Id { get; set; }
		public virtual Employee Employee { get; set; }
		public virtual string Description { get; set; }
	}
}
