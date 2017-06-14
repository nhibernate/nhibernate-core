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
using System.Data;
using System.Data.Common;
using System.Linq;
using NHibernate.Action;
using NHibernate.Engine.Query.Sql;
using NHibernate.Event;
using NHibernate.Exceptions;
using NHibernate.Hql;
using NHibernate.Impl;
using NHibernate.Loader.Custom.Sql;
using NHibernate.Param;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Engine.Query
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class NativeSQLQueryPlan
	{

		// DONE : H3.2 Executable query (now can be supported for named SQL query/ storedProcedure)
		public async Task<int> PerformExecuteUpdateAsync(QueryParameters queryParameters, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			CoordinateSharedCacheCleanup(session);

			if (queryParameters.Callable)
			{
				throw new ArgumentException("callable not yet supported for native queries");
			}

			RowSelection selection = queryParameters.RowSelection;

			int result;
			try
			{
				var parametersSpecifications = customQuery.CollectedParametersSpecifications.ToList();
				SqlString sql = ExpandDynamicFilterParameters(customQuery.SQL, parametersSpecifications, session);
				// After the last modification to the SqlString we can collect all parameters types.
				parametersSpecifications.ResetEffectiveExpectedType(queryParameters);

				var sqlParametersList = sql.GetParameters().ToList();
				SqlType[] sqlTypes = parametersSpecifications.GetQueryParameterTypes(sqlParametersList, session.Factory);
				
				var ps = await (session.Batcher.PrepareCommandAsync(CommandType.Text, sql, sqlTypes, cancellationToken)).ConfigureAwait(false);

				try
				{
					if (selection != null && selection.Timeout != RowSelection.NoValue)
					{
						// NH Difference : set Timeout for native query
						ps.CommandTimeout = selection.Timeout;
					}

					foreach (IParameterSpecification parameterSpecification in parametersSpecifications)
					{
						await (parameterSpecification.BindAsync(ps, sqlParametersList, queryParameters, session, cancellationToken)).ConfigureAwait(false);
					}
					
					result = await (session.Batcher.ExecuteNonQueryAsync(ps, cancellationToken)).ConfigureAwait(false);
				}
				finally
				{
					if (ps != null)
					{
						session.Batcher.CloseCommand(ps, null);
					}
				}
			}
			catch (HibernateException)
			{
				throw;
			}
			catch (Exception sqle)
			{
				throw ADOExceptionHelper.Convert(session.Factory.SQLExceptionConverter, sqle,
												 "could not execute native bulk manipulation query:" + sourceQuery);
			}

			return result;
		}
	}
}
