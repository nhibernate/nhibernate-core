using System;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	/// <summary>
	/// A property value, or grouped property value
	/// </summary>
	[Serializable]
	public class SubqueryProjection : SimpleProjection
	{
		private readonly SelectSubqueryExpression _subQuery;

		protected internal SubqueryProjection(SelectSubqueryExpression subquery)
		{
			_subQuery = subquery;
		}
		
		public override string ToString()
		{
			return _subQuery.ToString();
		}

		public override bool IsGrouped
		{
			get { return false; }
		}

		public override bool IsAggregate
		{
			get { return false; }
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return _subQuery.GetTypes();
		}

		public override SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery)
		{
			return _subQuery.ToSqlString(criteria, criteriaQuery)
				.Append(new SqlString(" as y", loc.ToString(), "_"));
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new InvalidOperationException("not a grouping projection");
		}
		
		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return _subQuery.GetTypedValues(criteria, criteriaQuery);
		}
	}
}
