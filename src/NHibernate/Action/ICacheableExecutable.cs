namespace NHibernate.Action
{
	//6.0 TODO: Merge to IExecutable
	public interface ICacheableExecutable : IExecutable
	{
		/// <summary>
		/// The query cache spaces (tables) which are affected by this action.
		/// </summary>
		string[] QueryCacheSpaces { get; }
	}
}
