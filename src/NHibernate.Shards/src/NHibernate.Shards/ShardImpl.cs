using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Shards.Criteria;
using NHibernate.Shards.Query;
using NHibernate.Shards.Session;

namespace NHibernate.Shards
{
	public class ShardImpl : IShard
	{
		/// <summary>
		/// SessionFactoryImplementor that owns the Session associated with this Shard
		/// </summary>
		public ISessionFactoryImplementor SessionFactoryImplementor
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// the Session associated with this Shard.  Will return null if
		/// the Session has not yet been established.
		/// </summary>
		public ISession Session
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Ids of the virtual shards that are mapped to this physical shard.
		/// The returned Set is unmodifiable.
		/// </summary>
		public Set<ShardId> ShardIds
		{
			get { throw new System.NotImplementedException(); }
		}

		/// <summary>
		/// Add a open Session event 
		/// </summary>
		/// <param name="event">the event to add</param>
		public void AddOpenSessionEvent(IOpenSessionEvent @event)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// establish a Session using the SessionFactoryImplementor associated
		/// with this Shard and Apply any OpenSessionEvents that have been added.  If
		/// the Session has already been established just return it.
		/// </summary>
		/// <returns></returns>
		public ISession EstablishSession()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// @see Criteria#list()
		/// </summary>
		/// <param name="criteriaId"></param>
		/// <returns></returns>
		public IList<object> List(CriteriaId criteriaId)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// @see Criteria#uniqueResult()
		/// </summary>
		/// <param name="criteriaId"></param>
		/// <returns></returns>
		public object UniqueResult(CriteriaId criteriaId)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Return a <see cref="IQuery"/> by a <see cref="QueryId"/>
		/// </summary>
		/// <param name="queryId">id of the Query</param>
		/// <returns>the Query uniquely identified by the given id (unique to the Shard)</returns>
		public IQuery GetQueryById(QueryId queryId)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// TODO: Documentation
		/// </summary>
		/// <param name="id">the id of the Query with which the event should be associated</param>
		/// <param name="event">the event to add</param>
		public void AddQueryEvent(QueryId id, IQueryEvent @event)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// TODO: documentation
		/// </summary>
		/// <param name="shardedQuery">the ShardedQuery for which this Shard should create an actual <see cref="IQuery"/> object.</param>
		/// <returns>Query for the given ShardedQuery</returns>
		public IQuery EstablishQuery(IShardedQuery shardedQuery)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// TODO: documentation
		/// IQuery#List()
		/// </summary>
		/// <param name="queryId"></param>
		/// <returns></returns>
		public IList<object> List(QueryId queryId)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// TODO: documentation
		/// IQuery#UniqueResult()
		/// </summary>
		/// <param name="queryId"></param>
		/// <returns></returns>
		public object UniqueResult(QueryId queryId)
		{
			throw new System.NotImplementedException();
		}
	}
}