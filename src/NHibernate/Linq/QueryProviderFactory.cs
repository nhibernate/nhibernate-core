using System;
using NHibernate.Engine;

namespace NHibernate.Linq
{
	static class QueryProviderFactory
	{
		/// <summary>
		/// Builds a new query provider.
		/// </summary>
		/// <param name="session">A session.</param>
		/// <param name="collection">If the query is to be filtered as belonging to an entity collection, the collection.</param>
		/// <returns>The new query provider instance.</returns>
		public static INhQueryProvider CreateQueryProvider(ISessionImplementor session, object collection)
		{
			if (session.Factory.Settings.LinqQueryProviderType == null)
			{
				return new DefaultQueryProvider(session, collection);
			}
			else
			{
				// For backward compatibility, prioritize using the version without collection.
				return (collection == null
					? Activator.CreateInstance(session.Factory.Settings.LinqQueryProviderType, session)
					: Activator.CreateInstance(session.Factory.Settings.LinqQueryProviderType, session, collection)) as INhQueryProvider;
			}
		}
	}
}
