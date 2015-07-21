using System;

namespace NHibernate.Test.NHSpecificTest.NH1857
{
	public class Employee
	{
		private Employee() {}

		public Employee(int id, string name, DateTime d)
		{
			Id = id;
			Name = name;
			CompanyJoinDate = d;
		}

		public int Id { get; private set; }

		public string Name { get; set; }

		public DateTime CompanyJoinDate { get; set; }

		public Department Department { get; internal set; }
	}
}
