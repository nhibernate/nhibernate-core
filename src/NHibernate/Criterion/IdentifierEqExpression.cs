using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An identifier constraint
	/// </summary>
	[Serializable]
	public class IdentifierEqExpression : ICriterion
	{
		private readonly object value;

		public IdentifierEqExpression(IProjection projection)
		{
			this.projection = projection;
		}

		private readonly IProjection projection;
		public IdentifierEqExpression(object value)
		{
			this.value = value;
		}

		#region ICriterion Members

		public SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			//Implementation changed from H3.2 to use SqlString
			string[] columns = criteriaQuery.GetIdentifierColumns(criteria);
			SqlStringBuilder result = new SqlStringBuilder(4 * columns.Length + 2);
			if (columns.Length > 1)
			{
				result.Add(StringHelper.OpenParen);
			}

			for (int i = 0; i < columns.Length; i++)
			{
				if (i > 0)
				{
					result.Add(" and ");
				}

				result.Add(columns[i])
					.Add(" = ");

				AddValueOrProjection(criteria, criteriaQuery, enabledFilters, result);
			}

			if (columns.Length > 1)
			{
				result.Add(StringHelper.ClosedParen);
			}
			return result.ToSqlString();
		}

		private void AddValueOrProjection(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters, SqlStringBuilder result)
		{
			if (projection == null)
			{
				result.AddParameter();
			}
			else
			{
				SqlString sql = projection.ToSqlString(criteria, GetHashCode(),criteriaQuery, enabledFilters);
				result.Add(StringHelper.RemoveAsAliasesFromSql(sql));
			}
		}

		public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] {criteriaQuery.GetTypedIdentifierValue(criteria, value)};
		}

		#endregion
	}
}