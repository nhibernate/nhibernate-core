using System;

namespace NHibernate.Test.NHSpecificTest.NH2976
{
	public class Employee
	{
		public virtual Guid Id { get; protected set; }

		public virtual string Name { get; set; }

		public virtual Employer Employer { get; set; }

		public Employee()
		{
		}

		public Employee(string name, Employer employer)
		{
			Id = Guid.NewGuid();
			Name = name;
			Employer = employer;
		}
	}
}