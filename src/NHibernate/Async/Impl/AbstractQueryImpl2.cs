﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Engine.Query;

namespace NHibernate.Impl
{
	using System.Threading.Tasks;
	using System.Threading;
	public abstract partial class AbstractQueryImpl2 : AbstractQueryImpl
	{

		public override async Task<int> ExecuteUpdateAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VerifyParameters();
			var namedParams = NamedParams;
			Before();
			try
			{
				return await (Session.ExecuteUpdateAsync(ExpandParameters(namedParams), GetQueryParameters(namedParams), cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}

		public override async Task<IEnumerable> EnumerableAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VerifyParameters();
			var namedParams = NamedParams;
			Before();
			try
			{
				return await (Session.EnumerableAsync(ExpandParameters(namedParams), GetQueryParameters(namedParams), cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}

		public override async Task<IEnumerable<T>> EnumerableAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VerifyParameters();
			var namedParams = NamedParams;
			Before();
			try
			{
				return await (Session.EnumerableAsync<T>(ExpandParameters(namedParams), GetQueryParameters(namedParams), cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}

		public override async Task<IList> ListAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			VerifyParameters();
			var namedParams = NamedParams;
			Before();
			try
			{
				return await (Session.ListAsync(ExpandParameters(namedParams), GetQueryParameters(namedParams), cancellationToken)).ConfigureAwait(false);
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
			var namedParams = NamedParams;
			Before();
			try
			{
				await (Session.ListAsync(ExpandParameters(namedParams), GetQueryParameters(namedParams), results, cancellationToken)).ConfigureAwait(false);
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
			var namedParams = NamedParams;
			Before();
			try
			{
				return await (Session.ListAsync<T>(ExpandParameters(namedParams), GetQueryParameters(namedParams), cancellationToken)).ConfigureAwait(false);
			}
			finally
			{
				After();
			}
		}

		protected internal override async Task<IEnumerable<ITranslator>> GetTranslatorsAsync(ISessionImplementor sessionImplementor, QueryParameters queryParameters, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			// NOTE: updates queryParameters.NamedParameters as (desired) side effect
			var queryExpression = ExpandParameters(queryParameters.NamedParameters);

			return (await (sessionImplementor.GetQueriesAsync(queryExpression, false, cancellationToken)).ConfigureAwait(false))
									 .Select(queryTranslator => new HqlTranslatorWrapper(queryTranslator));
		}
	}
}