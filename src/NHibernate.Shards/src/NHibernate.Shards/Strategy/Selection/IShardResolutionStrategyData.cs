namespace NHibernate.Shards.Strategy.Selection
{
	public interface IShardResolutionStrategyData
	{
		string EntityName { get; }

		/// <summary>
		/// Id is a serializable object
		/// </summary>
		object Id { get; }
	}
}