using NHibernate.Id;

namespace NHibernate.Shards.Id
{
	/// <summary>
	/// Identifier generator that contains encoded the Shard Id in the identifier.
	/// </summary>
	public interface IShardEncodingIdentifierGenerator : IIdentifierGenerator
	{
		ShardId ExtractShardId(object identifier);
	}
}