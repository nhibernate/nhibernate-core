using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Engine
{
	public class SubselectFetch
	{
		private readonly string alias;
		private readonly ILoadable loadable;
		private readonly IDictionary<string, int[]> namedParameterLocMap;
		private readonly QueryParameters queryParameters;
		private readonly SqlString queryString;
		private readonly ISet<EntityKey> resultingEntityKeys;

		public SubselectFetch(string alias, ILoadable loadable, QueryParameters queryParameters,
		                      ISet<EntityKey> resultingEntityKeys, IDictionary<string, int[]> namedParameterLocMap)
		{
			this.resultingEntityKeys = resultingEntityKeys;
			this.queryParameters = queryParameters;
			this.namedParameterLocMap = namedParameterLocMap;
			this.loadable = loadable;
			this.alias = alias;

			queryString = queryParameters.FilteredSQL.GetSubselectString();
		}

		public QueryParameters QueryParameters
		{
			get { return queryParameters; }
		}

		public ISet<EntityKey> Result
		{
			get { return resultingEntityKeys; }
		}

		public IDictionary<string, int[]> NamedParameterLocMap
		{
			get { return namedParameterLocMap; }
		}

		public SqlString ToSubselectString(string ukname)
		{
			string[] joinColumns = ukname == null
			                       	? StringHelper.Qualify(alias, loadable.IdentifierColumnNames)
			                       	: ((IPropertyMapping) loadable).ToColumns(alias, ukname);

			SqlString sqlString =
				new SqlStringBuilder().Add("select ").Add(StringHelper.Join(", ", joinColumns)).Add(queryString).ToSqlString();
			return sqlString;
		}

		public override string ToString()
		{
			return "SubselectFetch(" + queryString + ')';
		}
	}
}