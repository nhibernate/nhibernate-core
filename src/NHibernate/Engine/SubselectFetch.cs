using System;
using System.Collections;
using Iesi.Collections;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Engine
{
	using Dialect;

	public class SubselectFetch
	{
		private readonly ISet resultingEntityKeys;
		private readonly SqlString queryString;
		private readonly string alias;
		private readonly Dialect dialect;
		private readonly ILoadable loadable;
		private readonly QueryParameters queryParameters;
		private readonly IDictionary namedParameterLocMap;

		public SubselectFetch(
			string alias,
			Dialect dialect,
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
			this.dialect = dialect;

			queryString = queryParameters.FilteredSQL.GetSubselectString();
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
			string[] joinColumns = ukname == null
			                       	? StringHelper.Qualify(alias, loadable.IdentifierColumnNames)
			                       	: ((IPropertyMapping) loadable).ToColumns(alias, ukname);

			SqlString sqlString = new SqlStringBuilder()
				.Add("select ")
				.Add(StringHelper.Join(", ", joinColumns))
				.Add(queryString)
				.ToSqlString();

			RowSelection selection = queryParameters.RowSelection;

			bool useLimit = Loader.Loader.UseLimit(selection, dialect);
			bool hasFirstRow = Loader.Loader.GetFirstRow(selection) > 0;
			bool useOffset = hasFirstRow && useLimit && dialect.SupportsLimitOffset;

			if ((useLimit || hasFirstRow) == false)
				return sqlString;

			sqlString = AppendOrderByIfNeeded(sqlString);

			return dialect.GetLimitString(sqlString.Trim(),
										  useOffset ? Loader.Loader.GetFirstRow(selection) : 0,
										  Loader.Loader.GetMaxOrLimit(dialect, selection));
		}

		private SqlString AppendOrderByIfNeeded(SqlString sqlString)
		{
			SqlString orderByOrEmpty = queryParameters.FilteredSQL.SubstringStartingWithLast("order by");
			return sqlString.Append(orderByOrEmpty);
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
