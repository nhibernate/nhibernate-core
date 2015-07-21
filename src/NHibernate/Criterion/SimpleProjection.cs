using System;
using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Criterion
{
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
		
		public string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return GetColumnAliases(alias, position);
		}

		public String[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery) 
		{
			int numColumns = this.GetColumnCount(criteria, criteriaQuery);
			string[] aliases = new string[numColumns];
			for (int i = 0; i < numColumns; i++) 
			{
				aliases[i] = "y" + position + "_";
				position++;
			}
			return aliases;
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
		
		private int GetColumnCount(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			IType[] types = this.GetTypes(criteria, criteriaQuery);
			int count = 0;
			for (int i = 0; i < types.Length; i++) 
			{
				count += types[i].GetColumnSpan(criteriaQuery.Factory);
			}
			return count;
		}
	}
}
