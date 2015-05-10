using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Event;
using NHibernate.Impl;
using NHibernate.Util;

namespace NHibernate.Linq
{
    public class CollectionFilterQueryProvider : IQueryProvider
    {
        private static readonly MethodInfo CreateQueryMethodDefinition = ReflectionHelper.GetMethodDefinition((IQueryProvider p) => p.CreateQuery<object>(null));

        private readonly ISessionImplementor _session;
        private readonly AbstractPersistentCollection _collection;

        public CollectionFilterQueryProvider(ISessionImplementor session, AbstractPersistentCollection collection)
        {
            this._session = session;
            this._collection = collection;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return (IQueryable)CreateQueryMethodDefinition
                .MakeGenericMethod(expression.Type.GetGenericArguments()[0])
                .Invoke(this, new object[] { expression });
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new CollectionFilterQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            var linqExpression = new NhLinqExpression(expression, this._session.Factory);

            var queryPlan = GetFilterQueryPlan(linqExpression);

            var query = new CollectionFilterQueryImpl(linqExpression, expression.ToString(), this._session.FlushMode, this._session, queryPlan.ParameterMetadata);

            var parameters = GetQueryParameters(linqExpression, query);

            var results = (IList)typeof(List<>).MakeGenericType(linqExpression.Type)
                                                 .GetConstructor(System.Type.EmptyTypes)
                                                 .Invoke(null);
            queryPlan.PerformList(parameters, this._session, results);

            if (linqExpression.ExpressionToHqlTranslationResults.PostExecuteTransformer != null)
            {
                try
                {
                    return linqExpression.ExpressionToHqlTranslationResults.PostExecuteTransformer.DynamicInvoke(results.AsQueryable());
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }

            if (linqExpression.ReturnType == NhLinqExpressionReturnType.Sequence)
            {
                return results.AsQueryable();
            }

            return results.First();
        }

        private QueryParameters GetQueryParameters(NhLinqExpression linqExpression, CollectionFilterQueryImpl query)
        {
            foreach (var parameter in linqExpression.ParameterValuesByName)
            {
                query.SetParameter(parameter.Key, parameter.Value.Item1, parameter.Value.Item2 ?? NHibernateUtil.GuessType(parameter.Value.Item1));
            }

            var parameters = query.GetQueryParameters();

            var collectionEntry = this._session.PersistenceContext.GetCollectionEntryOrNull(this._collection);

            parameters.PositionalParameterTypes = new[] { collectionEntry.LoadedPersister.KeyType };
            parameters.PositionalParameterValues = new[] { collectionEntry.Key };
            return parameters;
        }

        private FilterQueryPlan GetFilterQueryPlan(NhLinqExpression linqExpression)
        {
            return new FilterQueryPlan(linqExpression, this._collection.Role, false, this._session.EnabledFilters, this._session.Factory);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }
    }
}
