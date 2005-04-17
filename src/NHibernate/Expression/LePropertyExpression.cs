namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "less than or equal" constraint 
	/// between two properties.
	/// </summary>
	public class LePropertyExpression : PropertyExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LePropertyExpression"/> class
		/// that compares two mapped properties using an "less than or equal" constraint.
		/// </summary>
		/// <param name="lhsPropertyName">The name of the Property to use as the left hand side.</param>
		/// <param name="rhsPropertyName">The name of the Property to use as the right hand side.</param>
		public LePropertyExpression( string lhsPropertyName, string rhsPropertyName )
			: base( lhsPropertyName, rhsPropertyName )
		{
		}

		/// <summary>
		/// Get the Sql operator to use for the <see cref="LePropertyExpression"/>.
		/// </summary>
		/// <value>The string "<c> &lt;= </c>"</value>
		protected override string Op
		{
			get { return " <= "; }
		}
	}
}