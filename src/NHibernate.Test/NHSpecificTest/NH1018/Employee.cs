using System;

namespace NHibernate.Test.NHSpecificTest.NH1018
{
	public class Employee
	{
		private int id;
		private string name;
		private Employer employer;

		public Employee()
		{
		}

		public Employee(string name)
		{
			this.name = name;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public Employer Employer
		{
			get { return employer; }
			set { employer = value; }
		}
	}
}