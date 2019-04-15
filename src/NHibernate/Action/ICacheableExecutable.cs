namespace NHibernate.Action
{
	//6.0 TODO: Merge to IExecutable
	public interface ICacheableExecutable : IExecutable
	{
		bool HasCache { get; }
	}
}