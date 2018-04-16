using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Param
{
	/// <summary>
	/// Autogenerated parameter for <see cref="IQuery.SetMaxResults"/>.
	/// </summary>
	public partial class QueryTakeParameterSpecification : IParameterSpecification
	{
		// NOTE: don't use this for HQL take clause
		private readonly string[] idTrack;
		private readonly string limitParametersNameForThisQuery = "<nhtake" + Guid.NewGuid().ToString("N"); // NH_note: to avoid conflicts using MultiQuery/Future
		private readonly IType type = NHibernateUtil.Int32;

		public QueryTakeParameterSpecification()
		{
			idTrack = new[] { limitParametersNameForThisQuery };
		}

		#region IParameterSpecification Members

		public void Bind(DbCommand command, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			Bind(command, sqlQueryParametersList, 0, sqlQueryParametersList, queryParameters, session);
		}

		public void Bind(DbCommand command, IList<Parameter> multiSqlQueryParametersList, int singleSqlParametersOffset, IList<Parameter> sqlQueryParametersList, QueryParameters queryParameters, ISessionImplementor session)
		{
			// The QueryTakeParameterSpecification is unique so we can use multiSqlQueryParametersList
			var effectiveParameterLocations = multiSqlQueryParametersList.GetEffectiveParameterLocations(limitParametersNameForThisQuery).ToArray();
			if (effectiveParameterLocations.Any())
			{
				// if the dialect does not support variable limits the parameter may was removed
				int value = Loader.Loader.GetLimitUsingDialect(queryParameters.RowSelection, session.Factory.Dialect) ?? queryParameters.RowSelection.MaxRows;
				int position = effectiveParameterLocations.Single();
				type.NullSafeSet(command, value, position, session);
			}
		}

		public IType ExpectedType
		{
			get { return type; }
			set { throw new InvalidOperationException(); }
		}

		public string RenderDisplayInfo()
		{
			return "query-take";
		}

		public IEnumerable<string> GetIdsForBackTrack(IMapping sessionFactory)
		{
			return idTrack;
		}

		#endregion

		public override bool Equals(object obj)
		{
			return Equals(obj as QueryTakeParameterSpecification);
		}

		public bool Equals(QueryTakeParameterSpecification other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.limitParametersNameForThisQuery, limitParametersNameForThisQuery);
		}

		public override int GetHashCode()
		{
			return limitParametersNameForThisQuery.GetHashCode();
		}
	}
}