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
	public partial class VersionTypeSeedParameterSpecification : IParameterSpecification
	{

		#region IParameterSpecification Members

		public Task BindAsync(DbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				int position = sqlQueryParametersList.GetEffectiveParameterLocations(IdBackTrack).Single(); // version parameter can't appear more than once
				return type.NullSafeSetAsync(command, type.Seed(session), position, session, cancellationToken);
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public Task BindAsync(DbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session, CancellationToken cancellationToken)
		{
			throw new NotSupportedException("Not supported for multiquery loader.");
		}

		#endregion
	}
}