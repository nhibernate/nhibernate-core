using System;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="LogicalExpression"/> that combines two <see cref="ICriterion"/>s 
	/// with an <c>and</c> between them.
	/// </summary>
	[Serializable]
	public class AndExpression : LogicalExpression
	{
		/// <summary>
		/// Get the Sql operator to put between the two <see cref="ICriterion"/>s.
		/// </summary>
		/// <value>The string "<c>and</c>"</value>
		protected override string Op
		{
			get { return "and"; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AndExpression"/> class
		/// that combines two <see cref="ICriterion"/>.
		/// </summary>
		/// <param name="lhs">The <see cref="ICriterion"/> to use as the left hand side.</param>
		/// <param name="rhs">The <see cref="ICriterion"/> to use as the right hand side.</param>
		public AndExpression(ICriterion lhs, ICriterion rhs) : base(lhs, rhs)
		{
		}
	}
}