namespace NHibernate.Expression
{
	/// <summary>
	/// An Expression that combines two <see cref="Expression"/>s with an 
	/// <c>and</c> between them.
	/// </summary>
	public class AndExpression : LogicalExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="lhs"></param>
		/// <param name="rhs"></param>
		internal AndExpression( Expression lhs, Expression rhs ) : base( lhs, rhs )
		{
		}

		/// <summary>
		/// Get the Sql operator to put between the two <see cref="Expression"/>s.
		/// </summary>
		/// <value>Returns "<c>and</c>"</value>
		protected override string Op
		{
			get { return "and"; }
		}
	}
}