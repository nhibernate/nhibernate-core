namespace NHibernate.Expression
{
	/// <summary>
	/// An <see cref="ICriterion"/> that represents an "like" constraint.
	/// </summary>
	/// <remarks>
	/// The case sensitivity depends on the database settings for string 
	/// comparisons.  Use <see cref="InsensitiveLikeExpression"/> if the
	/// string comparison should not be case sensitive.
	/// </remarks>
	public class LikeExpression : SimpleExpression
	{
		/// <summary>
		/// Initialize a new instance of the <see cref="LikeExpression" /> class for a named
		/// Property and its value.
		/// </summary>
		/// <param name="propertyName">The name of the Property in the class.</param>
		/// <param name="value">The value for the Property.</param>
		internal LikeExpression( string propertyName, object value ) : base( propertyName, value )
		{
		}

		/// <summary>
		/// Get the Sql operator to use for the <see cref="LikeExpression"/>.
		/// </summary>
		/// <value>The string "<c> like </c>"</value>
		protected override string Op
		{
			get { return " like "; }
		}
	}
}