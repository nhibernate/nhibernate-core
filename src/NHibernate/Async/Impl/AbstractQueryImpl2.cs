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
	using System;
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

		/// <summary>
		/// Return the query results as an <see cref="IEnumerable"/>. If the query contains multiple results
		/// per row, the results are returned in an instance of <c>object[]</c>.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <remarks>
		/// <p>
		/// Entities returned as results are initialized on demand. The first SQL query returns
		/// identifiers only.
		/// </p>
		/// <p>
		/// This is a good strategy to use if you expect a high number of the objects
		/// returned to be already loaded in the <see cref="ISession"/> or in the 2nd level cache.
		/// </p>
		/// </remarks>
		// Since v5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
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

		/// <summary>
		/// Strongly-typed version of <see cref="Enumerable()"/>.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		// Since v5.3
		[Obsolete("This method has no more usages and will be removed in a future version")]
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
			catch (System.Exception ex)
			{
				return Task.FromException<IEnumerable<ITranslator>>(ex);
			}
		}
	}
}
