using NHibernate.Shards;
using NHibernate.Shards.Util;
using NHibernate.Type;

namespace NHibernate.Shards.Session
{
	/// <summary>
	/// Interceptor that sets the <see cref="ShardId"/> of any object
	/// that implements the <see cref="IShardAware"/> interface and does already know its
	/// <see cref="ShardId"/> when the object is saved or loaded.
	/// </summary>
	public class ShardAwareInterceptor : EmptyInterceptor
	{
		private readonly IShardIdResolver shardIdResolver;

		public ShardAwareInterceptor(IShardIdResolver shardIdResolver)
		{
			Preconditions.CheckNotNull(shardIdResolver);
			this.shardIdResolver = shardIdResolver;
		}

		public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			return SetShardId(entity);
		}

		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			return SetShardId(entity);
		}

		/// <summary>
		/// Set the ShardId if the entity is <see cref="IShardAware"/> and it ShardId is null.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns>True if the state of the entity was changed</returns>
		private bool SetShardId(object entity)
		{
			if(entity is IShardAware)
			{
				IShardAware shardAware = (IShardAware) entity;

				if (shardAware.ShardId == null)
				{
					shardAware.ShardId = this.shardIdResolver.GetShardIdForObject(entity);
					return true;
				}
				
			}
			return false;
		}
	}
}