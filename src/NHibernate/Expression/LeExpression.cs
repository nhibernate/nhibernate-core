using System;

namespace NHibernate.Expression 
{
	
	public class LeExpression : SimpleExpression 
	{

		internal LeExpression(string propertyName, object value) : base (propertyName, value) 
		{
		}

		protected override string Op 
		{
			get { return " <= "; }
		}
	}
}
