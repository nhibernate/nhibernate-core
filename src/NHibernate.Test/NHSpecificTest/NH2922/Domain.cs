using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2922
{
	public class Employee
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public Store Store { get; set; }
	}

	public class Store
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public IList<Employee> Staff { get; set; }

		public Store()
		{
			Staff = new List<Employee>();
		}
	}
}
