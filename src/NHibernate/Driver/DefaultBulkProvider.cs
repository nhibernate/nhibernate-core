using System;
using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Driver
{
	sealed class DefaultBulkProvider : BulkProvider
	{
		public override void Insert<T>(ISessionImplementor session, IEnumerable<T> entities)
		{
			var statelessSession = session as IStatelessSession;

			if (statelessSession == null)
			{
				throw new InvalidOperationException("Insert can only be called with stateless sessions.");
			}

			foreach (var entity in entities)
			{
				statelessSession.Insert(entity);
			}
		}
	}
}
