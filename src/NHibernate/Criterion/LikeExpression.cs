using System;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "like" constraint.
	/// </summary>
	/// <remarks>
	/// The case sensitivity depends on the database settings for string 
	/// comparisons.  Use <see cref="InsensitiveLikeExpression"/> if the
	/// string comparison should not be case sensitive.
	/// </remarks>
	[Serializable]
	public class LikeExpression : AbstractCriterion
	{
		private readonly string value;
		private char? escapeChar;
		private readonly bool ignoreCase;
		private readonly IProjection projection;

		public LikeExpression(string propertyName, string value, char? escapeChar, bool ignoreCase)
		{
			this.projection = Projections.Property(propertyName);
			this.value = value;
			this.escapeChar = escapeChar;
			this.ignoreCase = ignoreCase;
		}

		public LikeExpression(IProjection projection, string value, MatchMode matchMode)
		{
			this.projection = projection;
			this.value = matchMode.ToMatchString(value);
		}


		public LikeExpression(string propertyName, string value)
			: this(propertyName, value, null, false)
		{
		}

		public LikeExpression(string propertyName, string value, MatchMode matchMode)
			: this(propertyName, matchMode.ToMatchString(value))
		{
		}

		public LikeExpression(string propertyName, string value, MatchMode matchMode, char? escapeChar, bool ignoreCase)
			: this(propertyName, matchMode.ToMatchString(value), escapeChar, ignoreCase)
		{
		}

		#region ICriterion Members

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			SqlString[] columns = CriterionUtil.GetColumnNamesUsingProjection(projection, criteriaQuery, criteria, enabledFilters);
			if (columns.Length != 1)
				throw new HibernateException("Like may only be used with single-column properties / projections.");

			SqlStringBuilder lhs = new SqlStringBuilder(6);

			if (ignoreCase)
			{
				Dialect.Dialect dialect = criteriaQuery.Factory.Dialect;
				lhs.Add(dialect.LowercaseFunction)
					.Add(StringHelper.OpenParen)
					.Add(columns[0])
					.Add(StringHelper.ClosedParen);
			}
			else
				lhs.Add(columns[0]);

			criteriaQuery.AddUsedTypedValues(GetTypedValues(criteria, criteriaQuery));

			if (ignoreCase)
			{
				Dialect.Dialect dialect = criteriaQuery.Factory.Dialect;
				lhs.Add(" like ")
					.Add(dialect.LowercaseFunction)
					.Add(StringHelper.OpenParen)
					.AddParameter()
					.Add(StringHelper.ClosedParen);
			}
			else
				lhs.Add(" like ").AddParameter();

			if (escapeChar.HasValue)
				lhs.Add(" escape '" + escapeChar + "'");
			return lhs.ToSqlString();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] {
								new TypedValue(NHibernateUtil.String, value, EntityMode.Poco),
			                        };
		}

		public override IProjection[] GetProjections()
		{
			return new IProjection[] { projection };
		}

		#endregion

		public override string ToString()
		{
			return projection + " like " + value;
		}
	}
}