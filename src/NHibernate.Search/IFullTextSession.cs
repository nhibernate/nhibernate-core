using System;
using Lucene.Net.Search;

namespace NHibernate.Search
{
	public interface IFullTextSession : ISession
	{
		IQuery CreateFullTextQuery<TEntity>(string defaultField, string query);

		IQuery CreateFullTextQuery<TEntity>(string query);

        IQuery CreateFullTextQuery(Lucene.Net.Search.Query luceneQuery, params System.Type[] entities);

		/// <summary>
		/// Force the (re)indexing of a given <b>managed</b> object.
		/// Indexation is batched per transaction</summary>
		/// <param name="entity"></param>
		IFullTextSession Index(Object entity);

        /// <summary>
        /// Purge the instance with the specified identity from the index, but not the database.
        /// </summary>
        /// <param name="clazz"></param>
        /// <param name="id"></param>
        void Purge(System.Type clazz, object id);

        /// <summary>
        /// Purge all instances from the index, but not the database.
        /// </summary>
        /// <param name="clazz"></param>
        void PurgeAll(System.Type clazz);
	}
}