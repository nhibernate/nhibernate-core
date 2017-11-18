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

namespace NHibernate.Linq
{
	public partial interface INhQueryProvider : IQueryProvider
	{
		Task<int> ExecuteDmlAsync<T>(QueryMode queryMode, Expression expression, CancellationToken cancellationToken);
	}

	public partial class DefaultQueryProvider : INhQueryProvider, IQueryProviderWithOptions
	{

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

		public Task<int> ExecuteDmlAsync<T>(QueryMode queryMode, Expression expression, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<int>(cancellationToken);
			}
			try
			{
				var nhLinqExpression = new NhLinqDmlExpression<T>(queryMode, expression, Session.Factory);

				var query = Session.CreateQuery(nhLinqExpression);

				SetParameters(query, nhLinqExpression.ParameterValuesByName);
				ApplyOptions(query);
				return query.ExecuteUpdateAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<int>(ex);
			}
		}
	}
}
