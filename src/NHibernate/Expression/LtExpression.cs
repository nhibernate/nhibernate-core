using System;

namespace NHibernate.Expression {
	
	public class LtExpression : SimpleExpression {

		internal LtExpression(string propertyName, object value) : base (propertyName, value) {
		}

		protected override string Op {
			get { return "<"; }
		}
	}
}
