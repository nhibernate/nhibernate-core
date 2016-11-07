namespace NHibernate.Test.Cascade.OneToOneCascadeDelete.Fk.Reversed.Bidirectional
{
	public class Employee
	{
		public virtual long Id { get; set; }
		public virtual string Name { get; set; }
		public virtual EmployeeInfo Info { get; set; }

		public Employee()
		{
		}
	}

	public class EmployeeInfo
	{
		public virtual long Id { get; set; }
		public virtual Employee EmployeeDetails { get; set; }
		public virtual string JobTitle { get; set; }

		public EmployeeInfo()
		{
		}

		public EmployeeInfo(Employee employee)
		{
			this.EmployeeDetails = employee;
		}
	}
}
