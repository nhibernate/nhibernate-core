namespace NHibernate.Test.MappingByCode.IntegrationTests.NH3041
{
	public class PersonDetail
	{
		public virtual string LastName { get; set; }
		public virtual Person Person { get; set; }
		public virtual int? PersonDetailId { get; set; }
	}

	public class Person
	{
		public virtual string FirstName { get; set; }
		public virtual PersonDetail PersonDetail { get; set; }
		public virtual int? PersonId { get; set; }
	}
}
