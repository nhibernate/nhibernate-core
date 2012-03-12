using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Type;

namespace NHibernate.Linq
{
	public interface INhQueryProvider : IQueryProvider
	{
		object ExecuteFuture(Expression expression);
		void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters);
	}

	public class DefaultQueryProvider : INhQueryProvider
	{
		public DefaultQueryProvider(ISessionImplementor session)
		{
			Session = session;
		}

		protected virtual ISessionImplementor Session { get; private set; }

		#region IQueryProvider Members

		public virtual object Execute(Expression expression)
		{
			IQuery query;
			NhLinqExpression nhQuery;
			NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query, out nhQuery);

			return ExecuteQuery(nhLinqExpression, query, nhQuery);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult) Execute(expression);
		}

		public virtual IQueryable CreateQuery(Expression expression)
		{
			MethodInfo m = ReflectionHelper.GetMethodDefinition((DefaultQueryProvider p) => p.CreateQuery<object>(null)).MakeGenericMethod(expression.Type.GetGenericArguments()[0]);

			return (IQueryable) m.Invoke(this, new[] {expression});
		}

		public virtual IQueryable<T> CreateQuery<T>(Expression expression)
		{
			return new NhQueryable<T>(this, expression);
		}

		#endregion

		public virtual object ExecuteFuture(Expression expression)
		{
			IQuery query;
			NhLinqExpression nhQuery;
			NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query, out nhQuery);
			return ExecuteFutureQuery(nhLinqExpression, query, nhQuery);
		}

		protected NhLinqExpression PrepareQuery(Expression expression, out IQuery query, out NhLinqExpression nhQuery)
		{
			var nhLinqExpression = new NhLinqExpression(expression, Session.Factory);

			query = Session.CreateQuery(nhLinqExpression);

			nhQuery = query.As<ExpressionQueryImpl>().QueryExpression.As<NhLinqExpression>();

			SetParameters(query, nhLinqExpression.ParameterValuesByName);
			SetResultTransformerAndAdditionalCriteria(query, nhQuery, nhLinqExpression.ParameterValuesByName);
			return nhLinqExpression;
		}

		protected virtual object ExecuteFutureQuery(NhLinqExpression nhLinqExpression, IQuery query, NhLinqExpression nhQuery)
		{
			MethodInfo method;
			if (nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence)
			{
				method = typeof (IQuery).GetMethod("Future").MakeGenericMethod(nhQuery.Type);
			}
			else
			{
				method = typeof (IQuery).GetMethod("FutureValue").MakeGenericMethod(nhQuery.Type);
			}

			object result = method.Invoke(query, new object[0]);


			if (nhQuery.ExpressionToHqlTranslationResults.PostExecuteTransformer != null)
			{
				((IDelayedValue) result).ExecuteOnEval = nhQuery.ExpressionToHqlTranslationResults.PostExecuteTransformer;
			}

			return result;
		}

		protected virtual object ExecuteQuery(NhLinqExpression nhLinqExpression, IQuery query, NhLinqExpression nhQuery)
		{
			IList results = query.List();

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

		private static void SetParameters(IQuery query, IDictionary<string, Tuple<object, IType>> parameters)
		{
			foreach (string parameterName in query.NamedParameters)
			{
				Tuple<object, IType> param = parameters[parameterName];

				if (param.First == null)
				{
					if (typeof (ICollection).IsAssignableFrom(param.Second.ReturnedClass))
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
					else if (param.Second != null)
					{
						query.SetParameter(parameterName, param.First, param.Second);
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