using System.Collections.Generic;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Engine
{
	public class SubselectFetch
	{
		private readonly string alias;
		private readonly ILoadable loadable;
		private readonly QueryParameters queryParameters;
		private readonly SqlString queryString;
		private readonly ISet<EntityKey> resultingEntityKeys;

		public SubselectFetch(string alias, ILoadable loadable, QueryParameters queryParameters,
		                      ISet<EntityKey> resultingEntityKeys)
		{
			this.resultingEntityKeys = resultingEntityKeys;
			this.queryParameters = queryParameters;
			this.loadable = loadable;
			this.alias = alias;

			queryString = queryParameters.ProcessedSql.GetSubselectString();
		}

		public QueryParameters QueryParameters
		{
			get { return queryParameters; }
		}

		public ISet<EntityKey> Result
		{
			get { return resultingEntityKeys; }
		}

		public SqlString ToSubselectString(string ukname)
		{
			string[] joinColumns = ukname == null
			                       	? StringHelper.Qualify(alias, loadable.IdentifierColumnNames)
			                       	: ((IPropertyMapping) loadable).ToColumns(alias, ukname);

			return new SqlString("select ", StringHelper.Join(", ", joinColumns), queryString);
		}

		public override string ToString()
		{
			return "SubselectFetch(" + queryString + ')';
		}
	}
}