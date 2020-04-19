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
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Engine.Query.Sql;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Impl
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class SqlQueryImpl : AbstractQueryImpl, ISQLQuery, ISynchronizableSQLQuery
	{

		public override async Task<IList> ListAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			NativeSQLQuerySpecification spec = GenerateQuerySpecification(namedParams);
			QueryParameters qp = GetQueryParameters(namedParams);

			Before();
			try
			{
				return await (Session.ListAsync(spec, qp, cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}

		public override async Task ListAsync(IList results, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			NativeSQLQuerySpecification spec = GenerateQuerySpecification(namedParams);
			QueryParameters qp = GetQueryParameters(namedParams);

			Before();
			try
			{
				await (Session.ListAsync(spec, qp, results, cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}

		public override async Task<IList<T>> ListAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VerifyParameters();
			IDictionary<string, TypedValue> namedParams = NamedParams;
			NativeSQLQuerySpecification spec = GenerateQuerySpecification(namedParams);
			QueryParameters qp = GetQueryParameters(namedParams);

			Before();
			try
			{
				return await (Session.ListAsync<T>(spec, qp, cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}

		public override Task<IEnumerable> EnumerableAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException("SQL queries do not currently support enumeration");
		}

		public override Task<IEnumerable<T>> EnumerableAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException("SQL queries do not currently support enumeration");
		}

		public override async Task<int> ExecuteUpdateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			IDictionary<string,TypedValue> namedParams = NamedParams;
			Before();
			try
			{
				ComputeFlattenedParameters();
				return await (Session.ExecuteNativeUpdateAsync(GenerateQuerySpecification(namedParams), GetQueryParameters(namedParams), cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}

		// Since v5.2
		/// <inheritdoc />
		[Obsolete("This method has no usages and will be removed in a future version")]
		protected internal override Task<IEnumerable<ITranslator>> GetTranslatorsAsync(ISessionImplementor sessionImplementor, QueryParameters queryParameters, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<IEnumerable<ITranslator>>(cancellationToken);
			}
			try
			{
				return Task.FromResult<IEnumerable<ITranslator>>(GetTranslators(sessionImplementor, queryParameters));
			}
			catch (Exception ex)
			{
				return Task.FromException<IEnumerable<ITranslator>>(ex);
			}
		}
	}
}
