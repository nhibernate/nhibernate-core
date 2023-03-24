namespace NHibernate.Test.NHSpecificTest.GH3263
{
	public class Employee
	{
		public virtual int EmployeeId { get; set; }
		public virtual string Name { get; set; }
		public virtual OptionalInfo OptionalInfo { get; set; }
	}

	public class OptionalInfo
	{
		public virtual int EmployeeId { get; set; }
		public virtual int Age { get; set; }
		public virtual Employee Employee { get; set; }
	}
}
