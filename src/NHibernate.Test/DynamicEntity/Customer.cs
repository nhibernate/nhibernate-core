namespace NHibernate.Test.DynamicEntity
{
	public interface Customer:Person
	{
		Company Company { get; set;}
	}
}