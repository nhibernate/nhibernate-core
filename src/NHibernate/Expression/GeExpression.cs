using System;

namespace NHibernate.Expression {
	
	public class GeExpression : SimpleExpression {

		internal GeExpression(string propertyName, object value) : base (propertyName, value) {
		}

		protected override string Op {
			get { return ">="; }
		}
	}
}
