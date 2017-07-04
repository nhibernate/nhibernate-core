namespace NHibernate.Test.NHSpecificTest.NH2241
{
	public class User
	{
		public virtual int Id { get; set; }

		public virtual Country Country { get; set; }
	}

	public class Country
	{
		public virtual string CountryCode { get; set; }

		public virtual string CountryName { get; set; }
	}
}
