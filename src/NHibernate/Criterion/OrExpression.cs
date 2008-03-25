using System;

namespace NHibernate.Criterion
{
	/// <summary>
	/// An <see cref="ICriterion" /> that combines two <see cref="ICriterion"/>s with an 
	/// <c>"or"</c> between them.
	/// </summary>
	[Serializable]
	public class OrExpression : LogicalExpression
	{
		/// <summary>
		/// Initialize a new instance of the <see cref="OrExpression" /> class for 
		/// two <see cref="ICriterion"/>s.
		/// </summary>
		/// <param name="lhs">The <see cref="ICriterion"/> to use as the left hand side.</param>
		/// <param name="rhs">The <see cref="ICriterion"/> to use as the right hand side.</param>
		public OrExpression(ICriterion lhs, ICriterion rhs) : base(lhs, rhs)
		{
		}

		/// <summary>
		/// Get the Sql operator to put between the two <see cref="Expression"/>s.
		/// </summary>
		/// <value>Returns "<c>or</c>"</value>
		protected override string Op
		{
			get { return "or"; }
		}
	}
}