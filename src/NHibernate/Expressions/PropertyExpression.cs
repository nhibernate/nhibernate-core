using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.SqlCommand;
using NHibernate.Util;

namespace NHibernate.Expressions
{
	/// <summary>
	/// Superclass for an <see cref="ICriterion"/> that represents a
	/// constraint between two properties (with SQL binary operators).
	/// </summary>
	[Serializable]
	public abstract class PropertyExpression : AbstractCriterion
	{
		private string _lhsPropertyName;
		private string _rhsPropertyName;

		private static TypedValue[] NoTypedValues = new TypedValue[0];

		/// <summary>
		/// Initialize a new instance of the <see cref="PropertyExpression" /> class 
		/// that compares two mapped properties.
		/// </summary>
		/// <param name="lhsPropertyName">The name of the Property to use as the left hand side.</param>
		/// <param name="rhsPropertyName">The name of the Property to use as the right hand side.</param>
		protected PropertyExpression(string lhsPropertyName, string rhsPropertyName)
		{
			_lhsPropertyName = lhsPropertyName;
			_rhsPropertyName = rhsPropertyName;
		}

		public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters)
		{
			string[] columnNames = criteriaQuery.GetColumnsUsingProjection(criteria, _lhsPropertyName);
			string[] otherColumnNames = criteriaQuery.GetColumnsUsingProjection(criteria, _rhsPropertyName);

			string result = string.Join(
				" and ",
				StringHelper.Add(columnNames, Op, otherColumnNames)
				);

			if (columnNames.Length > 1)
			{
				result = StringHelper.OpenParen + result + StringHelper.ClosedParen;
			}

			return new SqlString(result);
			//TODO: get SQL rendering out of this package!
		}

		public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery)
		{
			return NoTypedValues;
		}

		/// <summary></summary>
		public override string ToString()
		{
			return _lhsPropertyName + Op + _rhsPropertyName;
		}

		/// <summary>
		/// Get the Sql operator to use for the property expression.
		/// </summary>
		protected abstract string Op { get; }
	}
}