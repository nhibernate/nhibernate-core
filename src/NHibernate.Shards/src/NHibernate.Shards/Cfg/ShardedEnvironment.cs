using NHibernate.Cfg;

namespace NHibernate.Shards.Cfg
{
	/// <summary>
	/// Hibernate Shards configuration properties.
	/// <seealso cref="Environment"/>
	/// </summary>
	public static class ShardedEnvironment
	{
		/// <summary>
		/// Configuration property that determines whether or not we examine all
		/// associated objects for shard conflicts when we save or update.  A shard
		/// conflict is when we attempt to associate one object that lives on shard X
		/// with an object that lives on shard Y.  Turning this on will hurt
		/// performance but will prevent the programmer from ending up with the
		/// same entity on multiple shards, which is bad (at least in the current version). 
		/// </summary>
		public static readonly string CHECK_ALL_ASSOCIATED_OBJECTS_FOR_DIFFERENT_SHARDS = "hibernate.shard.enable_cross_shard_relationship_checks";

		/// <summary>
		/// Unique identifier for a shard.  Must be an Integer.
		/// </summary>
		public static readonly string SHARD_ID_PROPERTY = "hibernate.connection.shard_id";
	}
}