using System;

using NHibernate.Type;
using NHibernate.SqlCommand;


namespace NHibernate.Expression
{
	/// <summary>
	/// A single-column projection that may be aliased
	/// </summary>
	[Serializable]
	abstract public class SimpleProjection : IProjection
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
			return new string[] { "y" + loc + "_" };
		}

		public virtual string[] Aliases
		{
			get { return new String[1]; }
		}

		public virtual SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			throw new InvalidOperationException("not a grouping projection");
		}

		public virtual bool IsGrouped { get { return false; } }

		abstract public SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery cirteriaQuery);

		abstract public IType[] GetTypes(ICriteria criteria, ICriteriaQuery criteriaQuery);
	}
}
