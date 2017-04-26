using System;
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

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			var buf = new SqlStringBuilder().Add("count(");
			if (distinct)
			{
				buf.Add("distinct ");
			}
			if (projection != null)
			{
				buf.Add(SqlStringHelper.RemoveAsAliasesFromSql(projection.ToSqlString(criteria, position, criteriaQuery)));
			}
			else
			{
				buf.Add(criteriaQuery.GetColumn(criteria, propertyName));
			}

			buf.Add(") as y").Add(position.ToString()).Add("_");
			return buf.ToSqlString();
		}

		public CountProjection SetDistinct()
		{
			distinct = true;
			return this;
		}
	}
}
