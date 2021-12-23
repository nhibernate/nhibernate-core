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
using NHibernate.Multi;
using NHibernate.Param;

namespace NHibernate.Linq
{
	public partial interface INhQueryProvider : IQueryProvider
	{
		//Since 5.2
		[Obsolete("Replaced by ISupportFutureBatchNhQueryProvider interface")]
		IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression);

		//Since 5.2
		[Obsolete("Replaced by ISupportFutureBatchNhQueryProvider interface")]
		IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression);
		void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters);
		int ExecuteDml<T>(QueryMode queryMode, Expression expression);
		Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
	}

	// 6.0 TODO: merge into INhQueryProvider.
	public interface ISupportFutureBatchNhQueryProvider
	{
		IQuery GetPreparedQuery(Expression expression, out NhLinqExpression nhExpression);
		ISessionImplementor Session { get; }
	}

	/// <summary>
	/// The extended <see cref="T:System.Linq.IQueryProvider" /> that supports setting options for underlying <see cref="T:NHibernate.IQuery" />.
	/// </summary>
	public interface IQueryProviderWithOptions : IQueryProvider
	{
		/// <summary>
		/// Creates a copy of a current provider with set query options.
		/// </summary>
		/// <param name="setOptions">An options setter.</param>
		/// <returns>A new <see cref="IQueryProvider"/> with options.</returns>
		IQueryProvider WithOptions(Action<NhQueryableOptions> setOptions);
	}

	public partial class DefaultQueryProvider : INhQueryProvider, IQueryProviderWithOptions, ISupportFutureBatchNhQueryProvider
	{
		private static readonly MethodInfo CreateQueryMethodDefinition = ReflectHelper.GetMethodDefinition((INhQueryProvider p) => p.CreateQuery<object>(null));

		private readonly WeakReference<ISessionImplementor> _session;

		private readonly NhQueryableOptions _options;

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

		protected DefaultQueryProvider(ISessionImplementor session, object collection, NhQueryableOptions options)
			: this(session, collection)
		{
			_options = options;
		}

		public object Collection { get; }

		public virtual ISessionImplementor Session
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
			NhLinqExpression nhLinqExpression = PrepareQuery(expression, out var query);

			return ExecuteQuery(nhLinqExpression, query);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult)Execute(expression);
		}

		//TODO 6.0: Add to INhQueryProvider interface 
		public virtual IList<TResult> ExecuteList<TResult>(Expression expression)
		{
			var linqExpression = PrepareQuery(expression, out var query);
			var resultTransformer = linqExpression.ExpressionToHqlTranslationResults?.PostExecuteTransformer;
			if (resultTransformer == null)
			{
				return query.List<TResult>();
			}

			return new List<TResult>
			{
				(TResult) resultTransformer.DynamicInvoke(query.List().AsQueryable())
			};
		}

		public IQueryProvider WithOptions(Action<NhQueryableOptions> setOptions)
		{
			if (setOptions == null) throw new ArgumentNullException(nameof(setOptions));

			var options = _options != null
				? _options.Clone()
				: new NhQueryableOptions();
			setOptions(options);
			return CreateWithOptions(options);
		}

		protected virtual IQueryProvider CreateWithOptions(NhQueryableOptions options)
		{
			return new DefaultQueryProvider(Session, Collection, options);
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

		//Since 5.2
		[Obsolete("Replaced by ISupportFutureBatchNhQueryProvider interface")]
		public virtual IFutureEnumerable<TResult> ExecuteFuture<TResult>(Expression expression)
		{
			var nhExpression = PrepareQuery(expression, out var query);

			var result = query.Future<TResult>();
			if (result is IDelayedValue delayedValue)
				SetupFutureResult(nhExpression, delayedValue);

			return result;
		}

		//Since 5.2
		[Obsolete("Replaced by ISupportFutureBatchNhQueryProvider interface")]
		public virtual IFutureValue<TResult> ExecuteFutureValue<TResult>(Expression expression)
		{
			var nhExpression = PrepareQuery(expression, out var query);
			var linqBatchItem = new LinqBatchItem<TResult>(query, nhExpression);
			return Session.GetFutureBatch().AddAsFutureValue(linqBatchItem);
		}

		//Since 5.2
		[Obsolete]
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
				return ExecuteQueryAsync(nhLinqExpression, query, cancellationToken);
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

			SetParameters(query, nhLinqExpression.NamedParameters);
			_options?.Apply(query);
			SetResultTransformerAndAdditionalCriteria(query, nhLinqExpression, nhLinqExpression.ParameterValuesByName);

			return nhLinqExpression;
		}

		// Since v5.1
		[Obsolete("Use ExecuteQuery(NhLinqExpression nhLinqExpression, IQuery query) instead")]
		protected virtual object ExecuteQuery(NhLinqExpression nhLinqExpression, IQuery query, NhLinqExpression nhQuery)
		{
			IList results = query.List();

			if (nhQuery.ExpressionToHqlTranslationResults?.PostExecuteTransformer != null)
			{
				try
				{
					return nhQuery.ExpressionToHqlTranslationResults.PostExecuteTransformer.DynamicInvoke(results.AsQueryable());
				}
				catch (TargetInvocationException e)
				{
					throw ReflectHelper.UnwrapTargetInvocationException(e);
				}
			}

			if (nhLinqExpression.ReturnType == NhLinqExpressionReturnType.Sequence)
			{
				return results.AsQueryable();
			}

			return results[0];
		}

		protected virtual object ExecuteQuery(NhLinqExpression nhLinqExpression, IQuery query)
		{
			// For avoiding breaking derived classes, call the obsolete method until it is dropped.
#pragma warning disable 618
			return ExecuteQuery(nhLinqExpression, query, nhLinqExpression);
#pragma warning restore 618
		}

		private static void SetParameters(IQuery query, IDictionary<string, NamedParameter> parameters)
		{
			foreach (var parameterName in query.NamedParameters)
			{
				// The parameter type will be taken from the parameter metadata
				var parameter = parameters[parameterName];
				if (parameter.IsCollection)
				{
					query.SetParameterList(parameter.Name, (IEnumerable) parameter.Value);
				}
				else
				{
					//Let HQL try to process guessed types (hql doesn't support type guessing for NULL) 
					if (parameter.Type != null && (parameter.IsGuessedType == false || parameter.Value == null))
						query.SetParameter(parameter.Name, parameter.Value, parameter.Type);
					else
						query.SetParameter(parameter.Name, parameter.Value);
				}
			}
		}

		public virtual void SetResultTransformerAndAdditionalCriteria(IQuery query, NhLinqExpression nhExpression, IDictionary<string, Tuple<object, IType>> parameters)
		{
			if (nhExpression.ExpressionToHqlTranslationResults != null)
			{
				query.SetResultTransformer(nhExpression.ExpressionToHqlTranslationResults.ResultTransformer);

				foreach (var criteria in nhExpression.ExpressionToHqlTranslationResults.AdditionalCriteria)
				{
					criteria(query, parameters);
				}
			}
		}

		public int ExecuteDml<T>(QueryMode queryMode, Expression expression)
		{
			if (Collection != null)
				throw new NotSupportedException("DML operations are not supported for filters.");

			var nhLinqExpression = new NhLinqDmlExpression<T>(queryMode, expression, Session.Factory);

			var query = Session.CreateQuery(nhLinqExpression);

			SetParameters(query, nhLinqExpression.NamedParameters);
			_options?.Apply(query);
			return query.ExecuteUpdate();
		}

		public IQuery GetPreparedQuery(Expression expression, out NhLinqExpression nhExpression)
		{
			nhExpression = PrepareQuery(expression, out var query);
			return query;
		}
	}
}
