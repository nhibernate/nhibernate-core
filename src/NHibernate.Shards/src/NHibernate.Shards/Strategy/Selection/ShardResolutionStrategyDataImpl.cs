using System;

namespace NHibernate.Shards.Strategy.Selection
{
	public class ShardResolutionStrategyDataImpl : IShardResolutionStrategyData
	{
		private readonly String entityName;
		private readonly object id;

		public ShardResolutionStrategyDataImpl(string entityName, object id)
		{
			this.entityName = entityName;
			this.id = id;
		}

		public ShardResolutionStrategyDataImpl(System.Type type, object id)
			: this(type.FullName, id)
		{
		}

		private ShardResolutionStrategyDataImpl()
		{
		}

		#region IShardResolutionStrategyData Members

		public string EntityName
		{
			get { return entityName; }
		}

		public object Id
		{
			get { return id; }
		}

		#endregion
	}
}