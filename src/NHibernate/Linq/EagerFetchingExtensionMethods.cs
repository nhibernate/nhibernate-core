﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Linq;

namespace NHibernate.Linq
{
    public static class EagerFetchingExtensionMethods
    {
        public static INhFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(
            this IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (relatedObjectSelector == null) throw new ArgumentNullException(nameof(relatedObjectSelector));

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TOriginating), typeof(TRelated));
            return CreateFluentFetchRequest<TOriginating, TRelated>(methodInfo, query, relatedObjectSelector);
        }

		/// <summary>
		/// Fetch all lazy properties. Note that this method cannot be mixed with <see cref="Fetch{TOriginating,TRelated}"/> method that
		/// is used for fetching an individual lazy property.
		/// </summary>
		/// <typeparam name="TOriginating">The type on where all lazy properties will be fetched.</typeparam>
		/// <param name="query">The NHibernate query.</param>
		public static INhFetchRequest<TOriginating, TOriginating> FetchLazyProperties<TOriginating>(
			this IQueryable<TOriginating> query)
		{
			if (query == null) throw new ArgumentNullException(nameof(query));

			var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TOriginating));
			return CreateFluentFetchRequest<TOriginating, TOriginating>(methodInfo, query, null);
		}

        public static INhFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(
            this IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (relatedObjectSelector == null) throw new ArgumentNullException(nameof(relatedObjectSelector));

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TOriginating), typeof(TRelated));
            return CreateFluentFetchRequest<TOriginating, TRelated>(methodInfo, query, relatedObjectSelector);
        }

        public static INhFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(
            this INhFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (relatedObjectSelector == null) throw new ArgumentNullException(nameof(relatedObjectSelector));

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TQueried), typeof(TFetch), typeof(TRelated));
            return CreateFluentFetchRequest<TQueried, TRelated>(methodInfo, query, relatedObjectSelector);
        }

        public static INhFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(
            this INhFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (relatedObjectSelector == null) throw new ArgumentNullException(nameof(relatedObjectSelector));

            var methodInfo = ((MethodInfo)MethodBase.GetCurrentMethod()).MakeGenericMethod(typeof(TQueried), typeof(TFetch), typeof(TRelated));
            return CreateFluentFetchRequest<TQueried, TRelated>(methodInfo, query, relatedObjectSelector);
        }

        private static INhFetchRequest<TOriginating, TRelated> CreateFluentFetchRequest<TOriginating, TRelated>(
            MethodInfo currentFetchMethod,
            IQueryable<TOriginating> query,
            LambdaExpression relatedObjectSelector)
        {
            var queryProvider = query.Provider; // ArgumentUtility.CheckNotNullAndType<QueryProviderBase>("query.Provider", query.Provider);
            var callExpression = relatedObjectSelector != null
				? Expression.Call(currentFetchMethod, query.Expression, relatedObjectSelector)
				: Expression.Call(currentFetchMethod, query.Expression);

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
