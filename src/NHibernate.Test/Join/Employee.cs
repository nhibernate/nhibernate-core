using System;

namespace NHibernate.Test.Join
{
	public class Employee : Person
	{
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

	}
}
