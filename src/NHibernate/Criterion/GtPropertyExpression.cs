using System;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "greater than" constraint 
	/// between two properties.
	/// </summary>
	[Serializable]
	public class GtPropertyExpression : PropertyExpression
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GtPropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsPropertyName">Name of the LHS property.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		public GtPropertyExpression(string lhsPropertyName, IProjection rhsProjection) : base(lhsPropertyName, rhsProjection)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GtPropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsProjection">The LHS projection.</param>
		/// <param name="rhsProjection">The RHS projection.</param>
		public GtPropertyExpression(IProjection lhsProjection, IProjection rhsProjection) : base(lhsProjection, rhsProjection)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GtPropertyExpression"/> class.
		/// </summary>
		/// <param name="lhsProjection">The projection.</param>
		/// <param name="rhsPropertyName">Name of the RHS property.</param>
		public GtPropertyExpression(IProjection lhsProjection, string rhsPropertyName) : base(lhsProjection, rhsPropertyName)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GtPropertyExpression"/> class
		/// that compares two mapped properties using an "greater than" constraint.
		/// </summary>
		/// <param name="lhsPropertyName">The name of the Property to use as the left hand side.</param>
		/// <param name="rhsPropertyName">The name of the Property to use as the right hand side.</param>
		public GtPropertyExpression(string lhsPropertyName, string rhsPropertyName)
			: base(lhsPropertyName, rhsPropertyName)
		{
		}

		/// <summary>
		/// Get the Sql operator to use for the <see cref="LtPropertyExpression"/>.
		/// </summary>
		/// <value>The string "<c> &lt; </c>"</value>
		protected override string Op
		{
			get { return " > "; }
		}
	}
}