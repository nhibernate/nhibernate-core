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
		private readonly string propertyName;
		private readonly string value;
		private char? escapeChar;
		private readonly bool ignoreCase;
		private readonly IProjection _projection;

		public LikeExpression(string propertyName, string value, char? escapeChar, bool ignoreCase)
		{
			this.propertyName = propertyName;
			this.value = value;
			this.escapeChar = escapeChar;
			this.ignoreCase = ignoreCase;
		}

		public LikeExpression(IProjection projection, string value, MatchMode matchMode)
		{
			_projection = projection;
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
			string[] columns = criteriaQuery.GetColumnsUsingProjection(criteria, propertyName);
			if (columns.Length != 1)
				throw new HibernateException("Like may only be used with single-column properties");

			SqlStringBuilder lhs = new SqlStringBuilder(6);

			if(ignoreCase)
			{
				Dialect.Dialect dialect = criteriaQuery.Factory.Dialect;
				lhs.Add(dialect.LowercaseFunction).Add(StringHelper.OpenParen).Add(columns[0]).Add(
					StringHelper.ClosedParen);
			}
			else 
				lhs.Add(columns[0]);
			
			criteriaQuery.AddUsedTypedValues(GetTypedValues(criteria, criteriaQuery));
			lhs.Add(" like ").AddParameter();
			if (escapeChar.HasValue)
				lhs.Add(" escape '" + escapeChar + "'");
			return lhs.ToSqlString();
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return new TypedValue[] {criteriaQuery.GetTypedValue(criteria, propertyName, ignoreCase ? value.ToLower() : value)};
		}

		public override IProjection[] GetProjections()
		{
			if(_projection != null)
			{
				return new IProjection[] {_projection};
			}
			return null;
		}

		#endregion

		public override string ToString()
		{
			return (_projection != null ? _projection.ToString() : propertyName) + " like " + value;
		}
	}
}
