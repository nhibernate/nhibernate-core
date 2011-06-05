using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "like" constraint
	/// that is <b>not</b> case sensitive.
	/// </summary>
	//TODO:H2.0.3 renamed this to ILikeExpression
	[Serializable]
	public class InsensitiveLikeExpression : AbstractCriterion
	{
		private readonly string propertyName;
		private readonly object value;
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
			this.value = matchMode.ToMatchString(value);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InsensitiveLikeExpression"/> class.
		/// </summary>
		/// <param name="projection">The projection.</param>
		/// <param name="value">The value.</param>
		public InsensitiveLikeExpression(IProjection projection, object value)
		{
			this.projection = projection;
			this.value = value;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="InsensitiveLikeExpression" /> 
		/// class for a named Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public InsensitiveLikeExpression(string propertyName, object value)
		{
			this.propertyName = propertyName;
			this.value = value;
		}

		public InsensitiveLikeExpression(string propertyName, string value, MatchMode matchMode)
			: this(propertyName, matchMode.ToMatchString(value))
		{
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			//TODO: add default capacity
			SqlStringBuilder sqlBuilder = new SqlStringBuilder();
			SqlString[] columnNames =
				CriterionUtil.GetColumnNames(propertyName, projection, criteriaQuery, criteria, enabledFilters);

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

			sqlBuilder.Add(criteriaQuery.NewQueryParameter(GetParameterTypedValue(criteria, criteriaQuery)).Single());

			return sqlBuilder.ToSqlString();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			List<TypedValue> typedValues = new List<TypedValue>();

			if (projection != null)
			{
				typedValues.AddRange(projection.GetTypedValues(criteria, criteriaQuery));
			}
			typedValues.Add(GetParameterTypedValue(criteria, criteriaQuery));
			
			return typedValues.ToArray();
		}

		public TypedValue GetParameterTypedValue(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			var matchValue = value.ToString().ToLower();
			if (projection != null)
			{
				return CriterionUtil.GetTypedValues(criteriaQuery, criteria, projection, null, matchValue).Single();
			}
			return criteriaQuery.GetTypedValue(criteria, propertyName, matchValue);
		}

		public override IProjection[] GetProjections()
		{
			if (projection != null)
			{
				return new IProjection[] { projection };
			}
			return null;
		}

		public override string ToString()
		{
			return (projection ?? (object)propertyName) + " ilike " + value;
		}
	}
}