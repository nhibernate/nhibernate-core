namespace NHibernate.Test.NHSpecificTest.NH3845.Interfaces
{
	public interface IPropertyEntityC : IPropertyEntityBase, IHasDescription
	{
		int AnotherNumber { get; set; }
		IAnotherEntity AnotherEntity { get; set; }
	}
}
