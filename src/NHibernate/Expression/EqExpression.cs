namespace NHibernate.Expression
{
	/// <summary>
	/// An Expression that represents an "equal" constraint.
	/// </summary>
	public class EqExpression : SimpleExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		internal EqExpression( string propertyName, object value ) : base( propertyName, value )
		{
		}

		/// <summary></summary>
		protected override string Op
		{
			get { return " = "; }
		}
	}
}