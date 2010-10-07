using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
		    IQuery query;
		    NhLinqExpression nhQuery;
		    NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query, out nhQuery);
            
			return ExecuteQuery(nhLinqExpression, query, nhQuery);
		}

        public object ExecuteFuture(Expression expression)
        {
            IQuery query;
            NhLinqExpression nhQuery;
            NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query, out nhQuery);
            return ExecuteFutureQuery(nhLinqExpression, query, nhQuery); 
        }

	    private NhLinqExpression PrepareQuery(Expression expression, out IQuery query, out NhLinqExpression nhQuery)
	    {
	        var nhLinqExpression = new NhLinqExpression(expression);

	        query = _session.CreateQuery(nhLinqExpression);

	        nhQuery = query.As<ExpressionQueryImpl>().QueryExpression.As<NhLinqExpression>();

	        SetParameters(query, nhLinqExpression.ParameterValuesByName);
	        SetResultTransformerAndAdditionalCriteria(query, nhQuery, nhLinqExpression.ParameterValuesByName);
	        return nhLinqExpression;
	    }

	    private object ExecuteFutureQuery(NhLinqExpression nhLinqExpression, IQuery query, NhLinqExpression nhQuery)
	    {
	        MethodInfo method;
            if (nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence)
	        {
	            method = typeof (IQuery).GetMethod("Future").MakeGenericMethod(nhQuery.Type);
	        }
            else
            {
                method = typeof(IQuery).GetMethod("FutureValue").MakeGenericMethod(nhQuery.Type);
            }

	        var result = method.Invoke(query, new object[0]);


            
	        if (nhQuery.ExpressionToHqlTranslationResults.PostExecuteTransformer != null)
	        {
	            ((IDelayedValue) result).ExecuteOnEval = nhQuery.ExpressionToHqlTranslationResults.PostExecuteTransformer;
	        }

	        return result;

	    }

	    private object ExecuteQuery(NhLinqExpression nhLinqExpression, IQuery query, NhLinqExpression nhQuery)
	    {
	        var results = query.List();

	        if (nhQuery.ExpressionToHqlTranslationResults.PostExecuteTransformer != null)
	        {
	            try
	            {
	                return nhQuery.ExpressionToHqlTranslationResults.PostExecuteTransformer.DynamicInvoke(results.AsQueryable());
	            }
	            catch (TargetInvocationException e)
	            {
	                throw e.InnerException;
	            }
	        }

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
		    var m = ReflectionHelper.GetMethodDefinition((NhQueryProvider p) => p.CreateQuery<object>(null)).MakeGenericMethod(expression.Type.GetGenericArguments()[0]);

		    return (IQueryable) m.Invoke(this, new[] {expression});
		}

		public IQueryable<T> CreateQuery<T>(Expression expression)
		{
			return new NhQueryable<T>(this, expression);
		}

		static void SetParameters(IQuery query, IDictionary<string, Tuple<object, IType>> parameters)
		{
			foreach (var parameterName in query.NamedParameters)
			{
			    var param = parameters[parameterName];

                if (param.First == null)
                {
                    if (typeof(ICollection).IsAssignableFrom(param.Second.ReturnedClass))
                    {
                        query.SetParameterList(parameterName, null, param.Second);
                    }
                    else
                    {
                        query.SetParameter(parameterName, null, param.Second);
                    }
                }
                else
                {
                    if (param.First is ICollection)
                    {
                        query.SetParameterList(parameterName, (ICollection) param.First);
                    }
                    else
                    {
                        query.SetParameter(parameterName, param.First);
                    }
                }
			}
		}

        public void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters)
        {
            query.SetResultTransformer(nhExpression.ExpressionToHqlTranslationResults.ResultTransformer);

            foreach (var criteria in nhExpression.ExpressionToHqlTranslationResults.AdditionalCriteria)
            {
                criteria(query, parameters);
            }
        }
	}

    public class Tuple<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
 
    }

    public class Tuple<T1, T2, T3>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
        public T3 Third { get; set; }
    }
}