

namespace NHibernate.Test.NHSpecificTest.NH2049
{
	public abstract class Customer
	{
		public int Id { get; set; }
		public bool Deleted { get; set; }
	}

	public class IndividualCustomer : Customer
	{
		public Person Person { get; set; }
	}

	public class Person
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public IndividualCustomer IndividualCustomer { get; set; }
	}
}