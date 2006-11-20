using System;

namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "less than" constraint 
	/// between two properties.
	/// </summary>
	[Serializable]
	public class LtPropertyExpression : PropertyExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LtPropertyExpression"/> class
		/// that compares two mapped properties using an "less than" constraint.
		/// </summary>
		/// <param name="lhsPropertyName">The name of the Property to use as the left hand side.</param>
		/// <param name="rhsPropertyName">The name of the Property to use as the right hand side.</param>
		public LtPropertyExpression( string lhsPropertyName, string rhsPropertyName )
			: base( lhsPropertyName, rhsPropertyName )
		{
		}

		/// <summary>
		/// Get the Sql operator to use for the <see cref="LtPropertyExpression"/>.
		/// </summary>
		/// <value>The string "<c> &lt; </c>"</value>
		protected override string Op
		{
			get { return " < "; }
		}
	}
}