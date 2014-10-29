using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Driver
{
	sealed class DefaultBulkProvider : BulkProvider
	{
		public override void Insert<T>(ISessionImplementor session, IEnumerable<T> entities)
		{
			foreach (var entity in entities)
			{
				if (session is ISession)
				{
					(session as ISession).Save(entity);
				}
				else if (session is IStatelessSession)
				{
					(session as IStatelessSession).Insert(entity);
				}
			}
		}
	}
}
