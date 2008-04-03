using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	using System.Collections.Generic;

	/// <summary>
	/// An Aggregation
	/// </summary>
	[Serializable]
	public class AggregateProjection : SimpleProjection
	{
		protected readonly string propertyName;
		protected readonly string aggregate;

		protected internal AggregateProjection(string aggregate, string propertyName)
		{
			this.aggregate = aggregate;
			this.propertyName = propertyName;
		}

		public override bool IsAggregate
		{
			get { return true; }
		}

		public override string ToString()
		{
			return aggregate + "(" + propertyName + ')';
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {criteriaQuery.GetType(criteria, propertyName)};
		}

		public override SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return new SqlString(new object[]
			                     	{
			                     		aggregate,
			                     		"(",
			                     		criteriaQuery.GetColumn(criteria, propertyName),
			                     		") as y",
			                     		loc.ToString(),
			                     		"_"
			                     	});
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
