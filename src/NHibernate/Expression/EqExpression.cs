using System;

namespace NHibernate.Expression {
	
	/// <summary>
	/// An Expression that represents an "equal" constraint.
	/// </summary>
	public class EqExpression : SimpleExpression 
	{

		internal EqExpression(string propertyName, object value) : base (propertyName, value) 
		{
		}

		protected override string Op 
		{
			get { return " = "; }
		}
	}
}
