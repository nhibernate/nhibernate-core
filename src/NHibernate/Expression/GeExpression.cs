namespace NHibernate.Expression
{
	/// <summary></summary>
	public class GeExpression : SimpleExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		internal GeExpression( string propertyName, object value ) : base( propertyName, value )
		{
		}

		/// <summary></summary>
		protected override string Op
		{
			get { return " >= "; }
		}
	}
}