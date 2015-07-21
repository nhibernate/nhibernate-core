using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	[Serializable]
	public class GroupedProjection:IProjection
	{
		private readonly IProjection projection;

		public GroupedProjection(IProjection projection)
		{
			this.projection = projection;
		}

		public virtual SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return projection.ToSqlString(criteria, position, criteriaQuery, enabledFilters);
		}

		public virtual SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			return SqlStringHelper.RemoveAsAliasesFromSql(this.projection.ToSqlString(criteria, 0, criteriaQuery, enabledFilters));
		}

		public virtual IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetTypes(criteria, criteriaQuery);
		}

		public virtual IType[] GetTypes(String alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return this.projection.GetTypes(alias,criteria,criteriaQuery);
		}

		public string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return projection.GetColumnAliases(position, criteria, criteriaQuery);
		}

		public string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return null;
		}

		public virtual string[] Aliases
		{
			get { return new string[] { }; }
		}

		public virtual bool IsGrouped
		{
			get { return true; }
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
			return projection.ToString();
		}
	}
}
