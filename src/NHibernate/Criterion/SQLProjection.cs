using System;
using System.Collections.Generic;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	using Engine;

	/// <summary>
	/// A SQL fragment. The string {alias} will be replaced by the alias of the root entity.
	/// Criteria aliases can also be used: "{a}.Value + {bc}.Value". Such aliases need to be registered via call to AddAliases("a", "bc")
	/// </summary>
	[Serializable]
	public sealed class SQLProjection : IProjection
	{
		private readonly string sql;
		private readonly string groupBy;
		private readonly IType[] types;
		private readonly string[] aliases;
		private readonly string[] columnAliases;
		private readonly bool grouped;
		private List<string> _criteriaAliases;

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
			return GetSqlString(criteria, criteriaQuery, sql);
		}

		/// <summary>
		/// Provide list of criteria aliases that's used in SQL projection.
		/// To be replaced with SQL aliases.
		/// </summary>
		public SQLProjection AddAliases(params string[] criteriaAliases)
		{
			if(_criteriaAliases == null)
				_criteriaAliases = new List<string>();

			_criteriaAliases.AddRange(criteriaAliases);

			return this;
		}

		public SqlString ToGroupSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return GetSqlString(criteria, criteriaQuery, groupBy);
		}

		private SqlString GetSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, string sqlTemplate)
		{
			return GetSqlString(criteria, criteriaQuery, new SqlString(sqlTemplate), _criteriaAliases);
		}

		internal static SqlString GetSqlString(
			ICriteria criteria,
			ICriteriaQuery criteriaQuery,
			SqlString sqlTemplate,
			List<string> criteriaAliases)
		{
			if (criteriaAliases != null)
			{
				foreach (var alias in criteriaAliases)
				{
					sqlTemplate = sqlTemplate.Replace(
						"{" + alias + "}",
						criteriaQuery is ICriteriaQueryNextVer cqNew
							? cqNew.GetSQLAlias(alias)
							: criteriaQuery.GetSQLAlias(criteria, alias + ".id"));
				}
			}

			return sqlTemplate.Replace("{alias}", criteriaQuery.GetSQLAlias(criteria));
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

		public bool IsGrouped
		{
			get { return grouped; }
		}

		public bool IsAggregate
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the typed values for parameters in this projection
		/// </summary>
		/// <param name="criteria">The criteria.</param>
		/// <param name="criteriaQuery">The criteria query.</param>
		/// <returns></returns>
		public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return Array.Empty<TypedValue>();
		}

		public IType[] GetTypes(string alias, ICriteria crit, ICriteriaQuery criteriaQuery)
		{
			return null; //unsupported
		}

		public string[] GetColumnAliases(int loc)
		{
			return columnAliases;
		}

		public string[] GetColumnAliases(String alias, int loc)
		{
			return null; //unsupported
		}

		public string[] GetColumnAliases(int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return columnAliases;
		}

		public string[] GetColumnAliases(string alias, int position, ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return null; //unsupported
		}
	}
}
