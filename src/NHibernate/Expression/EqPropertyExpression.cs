namespace NHibernate.Expression
{
	/// <summary></summary>
	public class EqPropertyExpression : PropertyExpression
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="otherPropertyName"></param>
		public EqPropertyExpression( string propertyName, string otherPropertyName )
			: base( propertyName, otherPropertyName )
		{
		}

		/// <summary></summary>
		protected override string Op
		{
			get { return " = "; }
		}
	}
}