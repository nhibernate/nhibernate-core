namespace NHibernate.Expression
{
	/// <summary></summary>
	public class LtPropertyExpression : PropertyExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="otherPropertyName"></param>
		public LtPropertyExpression( string propertyName, string otherPropertyName )
			: base( propertyName, otherPropertyName )
		{
		}

		/// <summary></summary>
		protected override string Op
		{
			get { return " < "; }
		}
	}
}