using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	[Serializable]
	public class Distinct : IProjection
	{
		private readonly IProjection projection;

		public Distinct(IProjection proj)
		{
			this.projection = proj;
		}

		public virtual SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return new SqlString("distinct ")
				.Append(projection.ToSqlString(criteria, position, criteriaQuery,enabledFilters));
		}

		public virtual SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return projection.ToGroupSqlString(criteria, criteriaQuery,enabledFilters);
		}

		public virtual IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetTypes(criteria, criteriaQuery);
		}

		public virtual IType[] GetTypes(String alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetTypes(alias, criteria, criteriaQuery);
		}

		public virtual string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetColumnAliases(position, criteria, criteriaQuery);
		}

		public virtual string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetColumnAliases(alias, position, criteria, criteriaQuery);
		}

		public virtual string[] Aliases
		{
			get { return projection.Aliases; }
		}

		public virtual bool IsGrouped
		{
			get { return projection.IsGrouped; }
		}

		public bool IsAggregate
		{
			get { return projection.IsAggregate; }
		}

		/// <summary>
		/// Gets the typed values for parameters in this projection
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <param name="criteriaQuery">The criteria query.</param>
		/// <returns></returns>
		public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetTypedValues(criteria, criteriaQuery);
		}

		public override string ToString()
		{
			return "distinct " + projection.ToString();
		}
	}
}