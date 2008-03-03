using System;
using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that negates another <see cref="ICriterion"/>.
	/// </summary>
	[Serializable]
	public class NotExpression : AbstractCriterion
	{
		private ICriterion _criterion;

		/// <summary>
		/// Initialize a new instance of the <see cref="NotExpression" /> class for an
		/// <see cref="ICriterion"/>
		/// </summary>
		/// <param name="criterion">The <see cref="ICriterion"/> to negate.</param>
		public NotExpression(ICriterion criterion)
		{
			_criterion = criterion;
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			//TODO: set default capacity
			SqlStringBuilder builder = new SqlStringBuilder();

			bool needsParens = criteriaQuery.Factory.Dialect is MySQLDialect;
			if (needsParens)
			{
				builder.Add("not (");
			}
			else
			{
				builder.Add("not ");
			}

			builder.Add(_criterion.ToSqlString(criteria, criteriaQuery, enabledFilters));

			if (needsParens)
			{
				builder.Add(")");
			}

			return builder.ToSqlString();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return _criterion.GetTypedValues(criteria, criteriaQuery);
		}

		public override string ToString()
		{
			return "not " + _criterion.ToString();
		}
	}
}