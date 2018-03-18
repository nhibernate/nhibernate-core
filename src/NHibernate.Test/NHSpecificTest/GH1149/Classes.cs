namespace NHibernate.Test.NHSpecificTest.GH1149
{
	public class Company
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual Address Address { get; set; }
	}

	public class Address
	{
		public Address()
		{
		}

		public Address(Company company)
		{
			this.Company = company;
		}

		public virtual int Id { get; set; }

		public virtual Company Company { get; set; }

		public virtual string AddressLine1 { get; set; }

	}
}
