using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Expression
{
	/// <summary>
	/// An identifier constraint
	/// </summary>
	[Serializable]
	public class IdentifierEqExpression : ICriterion
	{
		private readonly object value;

		public IdentifierEqExpression(object value)
		{
			this.value = value;
		}

		#region ICriterion Members

		public SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary enabledFilters)
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
					.Add(" = ")
					.AddParameter();
			}

			if (columns.Length > 1)
			{
				result.Add(StringHelper.ClosedParen);
			}
			return result.ToSqlString();
		}

		public TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] {criteriaQuery.GetTypedIdentifierValue(criteria, value)};
		}

		#endregion

		public override string ToString()
		{
			return base.ToString();
		}
	}
}