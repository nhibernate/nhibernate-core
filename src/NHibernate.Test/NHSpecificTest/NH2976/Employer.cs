using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2976
{
	public class Employer
	{
        public virtual IDictionary<Guid, Employee> Employees { get; set; }

		public Employer()
		{
            Employees= new Dictionary<Guid, Employee>();
		}

	    public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }

        public virtual void AddEmployee(Employee employee)
		{
            Employees.Add(employee.Id, employee);
			employee.Employer = this;
		}
	}
}