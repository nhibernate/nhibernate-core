using System;
using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary>
	/// A Count
	/// </summary>
	[Serializable]
	public class CountProjection : AggregateProjection
	{
		private bool distinct;

		protected internal CountProjection(String prop) : base("count", prop) {}
		protected internal CountProjection(IProjection projection) : base("count", projection) {}

		public override IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new IType[] {NHibernateUtil.Int32};
		}

		public override string ToString()
		{
			return (distinct) ? "distinct " + base.ToString() : base.ToString();
		}

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery,
		                                      IDictionary<string, IFilter> enabledFilters)
		{
			SqlStringBuilder buf = new SqlStringBuilder().Add("count(");
			if (distinct)
			{
				buf.Add("distinct ");
			}
		    string column;
            if(projection!=null)
            {
                column =
                    SqlStringHelper.RemoveAsAliasesFromSql(projection.ToSqlString(criteria, position, criteriaQuery,
                                                                               enabledFilters)).ToString();
            }
            else
            {
                column = criteriaQuery.GetColumn(criteria, propertyName);
            }

		    buf.Add(column).Add(") as y").Add(position.ToString()).Add("_");
			return buf.ToSqlString();
		}

		public CountProjection SetDistinct()
		{
			distinct = true;
			return this;
		}
	}
}