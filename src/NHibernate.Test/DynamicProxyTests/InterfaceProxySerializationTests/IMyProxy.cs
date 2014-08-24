namespace NHibernate.Test.DynamicProxyTests.InterfaceProxySerializationTests
{
	public interface IMyProxy
	{
		int Id { get; set; }

		string Name { get; set; }

		void ThrowDeepException();
	}
}