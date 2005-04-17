namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "equal" constraint.
	/// </summary>
	public class EqExpression : SimpleExpression
	{
		/// <summary>
		/// Initialize a new instance of the <see cref="EqExpression" /> class for a named
		/// Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		internal EqExpression( string propertyName, object value ) : base( propertyName, value )
		{
		}

		/// <summary>
		/// Get the Sql operator to use for the <see cref="EqExpression"/>.
		/// </summary>
		/// <value>The string "<c> = </c>"</value>
		protected override string Op
		{
			get { return " = "; }
		}
	}
}