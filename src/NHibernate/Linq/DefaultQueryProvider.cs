using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Type;
using NHibernate.Util;
using System.Threading.Tasks;

namespace NHibernate.Linq
{
	public interface INhQueryProvider : IQueryProvider
	{
		object ExecuteFuture(Expression expression);
		void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters);
		Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
		object ExecuteFutureAsync(Expression expression, CancellationToken cancellationToken);
	}

	public partial class DefaultQueryProvider : INhQueryProvider
	{
		private static readonly MethodInfo CreateQueryMethodDefinition = ReflectHelper.GetMethodDefinition((INhQueryProvider p) => p.CreateQuery<object>(null));

		private readonly WeakReference _session;

		public DefaultQueryProvider(ISessionImplementor session)
		{
			_session = new WeakReference(session, true);
		}

		protected virtual ISessionImplementor Session
		{
			get { return _session.Target as ISessionImplementor; }
		}

		public virtual object Execute(Expression expression)
		{
			IQuery query;
			NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query);

			return ExecuteQuery(nhLinqExpression, query, nhLinqExpression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult) Execute(expression);
		}

		public virtual IQueryable CreateQuery(Expression expression)
		{
			MethodInfo m = CreateQueryMethodDefinition.MakeGenericMethod(expression.Type.GetGenericArguments()[0]);

			return (IQueryable) m.Invoke(this, new object[] {expression});
		}

		public virtual IQueryable<T> CreateQuery<T>(Expression expression)
		{
			return new NhQueryable<T>(this, expression);
		}

		public virtual object ExecuteFuture(Expression expression)
		{
			IQuery query;
			NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query);
			return ExecuteFutureQuery(nhLinqExpression, query);
		}

		public virtual object ExecuteFutureAsync(Expression expression, CancellationToken cancellationToken)
		{
			IQuery query;
			NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query);
			return ExecuteFutureQueryAsync(nhLinqExpression, query, cancellationToken);
		}

		public async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			return (TResult)await ExecuteAsync(expression, cancellationToken);
		}

		public virtual async Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
		{
			IQuery query;
			NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query);

			return await ExecuteQueryAsync(nhLinqExpression, query, nhLinqExpression, cancellationToken);
		}

		protected virtual NhLinqExpression PrepareQuery(Expression expression, out IQuery query)
		{
			var nhLinqExpression = new NhLinqExpression(expression, Session.Factory);

			query = Session.CreateQuery(nhLinqExpression);

			SetParameters(query, nhLinqExpression.ParameterValuesByName);
			SetResultTransformerAndAdditionalCriteria(query, nhLinqExpression, nhLinqExpression.ParameterValuesByName);

			return nhLinqExpression;
		}

		static readonly MethodInfo Future = ReflectHelper.GetMethodDefinition<IQuery>(q => q.Future<object>());
		static readonly MethodInfo FutureValue = ReflectHelper.GetMethodDefinition<IQuery>(q => q.FutureValue<object>());

		protected virtual object ExecuteFutureQuery(NhLinqExpression nhLinqExpression, IQuery query)
		{
			var method = nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence
				? Future.MakeGenericMethod(nhLinqExpression.Type)
				: FutureValue.MakeGenericMethod(nhLinqExpression.Type);

			var result = method.Invoke(query, new object[0]);

			if (nhLinqExpression.ExpressionToHqlTranslationResults.PostExecuteTransformer != null)
			{
				((IDelayedValue) result).ExecuteOnEval = nhLinqExpression.ExpressionToHqlTranslationResults.PostExecuteTransformer;
			}

			return result;
		}

		static readonly MethodInfo FutureAsync = ReflectHelper.GetMethodDefinition<IQuery>(q => q.FutureAsync<object>(default(CancellationToken)));
		static readonly MethodInfo FutureValueAsync = ReflectHelper.GetMethodDefinition<IQuery>(q => q.FutureValueAsync<object>(default(CancellationToken)));

		protected virtual object ExecuteFutureQueryAsync(NhLinqExpression nhLinqExpression, IQuery query, CancellationToken cancellationToken)
		{
			var method = nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence
				? FutureAsync.MakeGenericMethod(nhLinqExpression.Type)
				: FutureValueAsync.MakeGenericMethod(nhLinqExpression.Type);

			var result = method.Invoke(query, new object[] {cancellationToken});

			if (nhLinqExpression.ExpressionToHqlTranslationResults.PostExecuteTransformer != null)
			{
				((IDelayedValue) result).ExecuteOnEval = nhLinqExpression.ExpressionToHqlTranslationResults.PostExecuteTransformer;
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
			foreach (var parameterName in query.NamedParameters)
			{
				var param = parameters[parameterName];

				if (param.Item1 == null)
				{
					if (typeof(IEnumerable).IsAssignableFrom(param.Item2.ReturnedClass) &&
						param.Item2.ReturnedClass != typeof(string))
					{
						query.SetParameterList(parameterName, null, param.Item2);
					}
					else
					{
						query.SetParameter(parameterName, null, param.Item2);
					}
				}
				else
				{
					if (param.Item1 is IEnumerable && !(param.Item1 is string))
					{
						query.SetParameterList(parameterName, (IEnumerable)param.Item1);
					}
					else if (param.Item2 != null)
					{
						query.SetParameter(parameterName, param.Item1, param.Item2);
					}
					else
					{
						query.SetParameter(parameterName, param.Item1);
					}
				}
			}
		}

		public virtual void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters)
		{
			query.SetResultTransformer(nhExpression.ExpressionToHqlTranslationResults.ResultTransformer);

			foreach (var criteria in nhExpression.ExpressionToHqlTranslationResults.AdditionalCriteria)
			{
				criteria(query, parameters);
			}
		}
	}
}