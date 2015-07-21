namespace NHibernate.Test.DynamicEntity
{
	public interface Address
	{
		long Id { get; set;}
		string Street { get; set;}
		string City { get; set;}
		string PostalCode { get; set;}
	}
}