using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary>
	/// A sequence of logical <see cref="ICriterion"/>s combined by some associative
	/// logical operator.
	/// </summary>
	[Serializable]
	public abstract class Junction : AbstractCriterion
	{
		private readonly IList<ICriterion> criteria = new List<ICriterion>();

		/// <summary>
		/// Adds an <see cref="ICriterion"/> to the list of <see cref="ICriterion"/>s
		/// to junction together.
		/// </summary>
		/// <param name="criterion">The <see cref="ICriterion"/> to add.</param>
		/// <returns>
		/// This <see cref="Junction"/> instance.
		/// </returns>
		public Junction Add(ICriterion criterion)
		{
			criteria.Add(criterion);
			return this;
		}

		/// <summary>
		/// Adds an <see cref="ICriterion"/> to the list of <see cref="ICriterion"/>s
		/// to junction together.
		/// </summary>
		public Junction Add<T>(Expression<Func<T, bool>> expression)
		{
			ICriterion criterion = ExpressionProcessor.ProcessExpression<T>(expression);
			criteria.Add(criterion);
			return this;
		}

		/// <summary>
		/// Adds an <see cref="ICriterion"/> to the list of <see cref="ICriterion"/>s
		/// to junction together.
		/// </summary>
		public Junction Add(Expression<Func<bool>> expression)
		{
			ICriterion criterion = ExpressionProcessor.ProcessExpression(expression);
			criteria.Add(criterion);
			return this;
		}

		/// <summary>
		/// Get the Sql operator to put between multiple <see cref="ICriterion"/>s.
		/// </summary>
		protected abstract String Op { get; }

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var typedValues = new List<TypedValue>();

			foreach (ICriterion criterion in this.criteria)
			{
				TypedValue[] subvalues = criterion.GetTypedValues(criteria, criteriaQuery);
				typedValues.AddRange(subvalues);
			}

			return typedValues.ToArray();
		}

		/// <summary>
		/// The <see cref="SqlString" /> corresponding to an instance with no added
		/// subcriteria.
		/// </summary>
		protected abstract SqlString EmptyExpression { get; }

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			if (this.criteria.Count == 0)
			{
				return EmptyExpression;
			}

			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();

			sqlBuilder.Add("(");

			for (int i = 0; i < this.criteria.Count - 1; i++)
			{
				sqlBuilder.Add(this.criteria[i].ToSqlString(criteria, criteriaQuery, enabledFilters));
				sqlBuilder.Add(Op);
			}

			sqlBuilder.Add(this.criteria[this.criteria.Count - 1].ToSqlString(criteria, criteriaQuery, enabledFilters));


			sqlBuilder.Add(")");

			return sqlBuilder.ToSqlString();
		}

		public override string ToString()
		{
			return '(' + StringHelper.Join(Op, criteria) + ')';
		}

		public override IProjection[] GetProjections()
		{
			return null;
		}
	}
}
