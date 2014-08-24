using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.Test.NHSpecificTest.NH1857
{
	public class Department
	{
		private Department() {}

		public Department(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public int Id { get; private set; }

		public string Name { get; set; }

		private ISet<Employee> _employees = new HashSet<Employee>();

		public ReadOnlyCollection<Employee> Employees
		{
			get { return new List<Employee>(_employees).AsReadOnly(); }
		}

		public void AddEmployee(Employee e)
		{
			if (e != null && !_employees.Contains(e))
			{
				e.Department = this;
				_employees.Add(e);
			}
		}
	}
}
