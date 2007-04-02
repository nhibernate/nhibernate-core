using System;
using Lucene.Net.Search;

namespace NHibernate.Search
{
	public interface IFullTextSession : ISession
	{
		IQuery CreateFullTextQuery(Query luceneQuery, params System.Type[] entities);

		IQuery CreateFullTextQuery<TEntity>(string defaultField, string query);

		IQuery CreateFullTextQuery<TEntity>(string query);

		/// <summary>
		/// Force the (re)indexing of a given <b>managed</b> object.
		/// Indexation is batched per transaction</summary>
		/// <param name="entity"></param>
		IFullTextSession Index(Object entity);
	}
}