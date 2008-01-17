using System.Collections.Generic;
using System.Collections.ObjectModel;
using NHibernate.Shards.Util;

namespace NHibernate.Shards
{
	/// <summary>
	/// Base implementation for HasShadIdList.
	/// Takes care of null/empty checks.
	/// </summary>
	public abstract class BaseHasShardIdList : IHasShardIdList
	{
		/// <summary>
		/// our list of <see cref="ShardId"/> objects
		/// </summary>
		protected readonly IList<ShardId> shardIds;

		protected BaseHasShardIdList(IList<ShardId> shardIds)
		{
			Preconditions.CheckNotNull(shardIds);
			Preconditions.CheckArgument(!(shardIds.Count == 0)); //not empty

			this.shardIds = new List<ShardId>(shardIds);
		}

		public BaseHasShardIdList()
		{}

		/// <summary>
		/// Unmodifiable list of <see cref="ShardId"/>s
		/// </summary>
		public IList<ShardId> ShardIds
		{
			get
			{
				return new ReadOnlyCollection<ShardId>(shardIds);
			}
		}
	}
}