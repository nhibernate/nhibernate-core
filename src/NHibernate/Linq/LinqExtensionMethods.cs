using System.Linq;

namespace NHibernate.Linq
{
	public static class ExtensionMethods
	{
		public static IQueryable<T> Query<T>(this ISession session)
		{
			return new Query<T>(new NhQueryProvider(session));
		}
	}
}