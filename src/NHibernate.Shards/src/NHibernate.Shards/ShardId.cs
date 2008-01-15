namespace NHibernate.Shards
{
	/// <summary>
	/// Uniquely identifies a virtual shard.
	/// </summary>
	public class ShardId
	{
		private readonly int shardId;

		public ShardId(int shardId)
		{
			this.shardId = shardId;
		}

		public int Id
		{
			get { return shardId; }
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || GetType() != o.GetType())
			{
				return false;
			}

			ShardId shardId1 = (ShardId) o;

			return shardId == shardId1.shardId;
		}

		public override int GetHashCode()
		{
			return shardId;
		}

		public override string ToString()
		{
			return shardId.ToString();
		}
	}
}