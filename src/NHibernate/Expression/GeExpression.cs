using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "greater than or equal" constraint.
	/// </summary>
	[Serializable]
	public class GeExpression : SimpleExpression
	{
		/// <summary>
		/// Initialize a new instance of the <see cref="GeExpression" /> class for a named
		/// Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		public GeExpression(string propertyName, object value) : base(propertyName, value)
		{
		}

		/// <summary>
		/// Get the Sql operator to use for the <see cref="GeExpression"/>.
		/// </summary>
		/// <value>The string "<c> &gt;= </c>"</value>
		protected override string Op
		{
			get { return " >= "; }
		}
	}
}