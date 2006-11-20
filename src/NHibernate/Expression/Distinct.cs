using System;

using NHibernate.Type;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	[Serializable]
	public class Distinct : IProjection
	{
		readonly IProjection projection;

		public Distinct(IProjection proj)
		{
			this.projection = proj;
		}

		public virtual SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
            return new SqlString("distinct ")
                .Append(projection.ToSqlString(criteria, position, criteriaQuery));
		}

		public virtual SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.ToGroupSqlString(criteria, criteriaQuery);
		}

		public virtual IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetTypes(criteria, criteriaQuery);
		}

		public virtual IType[] GetTypes(String alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetTypes(alias, criteria, criteriaQuery);
		}

		public virtual string[] GetColumnAliases(int loc)
		{
			return projection.GetColumnAliases(loc);
		}

		public virtual string[] GetColumnAliases(string alias, int loc)
		{
			return projection.GetColumnAliases(alias, loc);
		}

		public virtual string[] Aliases
		{
			get { return projection.Aliases; }
		}

		public virtual bool IsGrouped
		{
			get { return projection.IsGrouped; }
		}

		public override string ToString()
		{
			return "distinct " + projection.ToString();
		}

	}
}