using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	using System.Collections.Generic;

	[Serializable]
	public class RowCountProjection : SimpleProjection
	{
		protected internal RowCountProjection()
		{
		}

		public override bool IsAggregate
		{
			get { return true; }
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {NHibernateUtil.Int32};
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlStringBuilder result = new SqlStringBuilder()
				.Add("count(*) as y")
				.Add(position.ToString())
				.Add("_");
			return result.ToSqlString();
		}

		public override string ToString()
		{
			return "count(*)";
		}

		public override bool IsGrouped
		{
			get { return false; }
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
		                                           IDictionary<string, IFilter> enabledFilters)
		{

			throw new InvalidOperationException("not a grouping projection");
		}
	}
}
