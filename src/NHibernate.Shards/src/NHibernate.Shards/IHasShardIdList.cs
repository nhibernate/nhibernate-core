using System.Collections.Generic;

namespace NHibernate.Shards
{
	/// <summary>
	/// Interface for objects that can provide a List of ShardIds.
	/// </summary>
	public interface IHasShardIdList
	{
		/// <summary>
		/// Unmodifiable list of <see cref="ShardId"/>s
		/// </summary>
		IList<ShardId> ShardIds { get; }
	}
}