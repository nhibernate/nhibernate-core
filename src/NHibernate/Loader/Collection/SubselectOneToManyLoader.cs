using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Implements subselect fetching for a one to many association
	/// </summary>
	public class SubselectOneToManyLoader : OneToManyLoader
	{
		private const int BatchSizeForSubselectFetching = 1;
		private readonly object[] keys;
		private readonly IDictionary<string, TypedValue> namedParameters;
		private readonly IType[] types;
		private readonly object[] values;
		private readonly List<IParameterSpecification> parametersSpecifications;

		public SubselectOneToManyLoader(IQueryableCollection persister, SqlString subquery, ICollection<EntityKey> entityKeys,
		                                QueryParameters queryParameters,
		                                ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(persister, BatchSizeForSubselectFetching, factory, enabledFilters)
		{
			keys = new object[entityKeys.Count];
			int i = 0;
			foreach (EntityKey entityKey in entityKeys)
			{
				keys[i++] = entityKey.Identifier;
			}

			// NH Different behavior: to deal with positionslParameter+NamedParameter+ParameterOfFilters
			namedParameters = new Dictionary<string, TypedValue>(queryParameters.NamedParameters);
			parametersSpecifications = queryParameters.ProcessedSqlParameters.ToList();
			var processedRowSelection = queryParameters.ProcessedRowSelection;
			SqlString finalSubquery = subquery;
			if (queryParameters.ProcessedRowSelection != null && !SubselectClauseExtractor.HasOrderBy(queryParameters.ProcessedSql))
			{
				// when the original query has an "ORDER BY" we can't re-apply the pagination.
				// This is a simplification, we should actually check which is the "ORDER BY" clause because when the "ORDER BY" is just for the PK we can re-apply "ORDER BY" and pagination.
				finalSubquery = GetSubSelectWithLimits(subquery, parametersSpecifications, processedRowSelection, namedParameters);
			}
			InitializeFromWalker(persister, finalSubquery, BatchSizeForSubselectFetching, enabledFilters, factory);

			types = queryParameters.PositionalParameterTypes;
			values = queryParameters.PositionalParameterValues;
		}

		public override void Initialize(object id, ISessionImplementor session)
		{
			LoadCollectionSubselect(session, keys, values, types, namedParameters, KeyType);
		}

		protected override IEnumerable<IParameterSpecification> GetParameterSpecifications()
		{
			return parametersSpecifications;
		}
	}
}