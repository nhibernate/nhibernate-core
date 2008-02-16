using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Expressions
{
	using System.Collections.Generic;

	/// <summary>
	/// A Count
	/// </summary>
	[Serializable]
	public class CountProjection : AggregateProjection
	{
		private bool distinct;

		protected internal CountProjection(String prop)
			: base("count", prop)
		{
		}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {NHibernateUtil.Int32};
		}

		public override string ToString()
		{
			return (distinct) ? "distinct " + base.ToString() : base.ToString();
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlStringBuilder buf = new SqlStringBuilder()
				.Add("count(");
			if (distinct)
			{
				buf.Add("distinct ");
			}
			buf.Add(criteriaQuery.GetColumn(criteria, propertyName))
				.Add(") as y")
				.Add(position.ToString())
				.Add("_");
			return buf.ToSqlString();
		}

		public CountProjection SetDistinct()
		{
			distinct = true;
			return this;
		}
	}
}