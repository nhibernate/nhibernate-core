namespace NHibernate.Expression
{
	/// <summary>
	/// Summary description for LikeExpression.
	/// </summary>
	public class LikeExpression : SimpleExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		internal LikeExpression( string propertyName, object value ) : base( propertyName, value )
		{
		}

		/// <summary></summary>
		protected override string Op
		{
			get { return " like "; }
		}
	}
}