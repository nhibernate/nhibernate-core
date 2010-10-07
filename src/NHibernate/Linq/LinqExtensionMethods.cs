using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Impl;

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

        public static IEnumerable<T> ToFuture<T>(this IQueryable<T> query)
        {
            var nhQueryable = query as NhQueryable<T>;
            if (nhQueryable == null)
                throw new NotSupportedException("You can also use the AsFuture() method on NhQueryable");


            var future = ((NhQueryProvider)nhQueryable.Provider).ExecuteFuture(nhQueryable.Expression);
            return (IEnumerable<T>)future;
        }

        public static IFutureValue<T> ToFutureValue<T>(this IQueryable<T> query)
        {
            var nhQueryable = query as NhQueryable<T>;
            if (nhQueryable == null)
                throw new NotSupportedException("You can also use the AsFuture() method on NhQueryable");

            var future = ((NhQueryProvider)nhQueryable.Provider).ExecuteFuture(nhQueryable.Expression);
            if(future is DelayedEnumerator<T>)
            {
                return new FutureValue<T>(() => ((IEnumerable<T>) future));
            }
            return (IFutureValue<T>)future;
        }
	}
}