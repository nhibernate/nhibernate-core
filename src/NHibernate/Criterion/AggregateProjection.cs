using System;
using NHibernate.SqlCommand;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An Aggregation
	/// </summary>
	[Serializable]
	public class AggregateProjection : SimpleProjection
	{
		protected readonly string aggregate;
		protected readonly IProjection projection;
		protected readonly string propertyName;

		protected internal AggregateProjection(string aggregate, string propertyName)
		{
			this.propertyName = propertyName;
			this.aggregate = aggregate;
		}

		protected internal AggregateProjection(string aggregate, IProjection projection)
		{
			this.aggregate = aggregate;
			this.projection = projection;
		}

		public override bool IsAggregate
		{
			get { return true; }
		}

		public override string ToString()
		{
			return aggregate + "(" + (projection != null ? projection.ToString() : propertyName) + ')';
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (projection != null)
			{
				return projection.GetTypes(criteria, criteriaQuery);
			}
			return new IType[] {criteriaQuery.GetType(criteria, propertyName)};
		}

		public override SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery)
		{
			var column = CriterionUtil.GetColumnNameAsSqlStringPart(propertyName, projection, criteriaQuery, criteria);

			return new SqlString(
				aggregate,
				"(",
				column,
				") as y",
				loc.ToString(),
				"_");
		}

		public override bool IsGrouped
		{
			get { return false; }
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new InvalidOperationException("not a grouping projection");
		}
		
		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			if (projection != null)
				return projection.GetTypedValues(criteria, criteriaQuery);
			
			return base.GetTypedValues(criteria, criteriaQuery);
		}
	}
}
