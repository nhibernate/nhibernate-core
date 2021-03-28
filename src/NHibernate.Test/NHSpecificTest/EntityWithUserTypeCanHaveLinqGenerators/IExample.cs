namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	public interface IExample
	{
		string Value { get; set; }
		bool IsEquivalentTo(IExample that);
	}
}
