using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary>
	/// The base class for an <see cref="ICriterion"/> that compares a single Property
	/// to a value.
	/// </summary>
	[Serializable]
	public class SimpleExpression : AbstractCriterion
	{
		private readonly IProjection projection;
		private readonly string propertyName;
		private readonly object value;
		private bool ignoreCase;
		private readonly string op;

		protected internal SimpleExpression(IProjection projection, object value, string op)
		{
			this.projection = projection;
			this.value = value;
			this.op = op;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="SimpleExpression" /> class for a named
		/// Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		/// <param name="op">The SQL operation.</param>
		public SimpleExpression(string propertyName, object value, string op)
		{
			this.propertyName = propertyName;
			this.value = value;
			this.op = op;
		}

		public SimpleExpression(string propertyName, object value, string op, bool ignoreCase)
			: this(propertyName, value, op)
		{
			this.ignoreCase = ignoreCase;
		}

		public SimpleExpression IgnoreCase()
		{
			ignoreCase = true;
			return this;
		}

		/// <summary>
		/// Gets the named Property for the Expression.
		/// </summary>
		/// <value>A string that is the name of the Property.</value>
		public string PropertyName
		{
			get { return propertyName; }
		}

		/// <summary>
		/// Gets the Value for the Expression.
		/// </summary>
		/// <value>An object that is the value for the Expression.</value>
		public object Value
		{
			get { return value; }
		}

		/// <summary>
		/// Converts the SimpleExpression to a <see cref="SqlString"/>.
		/// </summary>
		/// <returns>A SqlString that contains a valid Sql fragment.</returns>
		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlString[] columnNames =
				CriterionUtil.GetColumnNamesForSimpleExpression(propertyName, projection, criteriaQuery, criteria, enabledFilters,
				                                                 this, value);
			

			if (ignoreCase)
			{
				if (columnNames.Length != 1)
				{
					throw new HibernateException(
						"case insensitive expression may only be applied to single-column properties: " +
						propertyName);
				}

				return new SqlStringBuilder(6)
					.Add(criteriaQuery.Factory.Dialect.LowercaseFunction)
					.Add(StringHelper.OpenParen)
					.Add(columnNames[0])
					.Add(StringHelper.ClosedParen)
					.Add(Op)
					.AddParameter()
					.ToSqlString();
			}
			else
			{
				SqlStringBuilder sqlBuilder = new SqlStringBuilder(4 * columnNames.Length);

				for (int i = 0; i < columnNames.Length; i++)
				{
					if (i > 0)
					{
						sqlBuilder.Add(" and ");
					}

					sqlBuilder.Add(columnNames[i])
						.Add(Op)
						.AddParameter();
				}
				return sqlBuilder.ToSqlString();
			}
		}

		

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			object icvalue = ignoreCase ? value.ToString().ToLower() : value;

			return CriterionUtil.GetTypedValues(criteriaQuery, criteria, projection,propertyName, icvalue);
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (projection ?? (object)propertyName) + Op + value;
		}

		/// <summary>
		/// Get the Sql operator to use for the specific 
		/// subclass of <see cref="SimpleExpression"/>.
		/// </summary>
		protected virtual string Op
		{
			get { return op; }
		}
	}
}