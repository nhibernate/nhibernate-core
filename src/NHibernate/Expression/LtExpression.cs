namespace NHibernate.Expression
{
	/// <summary></summary>
	public class LtExpression : SimpleExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		internal LtExpression( string propertyName, object value ) : base( propertyName, value )
		{
		}

		/// <summary></summary>
		protected override string Op
		{
			get { return "<"; }
		}
	}
}