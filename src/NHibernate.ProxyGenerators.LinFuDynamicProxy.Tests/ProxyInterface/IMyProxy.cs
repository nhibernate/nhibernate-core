namespace NHibernate.ProxyGenerators.LinFuDynamicProxy.Tests.ProxyInterface
{
	public interface IMyProxy
	{
		int Id { get; set; }

		string Name { get; set; }

		void ThrowDeepException();
	}
}