using System;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "equal" constraint 
	/// between two properties.
	/// </summary>
	[Serializable]
	public class EqPropertyExpression : PropertyExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EqPropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsPropertyName">Name of the LHS property.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		public EqPropertyExpression(string lhsPropertyName, IProjection rhsProjection) : base(lhsPropertyName, rhsProjection)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EqPropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		public EqPropertyExpression(IProjection lhsProjection, IProjection rhsProjection) : base(lhsProjection, rhsProjection)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EqPropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsProjection">The projection.</param>
		/// <param name="rhsPropertyName">Name of the RHS property.</param>
		public EqPropertyExpression(IProjection lhsProjection, string rhsPropertyName) : base(lhsProjection, rhsPropertyName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EqPropertyExpression"/> class
		/// that compares two mapped properties using an "equal" constraint.
		/// </summary>
		/// <param name="lhsPropertyName">The name of the Property to use as the left hand side.</param>
		/// <param name="rhsPropertyName">The name of the Property to use as the right hand side.</param>
		public EqPropertyExpression(string lhsPropertyName, string rhsPropertyName)
			: base(lhsPropertyName, rhsPropertyName)
		{
		}

		/// <summary>
		/// Get the Sql operator to use for the <see cref="EqPropertyExpression"/>.
		/// </summary>
		/// <value>The string "<c> = </c>"</value>
		protected override string Op
		{
			get { return " = "; }
		}
	}
}