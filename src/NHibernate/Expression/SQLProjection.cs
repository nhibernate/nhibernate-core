using System;

using NHibernate.Type;
using NHibernate.SqlCommand;

namespace NHibernate.Expression
{
	/// <summary>
	/// A SQL fragment. The string {alias} will be replaced by the alias of the root entity.
	/// </summary>
	[Serializable]
	public sealed class SQLProjection : IProjection
	{
		readonly string sql;
		readonly string groupBy;
		readonly IType[] types;
		string[] aliases;
		string[] columnAliases;
		bool grouped;

		internal SQLProjection(string sql, string[] columnAliases, IType[] types)
			: this(sql, null, columnAliases, types)
		{
		}

		internal SQLProjection(string sql, string groupBy, string[] columnAliases, IType[] types)
		{
			this.sql = sql;
			this.types = types;
			this.aliases = columnAliases;
			this.columnAliases = columnAliases;
			this.grouped = groupBy != null;
			this.groupBy = groupBy;
		}

		public SqlString ToSqlString(ICriteria criteria, int loc, ICriteriaQuery criteriaQuery)
		{
            //SqlString result = new SqlString(criteriaQuery.GetSQLAlias(criteria));
            //result.Replace(sql, "{alias}");
            //return result;
			return new SqlString(Util.StringHelper.Replace(sql, "{alias}", criteriaQuery.GetSQLAlias(criteria)));
		}

		public SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new SqlString(Util.StringHelper.Replace(groupBy, "{alias}", criteriaQuery.GetSQLAlias(criteria)));
		}

		public override string ToString()
		{
			return sql;
		}

		public IType[] GetTypes(ICriteria crit, ICriteriaQuery criteriaQuery)
		{
			return types;
		}

		public string[] Aliases
		{
			get { return aliases; }
		}

		public string[] GetColumnAliases(int loc)
		{
			return columnAliases;
		}

		public bool IsGrouped
		{
			get { return grouped; }
		}

		public IType[] GetTypes(string alias, ICriteria crit, ICriteriaQuery criteriaQuery)
		{
			return null; //unsupported
		}

		public string[] GetColumnAliases(String alias, int loc)
		{
			return null; //unsupported
		}
	}
}
