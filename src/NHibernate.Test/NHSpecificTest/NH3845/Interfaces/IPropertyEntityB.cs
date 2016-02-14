namespace NHibernate.Test.NHSpecificTest.NH3845.Interfaces
{
	public interface IPropertyEntityB : IPropertyEntityBase, IHasDescription
	{
		string AnotherString { get; set; }
	}
}
