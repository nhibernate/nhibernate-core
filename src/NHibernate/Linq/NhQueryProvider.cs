using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Impl;
using NHibernate.Type;

namespace NHibernate.Linq
{
	public class NhQueryProvider : IQueryProvider
	{
		private readonly ISession _session;

		public NhQueryProvider(ISession session)
		{
			_session = session;
		}

		public object Execute(Expression expression)
		{
			var nhLinqExpression = new NhLinqExpression(expression);

			var query = _session.CreateQuery(nhLinqExpression);

			SetParameters(query, nhLinqExpression.ParameterValuesByName);
		    SetResultTransformerAndAdditionalCriteria(query, nhLinqExpression.ParameterValuesByName);

			var results = query.List();

			if (nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence)
			{
				return results.AsQueryable();
			}

			return results[0];
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult) Execute(expression);
		}

		public IQueryable CreateQuery(Expression expression)
		{
		    var m = ReflectionHelper.GetMethod((NhQueryProvider p) => p.CreateQuery<object>(null)).MakeGenericMethod(expression.Type.GetGenericArguments()[0]);

		    return (IQueryable) m.Invoke(this, new[] {expression});
		}

		public IQueryable<T> CreateQuery<T>(Expression expression)
		{
			return new NhQueryable<T>(this, expression);
		}

		static void SetParameters(IQuery query, IDictionary<string, Pair<object, IType>> parameters)
		{
			foreach (var parameterName in query.NamedParameters)
			{
			    var param = parameters[parameterName];
                if (param.Left == null)
                {
                    query.SetParameter(parameterName, param.Left, param.Right);
                }
                else
                {
                    query.SetParameter(parameterName, param.Left);
                }
			}
		}

        public void SetResultTransformerAndAdditionalCriteria(IQuery query, IDictionary<string, Pair<object, IType>> parameters)
        {
            var queryImpl = (ExpressionQueryImpl) query;

            var nhExpression = (NhLinqExpression) queryImpl.QueryExpression;

            query.SetResultTransformer(nhExpression.ExpressionToHqlTranslationResults.ResultTransformer);

            foreach (var criteria in nhExpression.ExpressionToHqlTranslationResults.AdditionalCriteria)
            {
                criteria(query, parameters);
            }
        }
	}

    public class Pair<TLeft, TRight>
    {
        public TLeft Left { get; set; }
        public TRight Right { get; set; }
    }
}