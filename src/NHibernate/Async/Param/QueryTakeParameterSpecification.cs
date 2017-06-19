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
	public partial class QueryTakeParameterSpecification : IParameterSpecification
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

		public Task BindAsync(DbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			try
			{
				// The QueryTakeParameterSpecification is unique so we can use multiSqlQueryParametersList
				var effectiveParameterLocations = multiSqlQueryParametersList.GetEffectiveParameterLocations(limitParametersNameForThisQuery).ToArray();
				if (effectiveParameterLocations.Any())
				{
					// if the dialect does not support variable limits the parameter may was removed
					int value = Loader.Loader.GetLimitUsingDialect(queryParameters.RowSelection, session.Factory.Dialect) ?? queryParameters.RowSelection.MaxRows;
					int position = effectiveParameterLocations.Single();
					return type.NullSafeSetAsync(command, value, position, session);
				}
				return Task.CompletedTask;
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		#endregion
	}
}