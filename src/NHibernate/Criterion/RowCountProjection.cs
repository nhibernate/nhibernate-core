using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
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

		public override SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			return new SqlString("count(*) as y", position.ToString(), "_");
		}

		public override string ToString()
		{
			return "count(*)";
		}

		public override bool IsGrouped
		{
			get { return false; }
		}

		public override SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{

			throw new InvalidOperationException("not a grouping projection");
		}
	}
}
