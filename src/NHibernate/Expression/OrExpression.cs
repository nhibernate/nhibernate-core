namespace NHibernate.Expression
{
	/// <summary>
	/// An Expression that combines two <see cref="Expression"/>s with an 
	/// <c>"or"</c> between them.
	/// </summary>
	public class OrExpression : LogicalExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		internal OrExpression( Expression lhs, Expression rhs ) : base( lhs, rhs )
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