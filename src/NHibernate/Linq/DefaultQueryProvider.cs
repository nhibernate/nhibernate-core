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
	public partial interface INhQueryProvider : IQueryProvider
	{
		IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression);
		IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression);
		void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters);
		int ExecuteDml<T>(QueryMode queryMode, Expression expression);
		Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
	}

	public partial class DefaultQueryProvider : INhQueryProvider
	{
		private static readonly MethodInfo CreateQueryMethodDefinition = ReflectHelper.GetMethodDefinition((INhQueryProvider p) => p.CreateQuery<object>(null));

		private readonly WeakReference<ISessionImplementor> _session;

		public DefaultQueryProvider(ISessionImplementor session)
		{
			// Short reference (no trackResurrection). If the session gets garbage collected, it will be in an unpredictable state:
			// better throw rather than resurrecting it.
			// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/weak-references
			_session = new WeakReference<ISessionImplementor>(session);
		}

		public DefaultQueryProvider(ISessionImplementor session, object collection)
			: this(session)
		{
			Collection = collection;
		}

		public object Collection { get; }

		protected virtual ISessionImplementor Session
		{
			get
			{
				if (!_session.TryGetTarget(out var target))
					throw new InvalidOperationException("Session has already been garbage collected");
				return target;
			}
		}

		public virtual object Execute(Expression expression)
		{
			IQuery query;
			NhLinqExpression nhLinqExpression = PrepareQuery(expression, out query);

			return ExecuteQuery(nhLinqExpression, query, nhLinqExpression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult)Execute(expression);
		}

		public virtual IQueryable CreateQuery(Expression expression)
		{
			MethodInfo m = CreateQueryMethodDefinition.MakeGenericMethod(expression.Type.GetGenericArguments()[0]);

			return (IQueryable)m.Invoke(this, new object[] { expression });
		}

		public virtual IQueryable<T> CreateQuery<T>(Expression expression)
		{
			return new NhQueryable<T>(this, expression);
		}

		public virtual IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression)
		{
			var nhExpression = PrepareQuery(expression, out var query);

			var result = query.Future<TResult>();
			SetupFutureResult(nhExpression, (IDelayedValue)result);

			return result;
		}

		public virtual IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression)
		{
			var nhExpression = PrepareQuery(expression, out var query);

			var result = query.FutureValue<TResult>();
			SetupFutureResult(nhExpression, (IDelayedValue)result);

			return result;
		}

		private static void SetupFutureResult(NhLinqExpression nhExpression, IDelayedValue result)
		{
			if (nhExpression.ExpressionToHqlTranslationResults.PostExecuteTransformer == null)
				return;

			result.ExecuteOnEval = nhExpression.ExpressionToHqlTranslationResults.PostExecuteTransformer;
		}

		public async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			return (TResult)await ExecuteAsync(expression, cancellationToken).ConfigureAwait(false);
		}

		public virtual Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				var nhLinqExpression = PrepareQuery(expression, out var query);
				return ExecuteQueryAsync(nhLinqExpression, query, nhLinqExpression, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		protected virtual NhLinqExpression PrepareQuery(Expression expression, out IQuery query)
		{
			var nhLinqExpression = new NhLinqExpression(expression, Session.Factory);

			if (Collection == null)
			{
				query = Session.CreateQuery(nhLinqExpression);
			}
			else
			{
				query = Session.CreateFilter(Collection, nhLinqExpression);
			}

			SetParameters(query, nhLinqExpression.ParameterValuesByName);
			SetResultTransformerAndAdditionalCriteria(query, nhLinqExpression, nhLinqExpression.ParameterValuesByName);

			return nhLinqExpression;
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

		public int ExecuteDml<T>(QueryMode queryMode, Expression expression)
		{
			if (Collection != null)
				throw new NotSupportedException("DML operations are not supported for filters.");

			var nhLinqExpression = new NhLinqDmlExpression<T>(queryMode, expression, Session.Factory);

			var query = Session.CreateQuery(nhLinqExpression);

			SetParameters(query, nhLinqExpression.ParameterValuesByName);

			return query.ExecuteUpdate();
		}
	}
}
