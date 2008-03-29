namespace NHibernate.Criterion
{
	using System;
	using System.Collections.Generic;
	using Dialect;
	using Engine;
	using SqlCommand;

	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "like" constraint
	/// that is <b>not</b> case sensitive.
	/// </summary>
	//TODO:H2.0.3 renamed this to ILikeExpression
	[Serializable]
	public class InsensitiveLikeExpression : AbstractCriterion
	{
		private readonly string _propertyName;
		private readonly object _value;
		private readonly IProjection projection;

		/// <summary>
		/// Initializes a new instance of the <see cref="InsensitiveLikeExpression"/> class.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value.</param>
		/// <param name="matchMode">The match mode.</param>
		public InsensitiveLikeExpression(IProjection projection, string value, MatchMode matchMode)
		{
			this.projection = projection;
			this._value = matchMode.ToMatchString(value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InsensitiveLikeExpression"/> class.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="_value">The _value.</param>
		public InsensitiveLikeExpression(IProjection projection, object _value)
		{
			this.projection = projection;
			this._value = _value;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="InsensitiveLikeExpression" /> 
		/// class for a named Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public InsensitiveLikeExpression(string propertyName, object value)
		{
			_propertyName = propertyName;
			_value = value;
		}

		public InsensitiveLikeExpression(string propertyName, string value, MatchMode matchMode)
			: this(propertyName, matchMode.ToMatchString(value))
		{
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
		                                      IDictionary<string, IFilter> enabledFilters)
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();
			SqlString[] columnNames =
				CriterionUtil.GetColumnNames(_propertyName, projection, criteriaQuery, criteria, enabledFilters);

			if (columnNames.Length != 1)
			{
				throw new HibernateException("insensitive like may only be used with single-column properties");
			}

			if (criteriaQuery.Factory.Dialect is PostgreSQLDialect)
			{
				sqlBuilder.Add(columnNames[0]);
				sqlBuilder.Add(" ilike ");
			}
			else
			{
				sqlBuilder.Add(criteriaQuery.Factory.Dialect.LowercaseFunction)
					.Add("(")
					.Add(columnNames[0])
					.Add(")")
					.Add(" like ");
			}

			sqlBuilder.AddParameter();

			return sqlBuilder.ToSqlString();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return CriterionUtil.GetTypedValues(criteriaQuery, criteria, projection, _propertyName, _value.ToString().ToLower());
		}

		/// <summary></summary>
		public override string ToString()
		{
			return (projection ?? (object)_propertyName) + " ilike " + _value;
		}
	}
}