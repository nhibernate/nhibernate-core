namespace NHibernate.ByteCode.LinFu.Tests.ProxyInterface
{
	public interface IMyProxy
	{
		int Id { get; set; }

		string Name { get; set; }

		void ThrowDeepException();
	}
}