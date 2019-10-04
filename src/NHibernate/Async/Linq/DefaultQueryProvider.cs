﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


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

namespace NHibernate.Linq
{
	public partial interface INhQueryProvider : IQueryProvider
	{
		Task<int> ExecuteDmlAsync<T>(QueryMode queryMode, Expression expression, CancellationToken cancellationToken);
	}

	public partial class DefaultQueryProvider : INhQueryProvider, IQueryProviderWithOptions, ISupportFutureBatchNhQueryProvider
	{

		//TODO 6.0: Add to INhQueryProvider interface 
		internal async Task<IList<TResult>> ExecuteListAsync<TResult>(Expression expression, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var linqExpression = PrepareQuery(expression, out var query);
			var resultTransformer = linqExpression.ExpressionToHqlTranslationResults?.PostExecuteTransformer;
			if (resultTransformer == null)
			{
				return await (query.ListAsync<TResult>(cancellationToken)).ConfigureAwait(false);
			}

			return new List<TResult>
			{
				(TResult) resultTransformer.DynamicInvoke((await (query.ListAsync(cancellationToken)).ConfigureAwait(false)).AsQueryable())
			};
		}

		// Since v5.1
		[Obsolete("Use ExecuteQuery(NhLinqExpression nhLinqExpression, IQuery query) instead")]
		protected virtual async Task<object> ExecuteQueryAsync(NhLinqExpression nhLinqExpression, IQuery query, NhLinqExpression nhQuery, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			IList results = await (query.ListAsync(cancellationToken)).ConfigureAwait(false);

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

		protected virtual Task<object> ExecuteQueryAsync(NhLinqExpression nhLinqExpression, IQuery query, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			// For avoiding breaking derived classes, call the obsolete method until it is dropped.
#pragma warning disable 618
			return ExecuteQueryAsync(nhLinqExpression, query, nhLinqExpression, cancellationToken);
#pragma warning restore 618
		}

		public Task<int> ExecuteDmlAsync<T>(QueryMode queryMode, Expression expression, CancellationToken cancellationToken)
		{
			if (Collection != null)
				throw new NotSupportedException("DML operations are not supported for filters.");
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			try
			{

				var nhLinqExpression = new NhLinqDmlExpression<T>(queryMode, expression, Session.Factory);

				var query = Session.CreateQuery(nhLinqExpression);

				SetParameters(query, nhLinqExpression.ParameterValuesByName);
				_options?.Apply(query);
				return query.ExecuteUpdateAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<int>(ex);
			}
		}
	}
}
