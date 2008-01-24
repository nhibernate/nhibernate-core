using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Shards.Criteria;
using NHibernate.Shards.Query;
using NHibernate.Shards.Session;

namespace NHibernate.Shards
{
	/// <summary>
	/// Interface representing a Shard.  A shard is a physical partition (as opposed
	/// to a virtual partition).  Shards know how to lazily instantiate Sessions
	/// and Apply {@link OpenSessionEvent}s, {@link CriteriaEvent}s, and {@link QueryEvent}s.
	/// Anybody else have a nagging suspicion this can get folded into the Session
	/// itself?
	/// </summary>
	public interface IShard
	{
		/// <summary>
		/// SessionFactoryImplementor that owns the Session associated with this Shard
		/// </summary>
		ISessionFactoryImplementor SessionFactoryImplementor { get; }

		/// <summary>
		/// the Session associated with this Shard.  Will return null if
		/// the Session has not yet been established.
		/// </summary>
		ISession Session { get; }

		/// <summary>
		/// Ids of the virtual shards that are mapped to this physical shard.
		/// The returned Set is unmodifiable.
		/// </summary>
		Set<ShardId> ShardIds { get; }

		/// <summary>
		/// Add a open Session event 
		/// </summary>
		/// <param name="event">the event to add</param>
		void AddOpenSessionEvent(IOpenSessionEvent @event);

		/// <summary>
		/// establish a Session using the SessionFactoryImplementor associated
		/// with this Shard and Apply any OpenSessionEvents that have been added.  If
		/// the Session has already been established just return it.
		/// </summary>
		/// <returns></returns>
		ISession EstablishSession();

		/// <summary>
		/// @see Criteria#list()
		/// </summary>
		/// <param name="criteriaId"></param>
		/// <returns></returns>
		IList<object> List(CriteriaId criteriaId);


		/// <summary>
		/// @see Criteria#uniqueResult()
		/// </summary>
		/// <param name="criteriaId"></param>
		/// <returns></returns>
		object UniqueResult(CriteriaId criteriaId);

		/// <summary>
		/// Return a <see cref="IQuery"/> by a <see cref="QueryId"/>
		/// </summary>
		/// <param name="queryId">id of the Query</param>
		/// <returns>the Query uniquely identified by the given id (unique to the Shard)</returns>
		IQuery GetQueryById(QueryId queryId);

		/// <summary>
		/// TODO: Documentation
		/// </summary>
		/// <param name="id">the id of the Query with which the event should be associated</param>
		/// <param name="event">the event to add</param>
		void AddQueryEvent(QueryId id, IQueryEvent @event);

		/// <summary>
		/// TODO: documentation
		/// </summary>
		/// <param name="shardedQuery">the ShardedQuery for which this Shard should create an actual <see cref="IQuery"/> object.</param>
		/// <returns>Query for the given ShardedQuery</returns>
		IQuery EstablishQuery(IShardedQuery shardedQuery);

		/// <summary>
		/// TODO: documentation
		/// IQuery#List()
		/// </summary>
		/// <param name="queryId"></param>
		/// <returns></returns>
		IList<object> List(QueryId queryId);

		/// <summary>
		/// TODO: documentation
		/// IQuery#UniqueResult()
		/// </summary>
		/// <param name="queryId"></param>
		/// <returns></returns>
		object UniqueResult(QueryId queryId);
	}
}