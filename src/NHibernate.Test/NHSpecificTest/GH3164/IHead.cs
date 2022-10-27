namespace NHibernate.Test.NHSpecificTest.GH3164
{

	public interface IHead
	{
		string Title { get; set; }

		bool DummyFieldToLoadEmptyComponent { get; }
	}
}
