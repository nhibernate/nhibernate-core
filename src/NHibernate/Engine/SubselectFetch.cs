using System;
using System.Collections;
using System.Text;
using Iesi.Collections;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Engine
{
	public class SubselectFetch
	{
		private readonly ISet resultingEntityKeys;
		private readonly SqlString queryString;
		private readonly string alias;
		private readonly ILoadable loadable;
		private readonly QueryParameters queryParameters;
		private readonly IDictionary namedParameterLocMap;

		public SubselectFetch(
			string alias,
			ILoadable loadable,
			QueryParameters queryParameters,
			ISet resultingEntityKeys,
			IDictionary namedParameterLocMap
		)
		{
			this.resultingEntityKeys = resultingEntityKeys;
			this.queryParameters = queryParameters;
			this.namedParameterLocMap = namedParameterLocMap;
			this.loadable = loadable;
			this.alias = alias;

			//TODO: ugly here:
			SqlString queryString = queryParameters.FilteredSQL;
			int fromIndex = queryString.IndexOfCaseInsensitive(" from ");
			int orderByIndex = queryString.LastIndexOfCaseInsensitive("order by");
			this.queryString = orderByIndex > 0 ?
					queryString.Substring(fromIndex, orderByIndex - fromIndex) :
					queryString.Substring(fromIndex);

		}

		public QueryParameters QueryParameters
		{
			get { return queryParameters; }
		}

		public ISet Result
		{
			get { return resultingEntityKeys; }
		}

		public SqlString ToSubselectString(string ukname)
		{

			string[] joinColumns = ukname == null ?
				StringHelper.Qualify(alias, loadable.IdentifierColumnNames) :
				((IPropertyMapping) loadable).ToColumns(alias, ukname);

			return new SqlStringBuilder()
				.Add("select ")
				.Add(StringHelper.Join(", ", joinColumns))
				.Add(queryString)
				.ToSqlString();
		}

		public override string ToString()
		{
			return "SubselectFetch(" + queryString + ')';
		}

		public IDictionary NamedParameterLocMap
		{
			get { return namedParameterLocMap; }
		}
	}
}
