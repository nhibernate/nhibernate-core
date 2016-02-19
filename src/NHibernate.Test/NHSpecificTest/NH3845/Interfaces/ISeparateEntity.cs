namespace NHibernate.Test.NHSpecificTest.NH3845.Interfaces
{
	public interface ISeparateEntity
	{
		int SeparateEntityId { get; set; }
		IMainEntity MainEntity { get; set; }
		int SeparateEntityValue { get; set; }
	}
}
