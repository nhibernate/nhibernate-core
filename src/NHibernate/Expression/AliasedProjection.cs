using System;
using System.Text;

using NHibernate.Type;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	public class AliasedProjection : IProjection
	{
		readonly IProjection projection;
		readonly string alias;

		protected internal AliasedProjection(IProjection projection, string alias)
		{
			this.projection = projection;
			this.alias = alias;
		}

		public virtual SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			return projection.ToSqlString(criteria, position, criteriaQuery);
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
			return this.alias.Equals(alias) ?
					GetTypes(criteria, criteriaQuery) :
					null;
		}

		public virtual string[] GetColumnAliases(int loc)
		{
			return projection.GetColumnAliases(loc);
		}

		public virtual string[] GetColumnAliases(string alias, int loc)
		{
			return this.alias.Equals(alias) ?
					GetColumnAliases(loc) :
					null;
		}

		public virtual string[] Aliases
		{
			get { return new string[] { alias }; }
		}

		public virtual bool IsGrouped { get { return projection.IsGrouped; } }

		public override string ToString()
		{
			return projection.ToString() + " as " + alias;
		}
	}
}
using System;
using System.Text;

using NHibernate.Type;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	public class AliasedProjection : IProjection
	{
		readonly IProjection projection;
		readonly string alias;

		protected internal AliasedProjection(IProjection projection, string alias)
		{
			this.projection = projection;
			this.alias = alias;
		}

		public virtual SqlString ToSqlString(ICriteria criteria, int position, ICriteriaQuery criteriaQuery)
		{
			return projection.ToSqlString(criteria, position, criteriaQuery);
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
			return this.alias.Equals(alias) ?
					GetTypes(criteria, criteriaQuery) :
					null;
		}

		public virtual string[] GetColumnAliases(int loc)
		{
			return projection.GetColumnAliases(loc);
		}

		public virtual string[] GetColumnAliases(string alias, int loc)
		{
			return this.alias.Equals(alias) ?
					GetColumnAliases(loc) :
					null;
		}

		public virtual string[] Aliases
		{
			get { return new string[] { alias }; }
		}

		public virtual bool IsGrouped { get { return projection.IsGrouped; } }

		public override string ToString()
		{
			return projection.ToString() + " as " + alias;
		}
	}
}
