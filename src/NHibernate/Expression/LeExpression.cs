namespace NHibernate.Expression
{
	/// <summary></summary>
	public class LeExpression : SimpleExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		internal LeExpression( string propertyName, object value ) : base( propertyName, value )
		{
		}

		/// <summary></summary>
		protected override string Op
		{
			get { return " <= "; }
		}
	}
}