using System;
using System.Collections.Generic;
using NHibernate.Engine;
using Environment = NHibernate.Cfg.Environment;

namespace NHibernate
{
	public static class SessionExtensions
	{
		public static void BulkInsert<T>(this ISession session, IEnumerable<T> entities) where T : class
		{
			BulkInsert(session.GetSessionImplementation(), entities);
		}

		public static void BulkInsert<T>(this IStatelessSession session, IEnumerable<T> entities) where T : class
		{
			BulkInsert(session.GetSessionImplementation(), entities);
		}

		private static void BulkInsert<T>(ISessionImplementor session, IEnumerable<T> entities) where T : class
		{
			using (var provider = session.Factory.ConnectionProvider.Driver.GetBulkProvider())
			{
				if (provider == null)
				{
					throw new InvalidOperationException("Current driver does not support bulk inserts.");
				}

				provider.Initialize(Environment.Properties);
				provider.Insert(session, entities);
			}
		}
	}
}
