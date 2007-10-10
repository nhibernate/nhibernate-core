using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader.Collection
{
	public class SubselectCollectionLoader : BasicCollectionLoader
	{
		private readonly object[] keys;
		private readonly IType[] types;
		private readonly object[] values;
		private readonly IDictionary namedParameters;
		private readonly IDictionary namedParameterLocMap;

		public SubselectCollectionLoader(
			IQueryableCollection persister,
			SqlString subquery,
			ICollection entityKeys,
			QueryParameters queryParameters,
			IDictionary namedParameterLocMap,
			ISessionFactoryImplementor factory,
			IDictionary<string, IFilter> enabledFilters)
			: base(persister, 1, subquery, factory, enabledFilters)
		{
			keys = new object[entityKeys.Count];
			int i = 0;
			foreach (EntityKey entityKey in entityKeys)
			{
				keys[i++] = entityKey.Identifier;
			}

			this.namedParameters = queryParameters.NamedParameters;
			this.types = queryParameters.FilteredPositionalParameterTypes;
			this.values = queryParameters.FilteredPositionalParameterValues;
			this.namedParameterLocMap = namedParameterLocMap;
		}

		public override void Initialize(object id, ISessionImplementor session)
		{
			LoadCollectionSubselect(
				session,
				keys,
				values,
				types,
				namedParameters,
				KeyType
				);
		}

		public override int[] GetNamedParameterLocs(string name)
		{
			return (int[]) namedParameterLocMap[name];
		}
	}
}