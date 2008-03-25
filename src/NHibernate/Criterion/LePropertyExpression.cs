using System;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "less than or equal" constraint 
	/// between two properties.
	/// </summary>
	[Serializable]
	public class LePropertyExpression : PropertyExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LePropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsPropertyName">Name of the LHS property.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		public LePropertyExpression(string lhsPropertyName, IProjection rhsProjection) : base(lhsPropertyName, rhsProjection)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LePropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		public LePropertyExpression(IProjection lhsProjection, IProjection rhsProjection) : base(lhsProjection, rhsProjection)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LePropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsProjection">The projection.</param>
		/// <param name="rhsPropertyName">Name of the RHS property.</param>
		public LePropertyExpression(IProjection lhsProjection, string rhsPropertyName) : base(lhsProjection, rhsPropertyName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LePropertyExpression"/> class
		/// that compares two mapped properties using an "less than or equal" constraint.
		/// </summary>
		/// <param name="lhsPropertyName">The name of the Property to use as the left hand side.</param>
		/// <param name="rhsPropertyName">The name of the Property to use as the right hand side.</param>
		public LePropertyExpression(string lhsPropertyName, string rhsPropertyName)
			: base(lhsPropertyName, rhsPropertyName)
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