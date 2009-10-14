using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Loader.Collection
{
	/// <summary>
	/// Implements subselect fetching for a one to many association
	/// </summary>
	public class SubselectOneToManyLoader : OneToManyLoader
	{
		private readonly object[] keys;
		private readonly IDictionary<string, int[]> namedParameterLocMap;
		private readonly IDictionary<string, TypedValue> namedParameters;
		private readonly IType[] types;
		private readonly object[] values;

		public SubselectOneToManyLoader(IQueryableCollection persister, SqlString subquery, ICollection<EntityKey> entityKeys,
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

			namedParameters = queryParameters.NamedParameters;
			// NH Different behavior: to deal with positionslParameter+NamedParameter+ParameterOfFilters
			types = new List<IType>(new JoinedEnumerable<IType>(queryParameters.FilteredParameterTypes, queryParameters.PositionalParameterTypes)).ToArray();
			values = new List<object>(new JoinedEnumerable<object>(queryParameters.FilteredParameterValues, queryParameters.PositionalParameterValues)).ToArray();
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
	}
}