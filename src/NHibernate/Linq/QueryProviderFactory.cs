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
		/// <returns>The new query provider instance.</returns>
		public static INhQueryProvider CreateQueryProvider(ISessionImplementor session)
		{
			if (session.Factory.Settings.LinqQueryProviderType == null)
			{
				return new DefaultQueryProvider(session);
			}
			else
			{
				return Activator.CreateInstance(session.Factory.Settings.LinqQueryProviderType, session) as INhQueryProvider;
			}
		}
	}
}
