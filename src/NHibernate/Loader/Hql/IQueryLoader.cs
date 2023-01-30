using System.Collections;
using NHibernate.Engine;
using NHibernate.Event;

namespace NHibernate.Loader.Hql
{
	public partial interface IQueryLoader: ILoader
	{
		IList List(ISessionImplementor session, QueryParameters queryParameters);
		IEnumerable GetEnumerable(QueryParameters queryParameters, IEventSource session);
	}
}
