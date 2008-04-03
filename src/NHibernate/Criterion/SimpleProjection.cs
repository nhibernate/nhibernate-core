using System;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	using System.Collections.Generic;
	using Engine;

	/// <summary>
	/// A single-column projection that may be aliased
	/// </summary>
	[Serializable]
	public abstract class SimpleProjection : IProjection
	{
		public IProjection As(string alias)
		{
			return Projections.Alias(this, alias);
		}

		public virtual string[] GetColumnAliases(string alias, int loc)
		{
			return null;
		}

		public virtual IType[] GetTypes(string alias, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return null;
		}

		public virtual string[] GetColumnAliases(int loc)
		{
			return new string[] {"y" + loc + "_"};
		}

		public virtual string[] Aliases
		{
			get { return new String[1]; }
		}

		public abstract bool IsGrouped { get; }

		public abstract SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters);

		public abstract bool IsAggregate { get; }


		/// <summary>
		/// Gets the typed values for parameters in this projection
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <param name="criteriaQuery">The criteria query.</param>
		/// <returns></returns>
		public virtual TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[0];
		}

		public abstract SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters);

		public abstract IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery);
	}
}
