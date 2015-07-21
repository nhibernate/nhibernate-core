using System;
using System.Collections;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2976
{
	public class Employer
	{
		public virtual Guid Id { get; set; }

		public virtual string Name { get; set; }

		public virtual IDictionary<Guid, Employee> Employees1 { get; set; }

		public Employer()
		{
			Employees1 = new Dictionary<Guid, Employee>();
		}

		public virtual void AddEmployee1(Employee employee)
		{
			Employees1.Add(employee.Id, employee);
			employee.Employer = this;
		}
	}
}