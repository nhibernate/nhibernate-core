using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Linq
{
	public static class LinqExtensionMethods
	{
        public static IQueryable<T> Query<T>(this ISession session)
        {
            return new NhQueryable<T>(session);
        }

        public static void ForEach<T>(this IEnumerable<T> query, System.Action<T> method)
        {
            foreach (T item in query)
            {
                method(item);
            }
        }
    }
}