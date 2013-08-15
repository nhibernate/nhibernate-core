namespace NHibernate.Test.NHSpecificTest.NH3512
{
	class Person
	{
		public virtual int Id { get; set; }
		public virtual byte[] Version { get; set; }
		public virtual string Name { get; set; }
		public virtual int Age { get; set; }
	}

	class Employee : Person
	{
		public virtual int Salary { get; set; }
	}
}