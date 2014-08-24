using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1018
{
	public class Employer
	{
		private int id;
		private string name;
		private IList<Employee> employees = new List<Employee>();

		public Employer()
		{
		}

		public Employer(string name)
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

		public IList<Employee> Employees
		{
			get { return employees; }
			set { employees = value; }
		}

		public void AddEmployee(Employee employee)
		{
			employees.Add(employee);
			employee.Employer = this;
		}
	}
}