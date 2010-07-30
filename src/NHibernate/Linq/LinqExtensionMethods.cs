using System.Linq;
using System.Linq.Expressions;

namespace NHibernate.Linq
{
	public static class LinqExtensionMethods
	{
        public static IQueryable<T> Query<T>(this ISession session)
        {
            return new NhQueryable<T>(session);
        }

        public static IQueryable<T> Cacheable<T>(this IQueryable<T> query)
        {
            var method = ReflectionHelper.GetMethodDefinition(() => Cacheable<object>(null)).MakeGenericMethod(typeof(T));

            var callExpression = Expression.Call(method, query.Expression);

            return new NhQueryable<T>(query.Provider, callExpression);
        }

        public static IQueryable<T> CacheMode<T>(this IQueryable<T> query, CacheMode cacheMode)
        {
            var method = ReflectionHelper.GetMethodDefinition(() => CacheMode<object>(null, NHibernate.CacheMode.Normal)).MakeGenericMethod(typeof(T));

            var callExpression = Expression.Call(method, query.Expression, Expression.Constant(cacheMode));

            return new NhQueryable<T>(query.Provider, callExpression);
        }

        public static IQueryable<T> CacheRegion<T>(this IQueryable<T> query, string region)
        {
            var method = ReflectionHelper.GetMethodDefinition(() => CacheRegion<object>(null, null)).MakeGenericMethod(typeof(T));

            var callExpression = Expression.Call(method, query.Expression, Expression.Constant(region));

            return new NhQueryable<T>(query.Provider, callExpression);
        }
	}
}