using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Param;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader.Collection
{
	/// <summary> Implements subselect fetching for a collection</summary>
	public class SubselectCollectionLoader : BasicCollectionLoader
	{
		private readonly object[] keys;
		private readonly IDictionary<string, int[]> namedParameterLocMap;
		private readonly IDictionary<string, TypedValue> namedParameters;
		private readonly IType[] types;
		private readonly object[] values;
		private readonly List<IParameterSpecification> parametersSpecifications;

		public SubselectCollectionLoader(IQueryableCollection persister, SqlString subquery, ICollection<EntityKey> entityKeys,
		                                 QueryParameters queryParameters, IDictionary<string, int[]> namedParameterLocMap,
		                                 ISessionFactoryImplementor factory, IDictionary<string, IFilter> enabledFilters)
			: base(persister, 1, subquery, factory, enabledFilters)
		{
			keys = new object[entityKeys.Count];
			int i = 0;
			foreach (EntityKey entityKey in entityKeys)
			{
				keys[i++] = entityKey.Identifier;
			}

			// NH Different behavior: to deal with positionslParameter+NamedParameter+ParameterOfFilters
			parametersSpecifications = queryParameters.ProcessedSqlParameters.ToList();
			namedParameters = queryParameters.NamedParameters;
			types = queryParameters.PositionalParameterTypes;
			values = queryParameters.PositionalParameterValues;
			this.namedParameterLocMap = namedParameterLocMap;
		}

		public override void Initialize(object id, ISessionImplementor session)
		{
			LoadCollectionSubselect(session, keys, values, types, namedParameters, KeyType);
		}

		public override int[] GetNamedParameterLocs(string name)
		{
			return namedParameterLocMap[name];
		}

		protected override IEnumerable<IParameterSpecification> GetParameterSpecifications()
		{
			return parametersSpecifications;
		}
	}
}