namespace NHibernate.Action
{
	//6.0 TODO: Merge to IExecutable
	public interface ICacheableExecutable : IExecutable
	{
		/// <summary>
		/// What QueryCache spaces(tables) are affected by this action?
		/// </summary>
		string[] QueryCacheSpaces { get; }
	}
}
