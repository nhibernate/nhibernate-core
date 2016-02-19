namespace NHibernate.Test.NHSpecificTest.NH3845.Interfaces
{
	public interface IPropertyEntityBase
	{
		int PropertyEntityBaseId { get; set; }
		string Name { get; set; }
		string SharedValue { get; set; }
		IMainEntity MainEntity { get; set; }
	}
}
