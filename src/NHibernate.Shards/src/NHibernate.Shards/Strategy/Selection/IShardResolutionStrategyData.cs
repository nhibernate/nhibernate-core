namespace NHibernate.Shards.Strategy.Selection
{
	public interface IShardResolutionStrategyData
	{
		/// <summary>
		/// Entity name of the persistent object
		/// </summary>
		string EntityName { get; }

		/// <summary>
		/// Id of the persistent object
		/// </summary>
		object Id { get; }
	}
}