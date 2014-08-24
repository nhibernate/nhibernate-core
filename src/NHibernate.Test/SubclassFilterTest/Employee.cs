using System;
using System.Collections.Generic;

namespace NHibernate.Test.SubclassFilterTest
{
	public class Employee : Person
	{
		private string title;
		private string department;
		private Employee manager;
		private ISet<Employee> minions = new HashSet<Employee>();

		public Employee()
		{
		}

		public Employee(string name) : base(name)
		{
		}

		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		public virtual string Department
		{
			get { return department; }
			set { department = value; }
		}

		public virtual Employee Manager
		{
			get { return manager; }
			set { manager = value; }
		}

		public virtual ISet<Employee> Minions
		{
			get { return minions; }
			set { minions = value; }
		}
	}
}