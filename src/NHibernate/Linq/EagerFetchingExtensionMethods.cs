using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;
using Remotion.Linq.Utilities;

namespace NHibernate.Linq
{
    public static class EagerFetchingExtensionMethods
    {
        public static INhFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(
            this IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        {
            ArgumentUtility.CheckNotNull("query", query);
            ArgumentUtility.CheckNotNull("relatedObjectSelector", relatedObjectSelector);

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TOriginating), typeof(TRelated));
            return CreateFluentFetchRequest<TOriginating, TRelated>(methodInfo, query, relatedObjectSelector);
        }

        public static INhFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(
            this IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            ArgumentUtility.CheckNotNull("query", query);
            ArgumentUtility.CheckNotNull("relatedObjectSelector", relatedObjectSelector);

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TOriginating), typeof(TRelated));
            return CreateFluentFetchRequest<TOriginating, TRelated>(methodInfo, query, relatedObjectSelector);
        }

        public static INhFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(
            this INhFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        {
            ArgumentUtility.CheckNotNull("query", query);
            ArgumentUtility.CheckNotNull("relatedObjectSelector", relatedObjectSelector);

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TQueried), typeof(TFetch), typeof(TRelated));
            return CreateFluentFetchRequest<TQueried, TRelated>(methodInfo, query, relatedObjectSelector);
        }

        public static INhFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(
            this INhFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            ArgumentUtility.CheckNotNull("query", query);
            ArgumentUtility.CheckNotNull("relatedObjectSelector", relatedObjectSelector);

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TQueried), typeof(TFetch), typeof(TRelated));
            return CreateFluentFetchRequest<TQueried, TRelated>(methodInfo, query, relatedObjectSelector);
        }

        private static INhFetchRequest<TOriginating, TRelated> CreateFluentFetchRequest<TOriginating, TRelated>(
            MethodInfo currentFetchMethod,
            IQueryable<TOriginating> query,
            LambdaExpression relatedObjectSelector)
        {
            var queryProvider = query.Provider; // ArgumentUtility.CheckNotNullAndType<QueryProviderBase>("query.Provider", query.Provider);
            var callExpression = Expression.Call(currentFetchMethod, query.Expression, relatedObjectSelector);
            return new NhFetchRequest<TOriginating, TRelated>(queryProvider, callExpression);
        }
    }

    public interface INhFetchRequest<TQueried, TFetch> : IOrderedQueryable<TQueried>
    {
    }

    public class NhFetchRequest<TQueried, TFetch> : QueryableBase<TQueried>, INhFetchRequest<TQueried, TFetch>
    {
        public NhFetchRequest(IQueryProvider provider, Expression expression)
            : base(provider, expression)
        {
        }
    }
}