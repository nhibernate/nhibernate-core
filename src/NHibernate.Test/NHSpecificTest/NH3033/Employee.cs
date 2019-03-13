using System;

namespace NHibernate.Test.NHSpecificTest.NH3033
{
	public class Employee
	{
		protected Employee() : this(string.Empty)
		{
		}

		public Employee(string name)
		{
			Name = name;
		}

		public virtual Guid Id { get; protected set; }

		public virtual string Name { get; protected set; }

		public virtual Company Company { get; protected set; }

		public virtual void WorksIn(Company company)
		{
			Company = company;
		}
	}
}
