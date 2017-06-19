﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Param
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class CriteriaNamedParameterSpecification : IParameterSpecification
	{

		#region IParameterSpecification Members

		public Task BindAsync(DbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			cancellationToken.ThrowIfCancellationRequested();
			return BindAsync(command, sqlQueryParametersList, 0, sqlQueryParametersList, queryParameters, session);
		}

		public async Task BindAsync(DbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			TypedValue typedValue = queryParameters.NamedParameters[name];
			string backTrackId = GetIdsForBackTrack(session.Factory).First();
			foreach (int position in sqlQueryParametersList.GetEffectiveParameterLocations(backTrackId))
			{
				await (ExpectedType.NullSafeSetAsync(command, typedValue.Value, position + singleSqlParametersOffset, session)).ConfigureAwait(false);
			}
		}

		#endregion
	}
}