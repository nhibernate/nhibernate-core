using System;

namespace NHibernate.Expression {
	
	public class GtExpression : SimpleExpression {

		internal GtExpression(string propertyName, object value) : base (propertyName, value) {
		}

		protected override string Op {
			get { return " > "; }
		}
	}
}
